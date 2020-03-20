using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Joseph.ServiceTrayMonitor.Annotations;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using File = System.IO.File;
using Prompt = Microsoft.Identity.Client.Prompt;
// ReSharper disable PossibleNullReferenceException

namespace Joseph.ServiceTrayMonitor
{
    /// <summary>
    /// CloudSyncWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CloudSyncWindow
    {
        private readonly MainWindow parentWindow;
        private AuthenticationResult authentication;
        private IPublicClientApplication app;
        [NotNull] private static IEnumerable<string> Scopes
            => Properties.Resources.MicrosoftGraphApiSpoce.Split('+');

        public CloudSyncWindow(MainWindow parentWindow)
        {
            this.parentWindow = parentWindow;
            InitializeComponent();
            Task.Run(ResumeLoginCache);
        }

        private async Task ResumeLoginCache()
        {
            InitPublicAppIfNull();
            var accounts = await app.GetAccountsAsync();
            try
            {
                authentication = await app
                    .AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                authentication = null;
            }
            SetLoginStateAuto();
        }

        private void InitPublicAppIfNull()
        {
            if(app != null) return;
            app = PublicClientApplicationBuilder
                .Create(Properties.Resources.MicrosoftGraphApiApplicationId)
                .WithDefaultRedirectUri()
                .Build();
            MicrosoftGraphTokenCacheHelper.EnableSerialization(app.UserTokenCache);
        }

        private void SetLoginStateAuto()
        {
            if (authentication != null)
            {
                SetLoginStateSuccess();
            }
            else
            {
                SetLoginStateFailed();
            }
        }

        private void SetLoginStateSuccess()
        {
            Dispatcher?.Invoke((ThreadStart) delegate
            {
                LoginButton.IsEnabled = false;
                LogoutButton.IsEnabled = true;
                accountLabel.Content = authentication.Account.Username;
                lastSyncLabel.Content = Properties.Resources.UICloudSyncLoginStateLoading;
            });
            Task.Run(GetAndSetLastSyncDate);
        }

        private void SetLoginStateFailed()
        {
            Dispatcher?.Invoke((ThreadStart) delegate
            {
                LoginButton.IsEnabled = true;
                LogoutButton.IsEnabled = false;
                UploadButton.IsEnabled = false;
                DownloadButton.IsEnabled = false;
                accountLabel.Content = "未登录";
                lastSyncLabel.Content = string.Empty;
            });
        }

        private async Task GetAndSetLastSyncDate()
        {
            DateTimeOffset? lastModify;
            try
            {
                var graphClient = new GraphServiceClient(GetAuthenticationProvider());
                var items = await graphClient
                    .Me.Drive.Special.AppRoot.Children.Request().GetAsync();
                lastModify = items.FirstOrDefault(
                    x => x.Name == Properties.Resources.PathCloudSyncConfigFile)
                    ?.LastModifiedDateTime;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    Properties.Resources.MsgCloudSyncGetLastSyncDateFailedError,
                    ex.Message));
                lastModify = null;
            }

            Dispatcher?.Invoke((ThreadStart) delegate
            {
                UploadButton.IsEnabled = true;
                DownloadButton.IsEnabled = (lastModify != null);
                lastSyncLabel.Content =
                    lastModify?
                        .ToLocalTime()
                        .ToString(Properties.Resources.ConstDataTimeFormat) ?? 
                    Properties.Resources.UICloudSyncLastSyncStateNotSynced;
            });
        }

        private InteractiveAuthenticationProvider GetAuthenticationProvider()
        {
            return new InteractiveAuthenticationProvider(app, Scopes);
        }

        private async Task Login()
        {
            InitPublicAppIfNull();
            try
            {
                authentication = await app
                    .AcquireTokenInteractive(Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();
            }
            catch (MsalClientException e)
            {
                authentication = null;
                MessageBox.Show(e.Message);
            }
        }

        private void LoginOnClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            var loginTask = Task.Run(Login);
            loginTask.Wait();
            IsEnabled = true;
            SetLoginStateAuto();
        }

        private async void LogoutOnClick(object sender, RoutedEventArgs e)
        {
            InitPublicAppIfNull();
            var accounts = await app.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await app.RemoveAsync(account);
            }

            app = null;
            authentication = null;
            if(File.Exists(MicrosoftGraphTokenCacheHelper.CacheFilePath))
                File.Delete(MicrosoftGraphTokenCacheHelper.CacheFilePath);
            SetLoginStateFailed();
            MessageBox.Show(Properties.Resources.MsgCloudSyncMicrosoftAccountLogout);
        }

        private void CloseOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            parentWindow.cloudSyncWindow = null;
            parentWindow.configListView.DataContext = parentWindow.config;
            parentWindow.configListView.Items.Refresh();
        }

        private async void UploadOnClick(object sender, RoutedEventArgs e)
        {
            var graphClient = new GraphServiceClient(GetAuthenticationProvider());
            IsEnabled = false;
            var tempBackupTime = lastSyncLabel.Content;
            lastSyncLabel.Content = Properties.Resources.UICloudSyncLastSyncStateSyncing;
            try
            {
                await graphClient.Me.Drive.Special.AppRoot
                    .Children[Properties.Resources.PathCloudSyncConfigFile].Content
                    .Request().PutAsync<DriveItem>(
                        new MemoryStream(
                            Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(
                                    parentWindow.config.ToList()))));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    Properties.Resources.MsgCloudSyncUploadFailedError, ex.Message));
                lastSyncLabel.Content = tempBackupTime;
                IsEnabled = true;
                return;
            }
            MessageBox.Show(Properties.Resources.MsgCloudSyncUploadSuccess);
            lastSyncLabel.Content = DateTime.Now.ToString(Properties.Resources.ConstDataTimeFormat);
            IsEnabled = true;
        }

        private async void DownloadOnClick(object sender, RoutedEventArgs e)
        {
            var graphClient = new GraphServiceClient(GetAuthenticationProvider());
            IsEnabled = false;
            var tempBackupTime = lastSyncLabel.Content;
            try
            {
                lastSyncLabel.Content = Properties.Resources.UICloudSyncLastSyncStateSyncing;
                var configContentStream =
                    await graphClient.Me.Drive.Special.AppRoot
                        .Children[Properties.Resources.PathCloudSyncConfigFile].Content
                        .Request().GetAsync();
                var configContentBytes = new byte[configContentStream.Length];
                await configContentStream.ReadAsync(configContentBytes, 0, configContentBytes.Length);
                string configContentString = Encoding.UTF8.GetString(configContentBytes);
                List<ConfigItem> syncedConfigItems =
                    JsonConvert.DeserializeObject<List<ConfigItem>>(configContentString);
                if (syncedConfigItems == null)
                    throw new NullReferenceException();
                parentWindow.config = syncedConfigItems.ToObservableCollection();
                parentWindow.SaveConfig();
                MessageBox.Show(Properties.Resources.MsgCloudSyncDownloadSuccess);
                IsEnabled = true;
                lastSyncLabel.Content = DateTime.Now.ToString(Properties.Resources.ConstDataTimeFormat);
                return;
            }
            catch (JsonException)
            {
                MessageBox.Show(Properties.Resources.MsgCloudSyncParseError);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show(Properties.Resources.MsgCloudSyncNullOrEmptyRemoteFileError);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    Properties.Resources.MsgCloudSyncDownloadFailedError, ex.Message));
            }
            lastSyncLabel.Content = tempBackupTime;
            IsEnabled = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Joseph.ServiceTrayMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public ServiceSelectWindow serviceSelectWindow;
        public CloudSyncWindow cloudSyncWindow;
        public FileStream configFileStream;
        public ObservableCollection<ConfigItem> config;

        public string ServiceName
        {
            get => serviceNameTextBox.Text;
            set => serviceNameTextBox.Text = value;
        }

        public string TrayDisplayName
        {
            get => trayDisplayNameTextBox.Text;
            set => trayDisplayNameTextBox.Text = value;
        }

        public string LogFilePath
        {
            get => logPathTextBox.Text;
            set => logPathTextBox.Text = value;
        }

        public MainWindow()
        {
            InitializeConfig();
            InitializeComponent();
            configListView.DataContext = config;
        }

        private void InitializeConfig()
        {
            configFileStream = new FileStream(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                + Properties.Resources.PathConfigFile,
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            LoadConfig();
        }

        public void LoadConfig()
        {
            try
            {
                config = (JsonConvert.DeserializeObject<List<ConfigItem>>(configFileStream.ReadAllString())
                          ?? new List<ConfigItem>()).ToObservableCollection();
            }
            catch (JsonException)
            {
                MessageBox.Show(Properties.Resources.MsgConfigFileBroken);
                config = new List<ConfigItem>().ToObservableCollection();
                SaveConfig();
            }
            ((App)Application.Current).ReloadTrayMenu();
        }

        public void SaveConfig()
        {
            configFileStream.WriteAllString(JsonConvert.SerializeObject(config.ToList()));
        }

        private void OnHelpButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(Properties.Resources.MsgHelp, 
                ((ConfigItem)configListView.SelectedItem)?.Id));
        }

        private void OnSelectLogFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = Environment.SystemDirectory,
                Filter = Properties.Resources.ConstLogFileSelectorFilter,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() ?? false)
            {
                IsDataChanged = true;
                logPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void OnCloudSyncButtonClick(object sender, RoutedEventArgs e)
        {
            cloudSyncWindow = new CloudSyncWindow(this);
            try
            {
                cloudSyncWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    Properties.Resources.MsgCloudSyncGeneralError, ex.Message));
            }

            ((App) Application.Current).ReloadTrayMenu();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (IsDataChanged)
            {
                if (MessageBox.Show(
                    Properties.Resources.MsgConfirmMainWindowClose,
                    string.Empty,
                    MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            }
            ((App)Application.Current).ReloadTrayMenu();
            Hide();
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Joseph.ServiceTrayMonitor
{
    public enum NotifySaveResult
    {
        OK,
        Cancel
    }

    public partial class MainWindow
    {
        private bool _isDataChanged;
        public bool IsDataChanged
        {
            get => _isDataChanged;
            set
            {
                _isDataChanged = value;
                saveButton.Content = value ? 
                    Properties.Resources.UISaveButtonContentTextEdited :
                    Properties.Resources.UISaveButtonContentTextNormal;
            }
        }

        public bool IsDataSelected => (configListView.SelectedItem != null);

        public void OnConfigEdit(object sender, KeyEventArgs e)
        {
            IsDataChanged = true;
        }

        public void OnConfigSave(object sender, RoutedEventArgs e)
        {
            IsDataChanged = false;
            SaveDataDependOnIfSelected((ConfigItem)configListView.SelectedItem);
        }

        public void OnConfigNew(object sender, RoutedEventArgs e)
        {
            if (IsDataChanged)
            {
                if (NotifySave((ConfigItem)configListView.SelectedItem) == NotifySaveResult.Cancel)
                {
                    return;
                }
            }

            RestoreInitialEditState();
        }

        public void OnConfigOpenLog(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(logPathTextBox.Text))
            {
                Process.Start(
                    Path.Combine(
                        Environment.GetEnvironmentVariable(
                            Properties.Resources.EnviromentNameWindir) ??
                        Properties.Resources.PathDefaultWindowsRoot,
                        Properties.Resources.PathExplorerExe)
                    , logPathTextBox.Text);
            }
            else
            {
                MessageBox.Show(Properties.Resources.MsgNoVaildLogPath);
            }
        }

        public void OnConfigDelete(object sender, RoutedEventArgs e)
        {
            if (IsDataSelected)
            {
                if (MessageBox.Show(Properties.Resources.MsgConfirmDelete, 
                    string.Empty,
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    config.Where(q => q.Id == ((ConfigItem)configListView.SelectedItem).Id)
                        .ToList().ForEach(x => config.Remove(x));
                    SaveConfig();
                    configListView.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.MsgSelectItemToBeDeleted);
            }
            ((App)Application.Current).ReloadTrayMenu();
        }

        public void OnConfigSelect(object sender, SelectionChangedEventArgs e)
        {
            if (IsDataChanged)
            {
                ConfigItem oldValue = e.RemovedItems.Cast<ConfigItem>().FirstOrDefault();
                ConfigItem validValue = oldValue ?? (ConfigItem)configListView.SelectedItem;
                if (NotifySave(validValue) == NotifySaveResult.Cancel)
                {
                    if (validValue != null)
                    {
                        configListView.SelectedItem = oldValue;
                    }
                }
            }

            SwitchSelectedItemData((ConfigItem)configListView.SelectedItem);
        }

        public void OnSelectServiceClick(object sender, RoutedEventArgs e)
        {
            IsDataChanged = true;
            serviceSelectWindow = new ServiceSelectWindow(this);
            serviceSelectWindow.Show();
            Hide();
        }

        public NotifySaveResult NotifySave(ConfigItem configItem)
        {
            MessageBoxResult saveMessageBoxResult = MessageBox.Show(
                Properties.Resources.MsgConfirmSave,
                string.Empty,
                MessageBoxButton.YesNoCancel);
            switch (saveMessageBoxResult)
            {
                case MessageBoxResult.Yes:
                    SaveDataDependOnIfSelected(configItem);
                    return NotifySaveResult.OK;
                case MessageBoxResult.No:
                    return NotifySaveResult.OK;
                case MessageBoxResult.Cancel:
                    return NotifySaveResult.Cancel;
            }
            return NotifySaveResult.Cancel;
        }

        private void SaveDataDependOnIfSelected(ConfigItem configItem)
        {
            if (IsDataSelected)
            {
                Guid selectedGuid = configItem.Id;
                config.Where(q => q.Id == selectedGuid).ToList().ForEach(x =>
                {
                    x.TrayDisplayName = TrayDisplayName;
                    x.LogFilePath = LogFilePath;
                    x.ServiceName = ServiceName;
                });
            }
            else
            {
                config.Add(new ConfigItem()
                {
                    Id = Guid.NewGuid(),
                    TrayDisplayName = TrayDisplayName,
                    LogFilePath = LogFilePath,
                    ServiceName = ServiceName
                });
            }

            SaveConfig();
            configListView.Items.Refresh();
            ((App)Application.Current).ReloadTrayMenu();
        }

        private void RestoreInitialEditState()
        {
            IsDataChanged = false;
            TrayDisplayName = string.Empty;
            LogFilePath = string.Empty;
            ServiceName = string.Empty;
            if (configListView.SelectedItem != null)
            {
                configListView.SelectedItem = null;
            }
            ((App)Application.Current).ReloadTrayMenu();
        }

        private void SwitchSelectedItemData(ConfigItem selectedItem)
        {
            IsDataChanged = false;
            if (selectedItem == null) return;
            TrayDisplayName = selectedItem.TrayDisplayName;
            LogFilePath = selectedItem.LogFilePath;
            ServiceName = selectedItem.ServiceName;
        }
    }
}

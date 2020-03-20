using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Joseph.ServiceTrayMonitor.Annotations;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Win32;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

// ReSharper disable LocalizableElement

namespace Joseph.ServiceTrayMonitor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : Application
    {
        public HostedNotifyIcon HostedNotifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Contains(ServiceTrayMonitor.Properties.Resources.ArgAutoRunUser))
            {
                RegHelper.AutoStart();
            }
            base.OnStartup(e);
            AppCenter.Start(ServiceTrayMonitor.Properties.Resources.AppCenterToken,
                typeof(Analytics), typeof(Crashes));
            HostedNotifyIcon = new HostedNotifyIcon();
            HostedNotifyIcon.Show();
            if (Current.MainWindow == null)
            {
                Current.MainWindow = new MainWindow();
                Current.MainWindow.Hide();
            }
            if (e.Args.Contains(ServiceTrayMonitor.Properties.Resources.ArgHideMainWindow))
                return;
            Current.MainWindow.Show();
        }

        public void ReloadTrayMenu()
        {
            HostedNotifyIcon.Dispose();
            HostedNotifyIcon = new HostedNotifyIcon();
            HostedNotifyIcon.Show();
            var itemList = ((MainWindow) MainWindow)?.config?.ToList() ?? new List<ConfigItem>();
            var toolStripItems = new List<ToolStripItem>();
            itemList.ForEach(item =>{ toolStripItems.Add(GetServiceMenu(item)); });
            toolStripItems.Reverse();
            toolStripItems.ForEach(
                item => { HostedNotifyIcon.contextMenuStrip.Items.Insert(0, item); });
        }

        private ToolStripItem GetServiceMenu(ConfigItem configItem)
        {
            var serviceItemMenu =
                new ToolStripMenuItem { Text = configItem.TrayDisplayName };
            var value = new ToolStripLabel(
                ServiceTrayMonitor.Properties.Resources.UITrayMenuItemStatusPlaceHolder
                + ServiceTrayMonitor.Properties.Resources.UITrayMenuItemStatusPending)
            {
                Name = configItem.ServiceName,
            };
            serviceItemMenu.DropDownItems.Add(value);
            serviceItemMenu.DropDownItems.Add(new ToolStripSeparator());
            AddClickEventItem(configItem.ServiceName, serviceItemMenu,
                ServiceTrayMonitor.Properties.Resources.UITrayMenuItemOperationStart,
                ServiceTrayMonitor.Properties.Resources.IconStart,
                OnServiceStartClick);
            AddClickEventItem(configItem.ServiceName, serviceItemMenu,
                ServiceTrayMonitor.Properties.Resources.UITrayMenuItemOperationStop,
                ServiceTrayMonitor.Properties.Resources.IconStop,
                OnServiceStopClick);
            AddClickEventItem(configItem.ServiceName, serviceItemMenu,
                ServiceTrayMonitor.Properties.Resources.UITrayMenuItemOperationRestart,
                ServiceTrayMonitor.Properties.Resources.IconRestart,
                OnServiceRestartClick);
            serviceItemMenu.DropDownItems.Add(new ToolStripSeparator());
            AddClickEventItem(configItem.ServiceName, serviceItemMenu,
                ServiceTrayMonitor.Properties.Resources.UITrayMenuItemOperationShowLog,
                null,
                OnServiceShowLogClick,
                string.IsNullOrWhiteSpace(configItem.LogFilePath));
            return serviceItemMenu;
        }

        public static ServiceController GetServiceByServiceName(string serviceName)
        {
            return ServiceController.GetServices()
                .FirstOrDefault(x => x.ServiceName == serviceName);
        }

        private void OnServiceShowLogClick(object sender, EventArgs e)
        {
            var logFilePath = GetConfigItemBySender(sender).LogFilePath;
            if (string.IsNullOrWhiteSpace(logFilePath)) return;
            Process.Start(
                Path.Combine(
                    Environment.GetEnvironmentVariable(
                        ServiceTrayMonitor.Properties.Resources.EnviromentNameWindir) ??
                    ServiceTrayMonitor.Properties.Resources.PathDefaultWindowsRoot, 
                    ServiceTrayMonitor.Properties.Resources.PathExplorerExe)
                , logFilePath);
        }

        private void OnServiceRestartClick(object sender, EventArgs e)
        {
            var serviceName = GetConfigItemBySender(sender)?.ServiceName;
            if (string.IsNullOrWhiteSpace(serviceName)) return;
            Task.Run(() =>
            {
                var service = GetServiceByServiceName(serviceName);
                service?.Stop();
                service?.Start();
                MessageBox.Show(string.Format(
                    ServiceTrayMonitor.Properties.Resources.MsgServiceRestartSuccess,
                    serviceName));
            });
        }

        

        private void OnServiceStopClick(object sender, EventArgs e)
        {
            var serviceName = GetConfigItemBySender(sender)?.ServiceName;
            if (string.IsNullOrWhiteSpace(serviceName)) return;
            Task.Run(() =>
            {
                GetServiceByServiceName(serviceName)?.Stop();
                MessageBox.Show(string.Format(
                    ServiceTrayMonitor.Properties.Resources.MsgServiceStopSuccess,
                    serviceName));
            });
        }

        private void OnServiceStartClick(object sender, EventArgs e)
        {
            var serviceName = GetConfigItemBySender(sender)?.ServiceName;
            if (string.IsNullOrWhiteSpace(serviceName)) return;
            Task.Run(() =>
            {
                GetServiceByServiceName(serviceName)?.Start();
                MessageBox.Show(string.Format(
                    ServiceTrayMonitor.Properties.Resources.MsgServiceStartSuccess,
                    serviceName));
            });
        }

        private ConfigItem GetConfigItemBySender(object sender)
        {
            return ((MainWindow)MainWindow)?.config?
                .Where(x => x.ServiceName == ((ToolStripMenuItem)sender).Name)
                .FirstOrDefault();
        }

        private static void AddClickEventItem(string name,
            [NotNull] ToolStripMenuItem serviceItemMenu,
            string text,
            [CanBeNull] Icon icon,
            EventHandler eventHandler,
            bool isDisabled = false)
        {
            if (serviceItemMenu == null) throw new ArgumentNullException(nameof(serviceItemMenu));
            var item = new ToolStripMenuItem(text)
            {
                Enabled = !isDisabled,
                Name = name
            };
            if (!isDisabled)
            {
                item.Click += eventHandler;
            }
            if(icon != null) { item.Image = icon.ToBitmap(); }
            serviceItemMenu.DropDownItems.Add(item);
        }
    }
}

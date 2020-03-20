using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using Joseph.ServiceTrayMonitor.Properties;

namespace Joseph.ServiceTrayMonitor
{
    public partial class HostedNotifyIcon : Control
    {
        public HostedNotifyIcon()
        {
            InitializeComponent();
            InitTimer();
            AddClickEventForAllServicesOperation();
        }

        private void InitTimer()
        {
            var timerConfigFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Resources.PathTimerConfigFile;
            if (!File.Exists(timerConfigFilePath))
            {
                File.WriteAllText(timerConfigFilePath, Resources.ConstDefaultInterval);
                MessageBox.Show(string.Format(
                    Resources.MsgCreatedTimerConfigFile,
                    Resources.ConstDefaultInterval,
                    timerConfigFilePath));
            }

            var timerIntervalString = File.ReadAllText(timerConfigFilePath).Trim();
            if (!int.TryParse(timerIntervalString, out var interval))
            {
                MessageBox.Show(string.Format(Resources.MsgUnableToLoadConfigTimerIntervalMs, Resources.ConstDataTimeFormat));
                interval = int.Parse(Resources.ConstDefaultInterval);
            }
            timer.Interval = interval;
            timer.Start();
        }

        private void AddClickEventForAllServicesOperation()
        {
            toolStripMenuItemStartAll.Click += (sender, args) => {
                var itemList = ((MainWindow)System.Windows.Application.Current.MainWindow)?.config?.ToList()
                               ?? new List<ConfigItem>();
                var exList = new List<string>();
                itemList.ForEach(x =>
                {
                    try
                    {
                        if (App.GetServiceByServiceName(x.ServiceName)?.Status == ServiceControllerStatus.Running)
                            throw new Exception(string.Format(Resources.MsgStartingStartedService, x.ServiceName));
                        App.GetServiceByServiceName(x.ServiceName)?.Start();
                    }
                    catch (Exception e)
                    {
                        exList.Add(e.Message);
                    }
                });
                if (exList.Count != 0)
                {
                    MessageBox.Show(string.Format(Resources.MsgServiceStopAllHaveError, string.Join("\n", exList)));
                    return;
                }
                MessageBox.Show(Resources.MsgServiceStartAllSuccess);
            };

            toolStripMenuItemStopAll.Click += (sender, args) => {
                var itemList = ((MainWindow)System.Windows.Application.Current.MainWindow)?.config?.ToList()
                               ?? new List<ConfigItem>();
                var exList = new List<string>();
                itemList.ForEach(x =>
                {
                    try
                    {
                        if (App.GetServiceByServiceName(x.ServiceName)?.Status == ServiceControllerStatus.Stopped)
                            throw new Exception(string.Format(Resources.MsgStoppingStopedService, x.ServiceName));
                        App.GetServiceByServiceName(x.ServiceName)?.Stop();
                    }
                    catch (Exception e)
                    {
                        exList.Add(e.Message);
                    }
                });
                if (exList.Count != 0)
                {
                    MessageBox.Show(string.Format(Resources.MsgServiceStopAllHaveError, string.Join("\n", exList)));
                    return;
                }
                MessageBox.Show(Resources.MsgServiceStopAllSuccess);
            };
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ToolStripMenuItemShowMain_OnClick(sender, e);
            
        }

        private void ToolStripMenuItemExit_OnClick(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ToolStripMenuItemShowMain_OnClick(object sender, EventArgs e)
        {
            var mw = (MainWindow)System.Windows.Application.Current.MainWindow ?? new MainWindow();
            mw.Show();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var services = ServiceController.GetServices();
            foreach (var item in contextMenuStrip.Items)
            {
                if(!(item is ToolStripMenuItem)) continue;
                var menuItem = (ToolStripMenuItem) item;
                if (!menuItem.HasDropDownItems) continue;
                var status =  services
                    .FirstOrDefault(x => x.ServiceName == menuItem.DropDownItems[0].Name)
                    ?.Status;
                menuItem.DropDownItems[0].Text =
                    Properties.Resources.UITrayMenuItemStatusPlaceHolder + status.ToLocalizedString();
            }
        }
    }
}

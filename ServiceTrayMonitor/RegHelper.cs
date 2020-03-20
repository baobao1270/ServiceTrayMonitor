using System;
using System.Security.Principal;
using Microsoft.Win32.TaskScheduler;

namespace Joseph.ServiceTrayMonitor
{
    public static class RegHelper
    {
        public static void AutoStart()
        {
            var ts = new TaskService();
            var td = ts.NewTask();
            td.RegistrationInfo.Description = Properties.Resources.RegValueName;
            td.Principal.DisplayName = Properties.Resources.RegValueName;
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Settings.MultipleInstances = TaskInstancesPolicy.Parallel;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            td.Triggers.Add(new LogonTrigger(){ UserId = WindowsIdentity.GetCurrent().Name });
            td.Actions.Add(new ExecAction(System.Windows.Forms.Application.ExecutablePath,
                Properties.Resources.ArgHideMainWindow));
            ts.RootFolder.RegisterTaskDefinition(Properties.Resources.RegValueName, td);
        }
    }
}
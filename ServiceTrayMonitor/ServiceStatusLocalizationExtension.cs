using System.Collections.Generic;
using System.ServiceProcess;

namespace Joseph.ServiceTrayMonitor
{
    public static class ServiceStatusLocalizationExtension
    {
        public static Dictionary<ServiceControllerStatus, string> Mapping
        = new Dictionary<ServiceControllerStatus, string>
        {
            { ServiceControllerStatus.Stopped, ServiceStateLocalizationResource.Stopped },
            { ServiceControllerStatus.StartPending, ServiceStateLocalizationResource.StartPending },
            { ServiceControllerStatus.StopPending, ServiceStateLocalizationResource.StopPending },
            { ServiceControllerStatus.Running, ServiceStateLocalizationResource.Running },
            { ServiceControllerStatus.ContinuePending, ServiceStateLocalizationResource.ContinuePending },
            { ServiceControllerStatus.PausePending, ServiceStateLocalizationResource.PausePending },
            { ServiceControllerStatus.Paused, null }
        };

        public static string ToLocalizedString(this ServiceControllerStatus? serviceStatus)
        {
            if (serviceStatus == null)
            {
                return ServiceStateLocalizationResource.Unknown;
            }
            return Mapping[serviceStatus.Value];
        }
    }
}

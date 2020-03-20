using System;
using Joseph.ServiceTrayMonitor.Annotations;

namespace Joseph.ServiceTrayMonitor
{
    public class ConfigItem
    {
        public Guid Id { get; set; }
        public string TrayDisplayName { get; set; }
        public string LogFilePath { get; set; }
        public string ServiceName { get; set; }
    }
}

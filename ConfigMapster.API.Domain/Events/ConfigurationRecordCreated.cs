using System;

namespace ConfigMapster.API.Domain.Events
{
    public class ConfigurationRecordCreated: BaseEvent
    {
        public bool IsActive { get;  set; }
        public string Environment { get;  set; }
        public string ApplicationName { get;  set; }
        public string Key { get;  set; }
        public string Value { get;  set; }
        public string Type { get;  set; }
        public int Version { get; set; }
    }
}

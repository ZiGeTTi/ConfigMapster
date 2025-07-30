using System;

namespace ConfigMapster.API.Domain.Events
{
    public class ConfigurationRecordUpdated:BaseEvent
    {
        public bool IsActive { get; private set; }
        public string Environment { get; private set; }
        public string ApplicationName { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public string Type { get; private set; }
        public int Version { get; set; }
    }
}

using System;

namespace ConfigMapster.API.Domain.Events
{
    public class ConfigurationRecordDeleted: BaseEvent
    {
        public string Environment { get; set; }
        public string ApplicationName { get; set; }
        public string Key { get; set; }

    }
}

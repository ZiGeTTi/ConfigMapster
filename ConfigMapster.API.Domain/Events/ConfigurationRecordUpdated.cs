using System;

namespace ConfigMapster.API.Domain.Events
{
    public class ConfigurationRecordUpdated:BaseEvent
    {
        public int Version { get; set; }
    }
}

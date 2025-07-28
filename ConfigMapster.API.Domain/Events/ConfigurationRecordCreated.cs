using System;

namespace ConfigMapster.API.Domain.Events
{
    public class ConfigurationRecordCreated: BaseEvent
    {
        public Guid ConfigurationRecordId { get; set; }
    }
}

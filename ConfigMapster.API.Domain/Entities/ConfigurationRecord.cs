using ConfigMapster.API.Domain.Events;
using ConfigurationApi.Entities.ValueObjects;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ConfigurationApi.Entities
{
    public class ConfigurationRecord : BaseEntity
    {
        public bool IsActive { get; private set; }
        public string Environment { get; private set; }
        public string ApplicationName { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public string Type { get; private set; }

        public ConfigurationRecord(
            string environment,
            string applicationName,
            string key,
            string value,
            string type,
            bool isActive = true,
            IdentityValueObject identity = null,
            AuditValueObject audit = null
            ) : base(identity)
        {
            IsActive = isActive;
            Audit = new AuditValueObject(DateTime.Now, null);
            
            Environment = environment;
            ApplicationName = applicationName;
            Key = key;
            Value = value;
            Type = type;
        }
        public void Create()
        {
            if (string.IsNullOrWhiteSpace(ApplicationName) ||
                string.IsNullOrWhiteSpace(Key) || string.IsNullOrWhiteSpace(Value) || string.IsNullOrWhiteSpace(Type))
            {
                throw new ValidationException("Environment, ApplicationName, Key, Value, and Type cannot be null or empty.");
            }
            RaiseDomainEvent(new ConfigurationRecordCreated
            {
                IsActive = IsActive,
                Environment = Environment,
                ApplicationName = ApplicationName,
                Key = Key,
                Value = Value,
                Type = Type,
                Version = IdentityObject.Version
            });
        }
        public void Update(string value, string type)
        {
            Value = value.Trim();
            Type = type.Trim();
            RaiseDomainEvent(new ConfigurationRecordUpdated
            {
                ConfigurationRecordId = IdentityObject.Id,
                Version = IdentityObject.Version,
            });
        }

        public void Delete()
        {
            IsActive = false;
            RaiseDomainEvent(new ConfigurationRecordDeleted()
            {
                ConfigurationRecordId = IdentityObject.Id,
                ApplicationName = ApplicationName,
                Key = Key,
            });
        }
    }
}

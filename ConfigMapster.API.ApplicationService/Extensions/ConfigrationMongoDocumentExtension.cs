using ConfigurationApi.Documents;
using ConfigurationApi.Entities;
using ConfigurationApi.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMapster.API.ApplicationService
{
    public static class ConfigrationMongoDocumentExtension
    {
        public static ConfigurationRecordDocument ToDocument(this ConfigurationRecord configurationRecord)
        {
            if (configurationRecord == null)
                return null;

            return new ConfigurationRecordDocument
            {
                Id = configurationRecord.IdentityObject.Id,
                Version = configurationRecord.IdentityObject.Version,
                CreatedAt = configurationRecord.Audit.CreatedAt,
                LastModifiedAt = configurationRecord.Audit.UpdatedAt,
                IsActive = configurationRecord.IsActive,
                Environment = configurationRecord.Environment,
                ApplicationName = configurationRecord.ApplicationName,
                Key = configurationRecord.Key,
                Value = configurationRecord.Value,
                Type = configurationRecord.Type
            };
        }
        public static ConfigurationRecord ToEntity(this ConfigurationRecordDocument document)
        {
            if (document == null)
                return null;
            return new ConfigurationRecord
            (
                document.Environment,
                document.ApplicationName,
                document.Key,
                document.Value,
                document.Type,
                document.IsActive,
                new IdentityValueObject(document.Id, document.Version),
                new AuditValueObject(document.CreatedAt, document.LastModifiedAt)
            );
        }
    }
}

using ConfigMapster.API.Domain.Events;
using ConfigurationApi.Entities.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace ConfigurationApi.Entities
{
    public class BaseEntity
    {
        public BaseEntity(IdentityValueObject identityObject)
        {
            IdentityObject = identityObject ?? new IdentityValueObject();
        }

        public IdentityValueObject IdentityObject { get; }

        public AuditValueObject Audit { get; protected set; }
        private readonly List<BaseEvent> _domainEvents = new();

        public IReadOnlyList<BaseEvent> GetDomainEvents()
        {
            return _domainEvents.ToList();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        protected void RaiseDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}

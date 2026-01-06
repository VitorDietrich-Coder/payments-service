using Payments.Microservice.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Microservice.Domain.Abstractions
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void Apply(IDomainEvent @event)
        {
            When(@event);
            _domainEvents.Add(@event);
        }

        protected abstract void When(IDomainEvent @event);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }

}

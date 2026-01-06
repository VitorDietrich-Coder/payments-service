
using Payments.Microservice.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Payments.Microservice.Domain.Core.Events
{
    public abstract class DomainEvent
    {
        public Guid AggregateId { get; }
        public DateTime OccurredAt { get; }
        public string CorrelationId { get; }

        protected DomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            OccurredAt = DateTime.UtcNow;
            CorrelationId = CorrelationContext.Current;
        }
    }
}

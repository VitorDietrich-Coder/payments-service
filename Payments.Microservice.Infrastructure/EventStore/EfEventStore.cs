using Games.Microservice.Infrastructure.EventStore;
using Payments.Microservice.Domain.Core.Events;
using Payments.Microservice.Domain.Interfaces;
using Payments.Microservice.Infrastructure.Persistence;
using System.Text.Json;


namespace Payments.Microservice.Infrastructure.EventStore
{
    public class StoredEvents : IEventStore
    {
        private readonly PaymentsDbContext _context;

        public StoredEvents(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(DomainEvent @event)
        {
            var storedEvent = new StoredEvent
            {
                Id = Guid.NewGuid(),
                AggregateId = @event.AggregateId,
                CorrelationId = @event.CorrelationId,
                Type = @event.GetType().Name,
                Data = JsonSerializer.Serialize(@event),
                OccurredAt = @event.OccurredAt
            };

            _context.StoredEvents.Add(storedEvent);
    
        }

        public async Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId)
        {
            var events = _context.StoredEvents
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.OccurredAt)
                .ToList();

            return events.Select(e =>
                JsonSerializer.Deserialize<DomainEvent>(e.Data)!);
        }
    }
}

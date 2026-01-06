using Payments.Microservice.Domain.Core.Events;

namespace Payments.Microservice.Domain.Interfaces
{
    public interface IEventStore
    {
        Task SaveAsync(DomainEvent @event);
        Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId);
    }
}

using Payments.Microservice.Domain.Enums;

namespace Payments.Microservice.Domain.Events;

public sealed record PaymentCreatedEvent(
    Guid AggregateId,
    Guid UserId,
    Guid GameId,
    decimal Price,
    PaymentStatus status
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

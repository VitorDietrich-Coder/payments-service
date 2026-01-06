namespace Payments.Contracts.Events;

public record PaymentCompletedIntegrationEvent(
    Guid PaymentId,
    Guid GameId,
    Guid UserId,
    decimal Price,
    string CorrelationId
);

 namespace Payments.Contracts.Events
{
    public record GamePurchasedIntegrationEvent(
        Guid GameId,
        Guid UserId,
        decimal Price,
        string CorrelationId
    );

}

namespace Payments.Microservice.Application.DTO;

public sealed class PaymentDto
{
    public Guid PaymentId { get; init; }
    public Guid GameId { get; init; }
    public Guid UserId { get; init; }
    public decimal Amount { get; init; }
    public string Status { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
}

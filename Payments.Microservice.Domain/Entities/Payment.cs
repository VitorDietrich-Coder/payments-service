 
using Payments.Microservice.Domain.Abstractions;
using Payments.Microservice.Domain.Enums;
using Payments.Microservice.Domain.Events;
using Payments.Microservice.Domain.ValueObjects;

public class Payment : AggregateRoot
{

    protected Payment() { }

    public Payment  (
        Guid userId,
        Guid gameId,
        CurrencyAmount price)
    {   
        Apply(new PaymentCreatedEvent(
            Guid.NewGuid(),
            userId,
            gameId,
            price.Value,
            PaymentStatus.Pending));
 
    }
    public Guid UserId { get;  set; }
    public Guid GameId { get;  set; }
    public CurrencyAmount Price { get;  set; }
    public PaymentStatus Status { get; set; }

    public void Complete()
        => Apply(new PaymentCompletedEvent(Id));

    public void Fail(string reason)
        => Apply(new PaymentFailedEvent(Id, reason));

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case PaymentCreatedEvent e:
                Id = e.AggregateId;
                UserId = e.UserId;
                GameId = e.GameId;
                Price = new CurrencyAmount(e.Price);
                Status = PaymentStatus.Pending;
                break;

            case PaymentCompletedEvent:
                Status = PaymentStatus.Completed;
                break;

            case PaymentFailedEvent:
                Status = PaymentStatus.Failed;
                break;
        }
    }
}

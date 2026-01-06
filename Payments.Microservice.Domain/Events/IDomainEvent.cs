namespace Payments.Microservice.Domain.Events
{
    public interface IDomainEvent
    {
        public DateTime OccurredOn { get; }
    }
}

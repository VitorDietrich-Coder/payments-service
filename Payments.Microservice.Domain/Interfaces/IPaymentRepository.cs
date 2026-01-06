namespace Payments.Microservice.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}

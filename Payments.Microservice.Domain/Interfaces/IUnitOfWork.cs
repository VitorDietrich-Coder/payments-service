namespace Payments.Microservice.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken ct);
    }
}

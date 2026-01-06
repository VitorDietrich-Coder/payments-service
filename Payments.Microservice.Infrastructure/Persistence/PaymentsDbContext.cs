using Games.Microservice.Infrastructure.EventStore;
using Games.Microservice.Infrastructure.MapEntities;
using Microsoft.EntityFrameworkCore;
using Payments.Microservice.Domain.Core.Events;

namespace Payments.Microservice.Infrastructure.Persistence;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<StoredEvent> StoredEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.Ignore<List<DomainEvent>>();
        modelBuilder.Ignore<IReadOnlyCollection<DomainEvent>>();

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PaymentConfiguration).Assembly
        );

    }

}

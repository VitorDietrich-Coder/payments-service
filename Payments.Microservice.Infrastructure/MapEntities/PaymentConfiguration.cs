using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;

namespace Games.Microservice.Infrastructure.MapEntities
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                    .IsRequired();

            builder.Property(x => x.GameId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.OwnsOne(x => x.Price, price =>
            {
                price.Property(p => p.Value)
                     .HasColumnName("Price")
                     .HasPrecision(18, 2)
                     .IsRequired();

                price.Property(p => p.Currency)
                     .HasColumnName("PriceCurrency")
                     .HasMaxLength(3)
                     .IsRequired();
            });
        }
    }
}

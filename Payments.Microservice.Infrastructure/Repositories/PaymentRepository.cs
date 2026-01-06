using Microsoft.EntityFrameworkCore;
using Payments.Microservice.Domain.Interfaces;
using Payments.Microservice.Infrastructure.Persistence;

namespace Games.Microservice.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentsDbContext _context;

        public PaymentRepository(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Payments.Where(user => user.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Payment payment)
        {
            var existingUser = await _context.Payments
                .FirstOrDefaultAsync(u => u.Id == payment.Id);

            if (existingUser is null)
                return;

            _context.Entry(existingUser).CurrentValues.SetValues(payment);

            return;
        }
    }
}
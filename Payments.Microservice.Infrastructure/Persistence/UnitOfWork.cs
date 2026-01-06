using Payments.Microservice.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Microservice.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentsDbContext _context;

        public UnitOfWork(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

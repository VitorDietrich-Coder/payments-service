 using Microsoft.EntityFrameworkCore;

namespace Payments.Microservice.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly PaymentsDbContext _context;

    public ApplicationDbContextInitialiser(PaymentsDbContext context)
    {
        _context = context;
    }

    public void Initialise()
    {
        // Early development strategy
        //_context.Database.EnsureDeleted();
        //_context.Database.EnsureCreated();
        //_context.Database.Migrate();

        // Late development strategy
        if (_context.Database.IsSqlServer())
        {
            _context.Database.Migrate();
        }
        else
        {
            _context.Database.EnsureCreated();
        }
    }
}

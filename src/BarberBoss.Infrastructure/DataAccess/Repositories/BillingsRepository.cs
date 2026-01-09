using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingsRepository : IBillingsWriteOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public BillingsRepository(BarberBossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Billing billing) => await _dbContext.Billings.AddAsync(billing);
}

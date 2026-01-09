using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingsRepository : IBillingsWriteOnlyRepository, IBillingsReadOnlyRepository, IBillingsUpdateOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public BillingsRepository(BarberBossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Billing billing) => await _dbContext.Billings.AddAsync(billing);

    public async Task<List<Billing>> GetAll() => await _dbContext.Billings.AsNoTracking().ToListAsync();

    async Task<Billing?> IBillingsReadOnlyRepository.GetById(Guid id) => await _dbContext.Billings.AsNoTracking().FirstOrDefaultAsync(billing => billing.Id == id);

    async Task<Billing?> IBillingsUpdateOnlyRepository.GetById(Guid id) => await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);

    public void Update(Billing billing) => _dbContext.Billings.Update(billing);

    public async Task<bool> Delete(Guid id)
    {
        var result = await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);
        if (result is null)
            return false;

        _dbContext.Billings.Remove(result);

        return true;
    }
}

using AccountManagement.Data;
using AccountManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AccountManagementDbContext dbContext;

        public CurrencyRepository(AccountManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Currency> CreateAsync(Currency currency)
        {
            await dbContext.Currencies.AddAsync(currency);
            await dbContext.SaveChangesAsync();

            return currency;
        }

        public async Task<Currency?> DeleteAsync(Guid id)
        {
            var existingCurrency = await dbContext.Currencies.FirstOrDefaultAsync(x => x.Id == id);

            if (existingCurrency == null)
            {
                return null;
            }

            dbContext.Currencies.Remove(existingCurrency);
            await dbContext.SaveChangesAsync();

            return existingCurrency;
        }

        public async Task<List<Currency>> GetAllAsync()
        {
            return await dbContext.Currencies.ToListAsync();
        }

        public async Task<Currency?> GetByIdAsync(Guid id)
        {
            return await dbContext.Currencies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Currency?> UpdateAsync(Guid id, Currency currency)
        {
            var existingCurrency = await dbContext.Currencies.FirstOrDefaultAsync(x => x.Id == id);

            if (existingCurrency == null)
            {
                return null;
            }

            existingCurrency.Code = currency.Code;
            existingCurrency.Description = currency.Description;
            existingCurrency.ExchangeRate = currency.ExchangeRate;
            existingCurrency.DateModified = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return existingCurrency;
        }
    }
}

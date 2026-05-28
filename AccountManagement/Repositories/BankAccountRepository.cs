using AccountManagement.Data;
using AccountManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly AccountManagementDbContext dbContext;

        public BankAccountRepository(AccountManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BankAccount> CreateAsync(BankAccount bankAccount)
        {
            await dbContext.BankAccounts.AddAsync(bankAccount);
            await dbContext.SaveChangesAsync();

            return bankAccount;
        }

        public async Task<BankAccount?> DeleteAsync(Guid id)
        {
            var bankAccount = await dbContext.BankAccounts.FirstOrDefaultAsync(x => x.Id == id);

            if (bankAccount == null)
            {
                return null;
            }

            dbContext.BankAccounts.Remove(bankAccount);
            await dbContext.SaveChangesAsync();

            return bankAccount;
        }

        public async Task<List<BankAccount>> GetAllAsync()
        {
            return await dbContext.BankAccounts.ToListAsync();
        }

        public async Task<BankAccount?> GetByIdAsync(Guid id)
        {
            return await dbContext.BankAccounts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BankAccount?> UpdateAsync(Guid id, BankAccount bankAccount)
        {
            var existingBankAccount = await dbContext.BankAccounts.FirstOrDefaultAsync(x => x.Id == id);

            if (existingBankAccount == null)
            {
                return null;
            }

            existingBankAccount.Code = bankAccount.Code;
            existingBankAccount.Name = bankAccount.Name;
            existingBankAccount.Balance = bankAccount.Balance;
            existingBankAccount.IsActive = bankAccount.IsActive;
            existingBankAccount.CurrencyId = bankAccount.CurrencyId;
            existingBankAccount.ClientId = bankAccount.ClientId;
            existingBankAccount.DateModified = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return existingBankAccount;
        }
    }
}

using AccountManagement.Data;
using AccountManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Repositories
{
    public class BankTransactionRepository : IBankTransactionRepository
    {
        private readonly AccountManagementDbContext dbContext;

        public BankTransactionRepository(AccountManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<BankTransaction> CreateAsync(BankTransaction bankTransaction)
        {
            await dbContext.BankTransactions.AddAsync(bankTransaction);
            await dbContext.SaveChangesAsync();

            return bankTransaction;
        }

        public async Task<BankTransaction?> DeleteAsync(Guid id)
        {
            var bankTransaction = await dbContext.BankTransactions.FirstOrDefaultAsync(x => x.Id == id);

            if (bankTransaction == null)
            {
                return null;
            }

            dbContext.Remove(bankTransaction);
            await dbContext.SaveChangesAsync();

            return bankTransaction;
        }

        public async Task<List<BankTransaction>> GetAllAsync()
        {
            return await dbContext.BankTransactions.ToListAsync();
        }

        public async Task<BankTransaction?> GetByIdAsync(Guid id)
        {
            return await dbContext.BankTransactions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BankTransaction?> UpdateAsync(Guid id, BankTransaction bankTransaction)
        {
            var existingBankTransaction = await dbContext.BankTransactions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingBankTransaction == null)
            {
                return null;
            }

            existingBankTransaction.BankAccountId = bankTransaction.BankAccountId;
            existingBankTransaction.Action = bankTransaction.Action;
            existingBankTransaction.Amount = bankTransaction.Amount;
            existingBankTransaction.IsActive = bankTransaction.IsActive;
            existingBankTransaction.DateModified = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return existingBankTransaction;
        }
    }
}

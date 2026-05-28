using AccountManagement.Data;
using AccountManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly AccountManagementDbContext dbContext;

        public ReportsRepository(AccountManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<BankAccount>> GetAccountReports()
        {
            return await dbContext.BankAccounts.Where(a => a.IsActive).Include(a => a.Client).Include(a => a.Currency).ToListAsync();
        }

        public async Task<List<BankAccount>> GetClientAccountReports(Guid id)
        {
            return await dbContext.BankAccounts.Where(a => a.ClientId == id).Where(a => a.IsActive).ToListAsync();
        }

        public async Task<List<BankTransaction>> GetTransactionReports(Guid id)
        {
            return await dbContext.BankTransactions.Where(t => t.BankAccountId == id).OrderByDescending(t => t.DateCreated).ToListAsync();
        }
    }
}

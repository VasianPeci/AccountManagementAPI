using AccountManagement.Models.Domain;

namespace AccountManagement.Repositories
{
    public interface IReportsRepository
    {
        Task<List<BankAccount>> GetAccountReports();
        Task<List<BankTransaction>> GetTransactionReports(Guid id);
        Task<List<BankAccount>> GetClientAccountReports(Guid id);
    }
}

using AccountManagement.Models.Domain;

namespace AccountManagement.Repositories
{
    public interface IBankTransactionRepository
    {
        Task<BankTransaction> CreateAsync(BankTransaction bankTransaction);
        Task<List<BankTransaction>> GetAllAsync();
        Task<BankTransaction?> GetByIdAsync(Guid id);
        Task<BankTransaction?> UpdateAsync(Guid id, BankTransaction bankTransaction);
        Task<BankTransaction?> DeleteAsync(Guid id);
    }
}

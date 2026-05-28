using AccountManagement.Models.Domain;

namespace AccountManagement.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount> CreateAsync(BankAccount bankAccount);
        Task<List<BankAccount>> GetAllAsync();
        Task<BankAccount?> GetByIdAsync(Guid id);
        Task<BankAccount?> UpdateAsync(Guid id, BankAccount bankAccount);
        Task<BankAccount?> DeleteAsync(Guid id);
    }
}

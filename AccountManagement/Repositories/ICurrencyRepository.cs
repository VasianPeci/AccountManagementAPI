using AccountManagement.Models.Domain;

namespace AccountManagement.Repositories
{
    public interface ICurrencyRepository
    {
        Task<Currency> CreateAsync(Currency currency);
        Task<List<Currency>> GetAllAsync();
        Task<Currency?> GetByIdAsync(Guid id);
        Task<Currency?> UpdateAsync(Guid id, Currency currency);
        Task<Currency?> DeleteAsync(Guid id);
    }
}

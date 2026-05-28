using AccountManagement.Models.Domain;
using AccountManagement.DTO;

namespace AccountManagement.Repositories
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(Guid id);
    }
}

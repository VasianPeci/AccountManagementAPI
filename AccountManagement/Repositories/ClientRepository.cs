using Microsoft.EntityFrameworkCore;
using AccountManagement.Data;
using AccountManagement.Models.Domain;
using AccountManagement.DTO;

namespace AccountManagement.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AccountManagementDbContext dbContext;

        public ClientRepository(AccountManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Client>> GetAllAsync()
        {
            return await dbContext.Clients.ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await dbContext.Clients.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}

using AccountManagement.Models.Domain;
using AccountManagement.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace AccountManagement.Data
{
    public class AccountManagementDbContext : DbContext
    {

        public AccountManagementDbContext(DbContextOptions<AccountManagementDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankTransaction> BankTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var now = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            // Seed data for currencies
            var currencies = new List<Currency>()
            {
                new Currency()
                {
                    Id = Guid.Parse("6bfde49c-9552-4978-99a4-5b7a03c20edd"),
                    Code = "USD",
                    Description = "US Dollar",
                    ExchangeRate = 1.00m,
                    DateCreated = now
                },
                new Currency()
                {
                    Id = Guid.Parse("e90405c6-bf44-4802-a938-865985224263"),
                    Code = "EUR",
                    Description = "Euro",
                    ExchangeRate = 0.92m,
                    DateCreated = now
                },
                new Currency()
                {
                    Id = Guid.Parse("78e87249-4bc8-4a82-82c8-ab13459e926d"),
                    Code = "GBP",
                    Description = "British Pound",
                    ExchangeRate = 0.79m,
                    DateCreated = now
                },
                new Currency()
                {
                    Id = Guid.Parse("55e399e4-d78e-4a09-b816-bb60ece33279"),
                    Code = "JPY",
                    Description = "Japanese Yen",
                    ExchangeRate = 155.00m,
                    DateCreated = now
                }
            };

            // Seed DB with Currencies
            modelBuilder.Entity<Currency>().HasData(currencies);           
        }
    }
}

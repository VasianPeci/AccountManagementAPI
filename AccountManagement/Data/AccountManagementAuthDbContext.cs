using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AccountManagement.Models.Identity;

namespace AccountManagement.Data
{
    public class AccountManagementAuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AccountManagementAuthDbContext(
            DbContextOptions<AccountManagementAuthDbContext> options
        ) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var clientRoleId = "d200261a-70ad-4937-a2c6-42041ed608e5";
            var adminRoleId = "4704cd91-4bcd-43e1-a7f7-424662a78a13";
            var auditorRoleId = "99eb0e7f-20c0-4889-a0db-21c5b81249bd";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = clientRoleId,
                    ConcurrencyStamp = clientRoleId,
                    Name = "Client",
                    NormalizedName = "CLIENT"
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = auditorRoleId,
                    ConcurrencyStamp = auditorRoleId,
                    Name = "Auditor",
                    NormalizedName = "AUDITOR"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
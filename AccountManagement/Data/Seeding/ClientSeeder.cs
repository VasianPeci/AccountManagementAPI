using AccountManagement.Models.Domain;
using AccountManagement.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace AccountManagement.Data.Seeding
{
    public class ClientSeeder
    {
        private readonly AccountManagementDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public ClientSeeder(
            AccountManagementDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (db.Clients.Any())
                return;

            var u1 = await userManager.FindByEmailAsync("client1@test.com");
            var u2 = await userManager.FindByEmailAsync("client2@test.com");
            var u3 = await userManager.FindByEmailAsync("client3@test.com");
            var u4 = await userManager.FindByEmailAsync("client4@test.com");

            if (u1 == null || u2 == null || u3 == null || u4 == null)
            {
                throw new Exception("Users must be seeded before clients. Identity seeding failed.");
            }

            var now = new DateTime(2026, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            db.Clients.AddRange(
                new Client
                {
                    Id = Guid.Parse("8e5f0c35-f696-4204-b56d-754c2afa9e0c"),
                    FirstName = "John",
                    LastName = "Doe",
                    Birthdate = new DateTime(2000, 1, 1),
                    Phone = "111111111",
                    DateCreated = now,
                    UserId = u1.Id
                },
                new Client
                {
                    Id = Guid.Parse("9357df4f-ac78-4348-a083-daba51f82a7d"),
                    FirstName = "Jane",
                    LastName = "Smith",
                    Birthdate = new DateTime(1998, 5, 10),
                    Phone = "222222222",
                    DateCreated = now,
                    UserId = u2.Id
                },
                new Client
                {
                    Id = Guid.Parse("75b036d0-d866-4f60-b570-a2a4904818d5"),
                    FirstName = "Alex",
                    LastName = "Brown",
                    Birthdate = new DateTime(1995, 3, 15),
                    Phone = "333333333",
                    DateCreated = now,
                    UserId = u3.Id
                },
                new Client
                {
                    Id = Guid.Parse("05c848bc-5c64-4597-9b8a-2639770b92e3"),
                    FirstName = "Emily",
                    LastName = "White",
                    Birthdate = new DateTime(2001, 7, 20),
                    Phone = "444444444",
                    DateCreated = now,
                    UserId = u4.Id
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
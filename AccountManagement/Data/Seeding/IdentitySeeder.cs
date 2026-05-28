using AccountManagement.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace AccountManagement.Data.Seeding
{
    public class IdentitySeeder
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IdentitySeeder(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task SeedUsersAsync()
        {
            var users = new List<(string username, string email)>
            {
                ("client1", "client1@test.com"),
                ("client2", "client2@test.com"),
                ("client3", "client3@test.com"),
                ("client4", "client4@test.com")
            };

            foreach (var (username, email) in users)
            {
                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser != null) continue;

                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "StrongPassword123!");

                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {email}");
                }

                // IMPORTANT FIX: assign role
                await userManager.AddToRoleAsync(user, "Client");
            }
        }
    }
}
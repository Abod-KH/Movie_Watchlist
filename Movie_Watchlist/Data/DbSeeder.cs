using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Movie_Watchlist.Models;
using Movie_Watchlist.Repositories;
using System;
namespace Movie_Watchlist.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var accountRepo = service.GetRequiredService<IAccountRepository>();

            var adminEmail = "admin@watchlist.com";
            var existingAdmin = await accountRepo.GetUserByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {

                var adminUser = new User
                {
                    Username = "Admin",
                    Email = adminEmail,

                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsActive = true
                };


                int adminId = await accountRepo.CreateUserAsync(adminUser);

                if (adminId > 0)
                {

                    await accountRepo.AddUserToRoleAsync(adminId, "Admin");
                }
            }
        }
    }
}
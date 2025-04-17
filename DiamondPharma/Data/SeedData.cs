using DiamondPharma.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DiamondPharma.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Seed Medicines if not exists
                if (!context.Medicines.Any())
                {
                    context.Medicines.AddRange(
                        new Medicine { Name = "Paracetamol", Description = "Pain reliever", Manufacturer = "Diamond", Price = 2.5M, Stock = 100 },
                        new Medicine { Name = "Ibuprofen", Description = "Anti-inflammatory", Manufacturer = "Diamond", Price = 3.0M, Stock = 80 }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // Seed admin user
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string adminEmail = "admin@diamondpharma.sy";
            string adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                // Optionally, assign to roles here if roles are set up
            }
        }
    }
}

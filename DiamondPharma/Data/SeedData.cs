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
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string adminEmail = "admin@diamondpharma.sy";
            string adminPassword = "Admin@123";
            string adminRole = "Admin";

            // Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
            }

            // Assign Admin role to admin user if not already assigned
            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}

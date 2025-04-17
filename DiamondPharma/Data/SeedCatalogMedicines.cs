using DiamondPharma.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DiamondPharma.Data
{
    public static class SeedCatalogMedicines
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!context.CatalogMedicines.Any())
                {
                    context.CatalogMedicines.AddRange(
                        new CatalogMedicine { Name = "Paracetamol", Description = "Pain reliever and fever reducer", Price = 2.5m, Stock = 500, IsActive = true },
                        new CatalogMedicine { Name = "Amoxicillin", Description = "Antibiotic for bacterial infections", Price = 8.0m, Stock = 200, IsActive = true },
                        new CatalogMedicine { Name = "Ibuprofen", Description = "Anti-inflammatory painkiller", Price = 3.0m, Stock = 300, IsActive = true },
                        new CatalogMedicine { Name = "Metformin", Description = "Diabetes medication", Price = 5.5m, Stock = 150, IsActive = true }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}

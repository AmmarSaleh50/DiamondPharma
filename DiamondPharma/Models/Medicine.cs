using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DiamondPharma.Models
{
    public class Medicine
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public int PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

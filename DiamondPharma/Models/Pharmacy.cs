using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DiamondPharma.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public bool IsApproved { get; set; } = false;

        public ICollection<Order> Orders { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DiamondPharma.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }

    public enum OrderStatus
    {
        Pending,
        Approved,
        Shipped,
        Delivered,
        Cancelled
    }
}

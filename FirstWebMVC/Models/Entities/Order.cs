using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstWebMVC.Models.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        // Khóa ngoại trỏ về Customer
        [Required]
        public int CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        // Navigation Property: 1 Đơn hàng -> Có nhiều Chi tiết đơn hàng
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
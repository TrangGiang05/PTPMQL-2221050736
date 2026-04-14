using System;
using System.Collections.Generic;

namespace FirstWebMVC.Models
{
    // Class này gom dữ liệu của Khách hàng và các Đơn hàng lại với nhau
    public class CustomerOrderHistoryViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        
        // Danh sách các đơn hàng đã mua
        public List<OrderSummary> Orders { get; set; } = new List<OrderSummary>();
    }

    // Class phụ trợ để hiển thị tóm tắt từng đơn hàng
    public class OrderSummary
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; } // Tổng tiền đơn hàng
        public List<string> ProductNames { get; set; } = new List<string>(); // Tên các món đã mua
    }
}
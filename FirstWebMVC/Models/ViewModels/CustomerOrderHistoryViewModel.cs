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

    // MỚI: Class đại diện cho từng dòng sản phẩm trong đơn hàng
    public class OrderItemDetail
    {
        public int OrderDetailId { get; set; } // Thêm ID để xóa được
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    // Class phụ trợ để hiển thị tóm tắt từng đơn hàng
    public class OrderSummary
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; } // Tổng tiền đơn hàng
        
        // ĐÃ SỬA: Thay List<string> bằng List<OrderItemDetail> để lưu cả Tên và Số lượng
        public List<OrderItemDetail> OrderItems { get; set; } = new List<OrderItemDetail>();
    }
}
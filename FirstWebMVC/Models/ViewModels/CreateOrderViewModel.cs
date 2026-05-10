using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Khách hàng")]
        public int CustomerId { get; set; }

        // Danh sách các sản phẩm trong đơn hàng
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    // Class đại diện cho 1 dòng sản phẩm trong đơn hàng
    public class OrderItemViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Sản phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 trở lên")]
        public int Quantity { get; set; }
    }
}
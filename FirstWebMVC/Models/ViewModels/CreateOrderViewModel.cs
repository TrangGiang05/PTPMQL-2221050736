using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn Khách hàng")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Sản phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 trở lên")]
        public int Quantity { get; set; }
    }
}
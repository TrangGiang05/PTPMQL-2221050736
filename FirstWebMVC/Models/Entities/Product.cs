using System.ComponentModel.DataAnnotations;

namespace FirstWebMVC.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không được âm")]
        public decimal Price { get; set; }

        // Navigation Property: Thể hiện quan hệ 1 Sản phẩm -> Nằm trong nhiều Chi tiết đơn hàng
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
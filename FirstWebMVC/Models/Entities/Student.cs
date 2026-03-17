using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstWebMVC.Models.Entities
{
    // Báo cho Entity Framework biết sẽ tạo bảng tên là "Students" trong CSDL
    [Table("Students")]
    public class Student
    {
        // Đánh dấu StudentCode là Khóa chính (Primary Key) của bảng
        [Key]
        [Required(ErrorMessage = "Mã sinh viên không được để trống!")]
        public string StudentCode { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 50 ký tự!")]
        public string FullName { get; set; }

        // Bổ sung thuộc tính Tuổi để dùng Range
        [Required(ErrorMessage = "Vui lòng nhập tuổi!")]
        [Range(18, 200, ErrorMessage = "Yêu cầu trên 18 tuổi!")]
        public int Age { get; set; }

        // Bổ sung thuộc tính Email để dùng EmailAddress
        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng (VD: ten@gmail.com)!")]
        public string Email { get; set; }
    }
}
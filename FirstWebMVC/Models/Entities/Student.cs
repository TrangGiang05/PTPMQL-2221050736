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
        public string StudentCode { get; set; }
        
        public string FullName { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Models.Entities; // Thay thế bằng đường dẫn tới thư mục chứa Model của bạn nếu khác

namespace FirstWebMVC.Data
{
    // Kế thừa từ DbContext của Entity Framework Core
    public class ApplicationDbContext : DbContext
    {
        // Constructor nhận cấu hình (options) từ Program.cs truyền vào
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng trong CSDL ở đây (Ví dụ với bảng Student)
        public DbSet<Student> Students { get; set; }
    }
}

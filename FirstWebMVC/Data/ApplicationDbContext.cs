using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Models.Entities; // Đảm bảo namespace này trỏ đúng đến nơi chứa Customer, Order...

namespace FirstWebMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }

        // THÊM 4 DÒNG MỚI CHO BÀI THỰC HÀNH BUỔI 9:
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using FirstWebMVC.Models; // Gọi namespace chứa ViewModel
using System.Linq;

namespace FirstWebMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Tiêm DbContext vào Controller (Dependency Injection)
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. Chức năng READ: Hiển thị danh sách khách hàng ---
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        // GET: Customer/Create
        // Action này chỉ có nhiệm vụ trả về cái View chứa cái Form trắng để bạn điền
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // Action này nhận dữ liệu từ Form gửi lên (đối tượng customer)
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật: chống tấn công giả mạo yêu cầu
        public IActionResult Create(Customer customer)
        {
            // Kiểm tra xem dữ liệu có khớp với các ràng buộc [Required], [Phone] ở Model không
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer); // Thêm vào bộ nhớ đệm
                _context.SaveChanges();           // Lưu thật sự vào file App.db
                return RedirectToAction(nameof(Index)); // Lưu xong thì quay về trang danh sách
            }
            
            // Nếu dữ liệu sai (ví dụ quên nhập tên), trả về lại Form kèm thông báo lỗi
            return View(customer);
        }

        // --- 2. Chức năng XEM CHI TIẾT ĐƠN HÀNG (Yêu cầu khó nhất) ---
        public IActionResult Details(int id)
        {
            // Bước 2.1: Truy vấn CSDL, dùng Include để nối 4 bảng lại với nhau
            var customerWithOrders = _context.Customers
                .Include(c => c.Orders)                     // Nối Khách hàng -> Đơn hàng
                    .ThenInclude(o => o.OrderDetails)       // Nối Đơn hàng -> Chi tiết đơn hàng
                        .ThenInclude(od => od.Product)      // Nối Chi tiết Đơn hàng -> Sản phẩm
                .FirstOrDefault(c => c.Id == id); // Tìm đúng khách hàng có Id được truyền vào

            // Nếu không tìm thấy khách hàng
            if (customerWithOrders == null)
            {
                return NotFound();
            }

            // Bước 2.2: Chuyển dữ liệu từ Entity (CSDL) sang ViewModel (để ném ra View HTML)
            var viewModel = new CustomerOrderHistoryViewModel
            {
                CustomerId = customerWithOrders.Id,
                CustomerName = customerWithOrders.Name,
                CustomerPhone = customerWithOrders.Phone,
                Orders = customerWithOrders.Orders.Select(o => new OrderSummary
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    // Tính tổng tiền = Tổng của (Số lượng * Đơn giá) trong chi tiết đơn hàng
                    TotalAmount = o.OrderDetails.Sum(od => od.Quantity * od.UnitPrice),
                    // Lấy ra danh sách tên sản phẩm
                    ProductNames = o.OrderDetails.Select(od => od.Product.Name).ToList()
                }).ToList()
            };

            // Ném ViewModel ra View
            return View(viewModel);
        }
    }
}
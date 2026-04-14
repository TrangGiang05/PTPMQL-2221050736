using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using FirstWebMVC.Models;
using System;
using System.Linq;

namespace FirstWebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hiển thị form chọn hàng
        public IActionResult Create()
        {
            // Lấy danh sách Khách hàng và Sản phẩm từ Database để nhét vào thẻ <select>
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name");
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            
            return View();
        }

        // POST: Xử lý khi bấm nút "Chốt Đơn"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Tìm giá sản phẩm hiện tại trong kho
                var product = _context.Products.Find(model.ProductId);
                if (product == null) return NotFound("Sản phẩm không tồn tại!");

                // 2. Tạo VỎ ĐƠN HÀNG (Bảng Order)
                var newOrder = new Order
                {
                    CustomerId = model.CustomerId,
                    OrderDate = DateTime.Now // Tự động lấy giờ hiện tại
                };
                _context.Orders.Add(newOrder);
                _context.SaveChanges(); // LƯU LẦN 1: Để Database cấp cho newOrder một cái Id (Mã đơn)

                // 3. Tạo RUỘT ĐƠN HÀNG (Bảng OrderDetail)
                var orderDetail = new OrderDetail
                {
                    OrderId = newOrder.Id, // Dùng ID vừa được cấp ở trên
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    UnitPrice = product.Price // Lưu cứng lại giá tiền tại thời điểm mua
                };
                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges(); // LƯU LẦN 2: Hoàn tất giao dịch

                // 4. Mua xong thì chuyển hướng về trang Chi tiết của khách hàng đó để xem thành quả
                return RedirectToAction("Details", "Customer", new { id = model.CustomerId });
            }

            // Nếu điền thiếu form, nạp lại dữ liệu cho Dropdown và báo lỗi
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name", model.CustomerId);
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name", model.ProductId);
            return View(model);
        }
    }
}
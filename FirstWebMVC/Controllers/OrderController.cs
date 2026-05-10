using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using FirstWebMVC.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirstWebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- DANH SÁCH TẤT CẢ ĐƠN HÀNG ---
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            
            return View(orders);
        }

        // GET: Hiển thị form chọn hàng
        public IActionResult Create()
        {
            // Lấy danh sách Khách hàng và Sản phẩm từ Database
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name");
            
            // Lấy danh sách sản phẩm kèm giá hiện tại
            var products = _context.Products.Select(p => new 
            {
                p.Id,
                p.Name,
                Price = p.Price.ToString("N0") // Format giá hiển thị
            }).ToList();
            
            ViewBag.Products = new SelectList(products, "Id", "Name");
            ViewBag.ProductPrices = System.Text.Json.JsonSerializer.Serialize(
                _context.Products.ToDictionary(p => p.Id, p => p.Price)
            );
            
            var model = new CreateOrderViewModel();
            return View(model);
        }

        // POST: Xử lý khi bấm nút "Chốt Đơn"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra khách hàng tồn tại
                var customer = _context.Customers.Find(model.CustomerId);
                if (customer == null) 
                    return NotFound("Khách hàng không tồn tại!");

                // Kiểm tra có sản phẩm nào được chọn không
                if (model.OrderItems == null || model.OrderItems.Count == 0)
                {
                    ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 sản phẩm!");
                    ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name", model.CustomerId);
                    ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
                    return View(model);
                }

                // 1. Tạo VỎ ĐƠN HÀNG (Bảng Order)
                var newOrder = new Order
                {
                    CustomerId = model.CustomerId,
                    OrderDate = DateTime.Now
                };
                _context.Orders.Add(newOrder);
                _context.SaveChanges(); // LƯU LẦN 1: Để Database cấp Id cho newOrder

                // 2. Tạo RUỘT ĐƠN HÀNG (Bảng OrderDetail) - có thể nhiều sản phẩm
                foreach (var item in model.OrderItems)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product == null) 
                    {
                        ModelState.AddModelError("", $"Sản phẩm ID {item.ProductId} không tồn tại!");
                        continue;
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderId = newOrder.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price // Lưu giá tại thời điểm mua
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                _context.SaveChanges(); // LƯU LẦN 2: Hoàn tất giao dịch

                // 3. Chuyển hướng về trang Chi tiết khách hàng
                return RedirectToAction("Details", "Customer", new { id = model.CustomerId });
            }

            // Nếu có lỗi, nạp lại dữ liệu
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name", model.CustomerId);
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            return View(model);
        }

        // --- XEM CHI TIẾT ĐƠN HÀNG ---
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại!");
            }

            return View(order);
        }

        // --- EDIT: Hiển thị form sửa đơn hàng ---
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại!");
            }

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            return View(order);
        }

        // --- EDIT: Xử lý cập nhật ngày mua và khách hàng ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order model)
        {
            if (id != model.Id)
            {
                return BadRequest("ID không khớp!");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại!");
            }

            try
            {
                order.CustomerId = model.CustomerId;
                order.OrderDate = model.OrderDate;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name", model.CustomerId);
                return View(model);
            }
        }

        // --- DELETE: Xác nhận xóa đơn hàng ---
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại!");
            }

            return View(order);
        }

        // --- DELETE: Thực hiện xóa đơn hàng ---
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại!");
            }

            try
            {
                // Xóa các OrderDetail trước
                _context.OrderDetails.RemoveRange(order.OrderDetails);
                
                // Xóa Order
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa: " + ex.Message);
            }
        }

        // --- XÓA CHI TIẾT ĐƠN HÀNG (OrderDetail) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOrderDetail(int orderDetailId, int customerId)
        {
            // Tìm chi tiết đơn hàng cần xóa
            var orderDetail = _context.OrderDetails.FirstOrDefault(od => od.Id == orderDetailId);
            
            if (orderDetail == null)
            {
                return NotFound("Chi tiết đơn hàng không tồn tại!");
            }

            try
            {
                // Xóa OrderDetail
                _context.OrderDetails.Remove(orderDetail);
                _context.SaveChanges();
                
                // Kiểm tra xem Order còn OrderDetail khác không
                var order = _context.Orders.FirstOrDefault(o => o.Id == orderDetail.OrderId);
                if (order != null)
                {
                    var remainingItems = _context.OrderDetails.Where(od => od.OrderId == order.Id).Count();
                    
                    // Nếu đơn hàng không còn sản phẩm nào, xóa luôn đơn hàng
                    if (remainingItems == 0)
                    {
                        _context.Orders.Remove(order);
                        _context.SaveChanges();
                    }
                }
                
                return RedirectToAction("Details", "Customer", new { id = customerId });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi xóa: " + ex.Message);
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FirstWebMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hiển thị danh sách sản phẩm
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        /// <summary>
        /// Hiển thị form tạo mới sản phẩm
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Xử lý tạo mới sản phẩm
        /// </summary>
        /// <param name="product">Thông tin sản phẩm cần tạo</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        /// <summary>
        /// Hiển thị chi tiết một sản phẩm
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xem</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        /// <summary>
        /// Hiển thị form chỉnh sửa sản phẩm
        /// </summary>
        /// <param name="id">ID của sản phẩm cần chỉnh sửa</param>
        // GET: Hiển thị form chứa thông tin cũ
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        /// <summary>
        /// Xử lý chỉnh sửa sản phẩm
        /// </summary>
        /// <param name="id">ID của sản phẩm</param>
        /// <param name="product">Thông tin sản phẩm được chỉnh sửa</param>
        // POST: Xử lý lưu thông tin mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _context.Products.AnyAsync(p => p.Id == product.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dữ liệu đã được thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }
            return View(product);
        }

        /// <summary>
        /// Hiển thị trang xác nhận xóa sản phẩm
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xóa</param>
        // GET: Hiển thị trang hỏi "Bạn có chắc muốn xóa không?"
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        /// <summary>
        /// Xử lý xóa sản phẩm
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xóa</param>
        // POST: Nút xác nhận xóa (Thực hiện xóa thật)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm này vì nó đang được sử dụng. " + 
                    (ex.InnerException?.Message ?? "");
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }
    }
}
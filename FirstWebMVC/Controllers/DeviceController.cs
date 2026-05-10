using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;

namespace FirstWebMVC.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hiển thị danh sách các thiết bị với tính năng tìm kiếm
        /// </summary>
        /// <param name="searchString">Từ khóa tìm kiếm theo tên thiết bị</param>
        // GET: Device
        public async Task<IActionResult> Index(string searchString)
        {
            // Lưu lại từ khóa trên ô input
            ViewData["CurrentFilter"] = searchString;

            // Lấy danh sách thiết bị kèm theo thông tin Loại thiết bị (Category)
            var applicationDbContext = _context.Devices.Include(d => d.Category).AsQueryable();

            // Nếu có từ khóa thì lọc theo DeviceName (với null check)
            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(d => 
                    d.DeviceName != null && d.DeviceName.Contains(searchString));
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Device/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Device/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }

        /// <summary>
        /// Xử lý tạo mới thiết bị
        /// </summary>
        /// <param name="device">Thông tin thiết bị cần tạo</param>
        // POST: Device/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DeviceName,CategoryId")] Device device)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra CategoryId có tồn tại không (FK validation)
                var category = await _context.Categories.FindAsync(device.CategoryId);
                if (category == null)
                {
                    ModelState.AddModelError("CategoryId", "Loại thiết bị không tồn tại!");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", device.CategoryId);
                    return View(device);
                }

                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", device.CategoryId);
            return View(device);
        }

        // GET: Device/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", device.CategoryId);
            return View(device);
        }

        /// <summary>
        /// Xử lý chỉnh sửa thông tin thiết bị
        /// </summary>
        /// <param name="id">ID của thiết bị</param>
        /// <param name="device">Thông tin thiết bị được chỉnh sửa</param>
        // POST: Device/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DeviceName,CategoryId")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra CategoryId có tồn tại không (FK validation)
                var category = await _context.Categories.FindAsync(device.CategoryId);
                if (category == null)
                {
                    ModelState.AddModelError("CategoryId", "Loại thiết bị không tồn tại!");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", device.CategoryId);
                    return View(device);
                }

                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.Id))
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
                    // Lỗi từ Database (ví dụ: Vi phạm constraint)
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.InnerException?.Message);
                }
                catch (Exception ex)
                {
                    // Lỗi không mong đợi
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }

                if (ModelState.IsValid)
                    return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", device.CategoryId);
            return View(device);
        }

        // GET: Device/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        /// <summary>
        /// Xử lý xóa thiết bị
        /// </summary>
        /// <param name="id">ID của thiết bị cần xóa</param>
        // POST: Device/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var device = await _context.Devices.FindAsync(id);
                if (device != null)
                {
                    _context.Devices.Remove(device);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Lỗi này thường xảy ra khi có FK constraint (thiết bị đang được dùng)
                TempData["ErrorMessage"] = "Không thể xóa thiết bị này vì nó đang được sử dụng. " + 
                    (ex.InnerException?.Message ?? "");
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.Id == id);
        }
    }
}

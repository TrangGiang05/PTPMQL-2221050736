using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using System.Threading.Tasks;
using System.Linq;
using FirstWebMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirstWebMVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- YÊU CẦU 2: HIỂN THỊ DỮ LIỆU ---
        public async Task<IActionResult> Index()
        {
            var danhSachSinhVien = await _context.Students
                .Include(s => s.Faculty) // Kéo theo dữ liệu từ bảng Faculty
                .Select(s => new StudentFacultyViewModel // Nhặt dữ liệu đưa vào ViewModel
                {
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    FacultyName = s.Faculty != null ? s.Faculty.FacultyName : "Chưa có khoa"
                })
                .ToListAsync(); 

            return View(danhSachSinhVien);
        }

        // --- YÊU CẦU 3: THÊM MỚI DỮ LIỆU (CREATE) ---
        [HttpGet]
        public IActionResult Create()
        {
            var danhSachKhoa = _context.Faculties.ToList();
            ViewBag.FacultyList = new SelectList(danhSachKhoa, "FacultyID", "FacultyName");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            ModelState.Remove("Faculty");

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.FacultyList = new SelectList(_context.Faculties.ToList(), "FacultyID", "FacultyName", student.FacultyID);
            return View(student);
        }

        // --- YÊU CẦU 4: CHỈNH SỬA DỮ LIỆU (EDIT) ---
        // 1. Action GET: Lấy dữ liệu cũ và danh sách khoa lên Form
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound("NotFound");
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("NotFound");
            }
            
            // Lấy danh sách khoa và chọn sẵn khoa hiện tại của sinh viên
            var danhSachKhoa = _context.Faculties.ToList();
            ViewBag.FacultyList = new SelectList(danhSachKhoa, "FacultyID", "FacultyName", student.FacultyID);

            return View(student);
        }

        // 2. Action POST: Lưu thông tin thay đổi vào CSDL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Student student)
        {
            if (id != student.StudentCode)
            {
                return NotFound("NotFound");
            }

            // Gỡ bỏ kiểm tra ràng buộc thuộc tính Faculty ngầm
            ModelState.Remove("Faculty");

            if (ModelState.IsValid)
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Nếu có lỗi, nạp lại danh sách khoa để hiển thị lại Form
            ViewBag.FacultyList = new SelectList(_context.Faculties.ToList(), "FacultyID", "FacultyName", student.FacultyID);
            return View(student);
        }

        // --- YÊU CẦU 5: XÓA DỮ LIỆU (DELETE) ---
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound("NotFound");
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("NotFound");
            }

            return View(student); 
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student); 
                await _context.SaveChangesAsync(); 
            }
            return RedirectToAction(nameof(Index)); 
        }
    }
}
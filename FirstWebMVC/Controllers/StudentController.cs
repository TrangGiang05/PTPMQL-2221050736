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
                    // Xử lý trường hợp sinh viên chưa được gán FacultyID thì báo "Chưa có khoa" để không bị lỗi ứng dụng
                    FacultyName = s.Faculty != null ? s.Faculty.FacultyName : "Chưa có khoa"
                })
                .ToListAsync(); // Dùng ToListAsync() vì hàm đang là async Task

            return View(danhSachSinhVien);
        }

        // --- YÊU CẦU 3: THÊM MỚI DỮ LIỆU (CREATE) ---
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách các Khoa từ Database
            var danhSachKhoa = _context.Faculties.ToList();
            
            // Gói vào SelectList và gửi qua ViewBag
            ViewBag.FacultyList = new SelectList(danhSachKhoa, "FacultyID", "FacultyName");
            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            // 1. Gỡ bỏ đòi hỏi kiểm tra thuộc tính ngầm Faculty (Vì trên form mình chỉ có FacultyID)
            ModelState.Remove("Faculty");

            // 2. Kiểm tra lại xem dữ liệu đã hợp lệ hết chưa
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Lưu thành công thì về trang danh sách
            }
            
            // 3. NẾU BỊ LỖI (chưa hợp lệ): Phải nạp lại danh sách Khoa để đẩy ra màn hình
            var danhSachKhoa = _context.Faculties.ToList();
            // Lần này ta truyền thêm tham số student.FacultyID để nó giữ nguyên cái Khoa mà bạn vừa chọn hụt
            ViewBag.FacultyList = new SelectList(danhSachKhoa, "FacultyID", "FacultyName", student.FacultyID);
            
            // Trả lại View kèm theo dữ liệu vừa nhập
            return View(student);
        }

        // 1. Action GET: Lấy dữ liệu của bản ghi muốn sửa => trả dữ liệu về View
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound("NotFound"); // Báo lỗi nếu không có mã sinh viên
            }

            // Tìm sinh viên trong CSDL dựa vào khóa chính (StudentCode)
            var student = await _context.Students.FindAsync(id);
            
            if (student == null)
            {
                return NotFound("NotFound"); // Báo lỗi nếu không tìm thấy sinh viên trong CSDL
            }
            
            // Trả dữ liệu của sinh viên đó về View Edit để hiển thị lên Form
            return View(student);
        }

        // 3. Nhận dữ liệu từ view gửi lên và tiến hành lưu vào CSDL (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Student student)
        {
            // Kiểm tra xem mã sinh viên trên đường dẫn (id) và trong Form (student.StudentCode) có khớp nhau không
            if (id != student.StudentCode)
            {
                return NotFound("NotFound");
            }

            // Kiểm tra tính hợp lệ của dữ liệu
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin mới vào DbContext
                _context.Update(student);
                
                // Lưu thay đổi vào CSDL (file App.db)
                await _context.SaveChangesAsync();
                
                // Lưu xong thì tự động quay về trang danh sách (Index)
                return RedirectToAction(nameof(Index));
            }
            
            // Nếu dữ liệu lỗi, hiển thị lại Form với thông tin vừa nhập
            return View(student);
        }

        // 1. Action GET: Lấy dữ liệu của bản ghi muốn xoá => trả dữ liệu về View
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound("NotFound"); // Báo lỗi nếu không nhận được mã sinh viên
            }

            // Tìm bản ghi trong CSDL
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("NotFound");
            }

            // Trả dữ liệu về View để hiển thị form xác nhận
            return View(student); 
        }

        // 3. Submit for delete => Xoá bỏ khỏi dbContext => Lưu thay đổi vào CSDL
        // Lưu ý: Đặt tên hàm là DeleteConfirmed nhưng vẫn map với action "Delete" trên Form
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Tìm lại bản ghi cần xóa
            var student = await _context.Students.FindAsync(id);
            
            if (student != null)
            {
                // Xóa bỏ khỏi dbContext
                _context.Students.Remove(student); 
                
                // Lưu thay đổi vào CSDL (App.db)
                await _context.SaveChangesAsync(); 
            }
            
            // Xóa xong thì quay về trang danh sách
            return RedirectToAction(nameof(Index)); 
        }
    }
}
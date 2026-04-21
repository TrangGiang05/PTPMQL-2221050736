using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using System.Threading.Tasks;
using System.Linq;
using FirstWebMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using System.IO;

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
                _context.Add(student); //cau lenh nay use entities framwork de quan ly trang thai cua du lieu
                await _context.SaveChangesAsync(); // luu thay doi vao trong csdl
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.FacultyList = new SelectList(_context.Faculties.ToList(), "FacultyID", "FacultyName", student.FacultyID);
            return View(student);
        }

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

        // --- ĐỌC DỮ LIỆU TỪ EXCEL VÀ LƯU VÀO CSDL ---
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile fileExcel)
        {
            // 1. Kiểm tra xem người dùng có chọn file chưa
            if (fileExcel == null || fileExcel.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một file Excel!";
                return RedirectToAction(nameof(Index));
            }

            var studentsList = new List<Student>();

            try
            {
                // 2. Mở file Excel ra đọc
                using (var stream = new MemoryStream())
                {
                    await fileExcel.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        // Lấy Sheet đầu tiên
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RangeUsed().RowsUsed();

                        // 3. Đọc từ dòng 2 (Skip dòng 1 vì là dòng Tiêu đề)
                        foreach (var row in rows.Skip(1))
                        {
                            var newStudent = new Student
                            {
                                // Đối chiếu đúng 5 cột trong file Template Excel
                                StudentCode = row.Cell(1).Value.ToString().Trim(),
                                FullName = row.Cell(2).Value.ToString().Trim(),
                                Age = int.Parse(row.Cell(3).Value.ToString().Trim()),
                                Email = row.Cell(4).Value.ToString().Trim(),
                                FacultyID = row.Cell(5).Value.ToString().Trim()
                            };

                            studentsList.Add(newStudent);
                        }
                    }
                }

                // 4. Lưu toàn bộ vào Database
                if (studentsList.Any())
                {
                    _context.Students.AddRange(studentsList);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = $"Import thành công {studentsList.Count} sinh viên!";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi đọc file. Vui lòng kiểm tra lại định dạng file Excel mẫu! Chi tiết: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
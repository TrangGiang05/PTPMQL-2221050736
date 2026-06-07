using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using System;
using System.Collections.Generic;
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

        // --- BƯỚC 1: SỬA LẠI INDEX CHO AJAX ---
        public IActionResult Index()
        {
            // Trả về view rỗng, dữ liệu sẽ được load ngầm qua hàm GetStudents bên dưới
            return View();
        }

        // --- BƯỚC 2: HÀM CUNG CẤP DỮ LIỆU CHO AJAX (READ) ---
        public async Task<IActionResult> GetStudents(int page = 1, int pageSize = 10)
        {
            // Dùng lại logic lấy StudentFacultyViewModel rất chuẩn của bạn
            var query = _context.Students
                .Include(s => s.Faculty) 
                .AsNoTracking()
                .Select(s => new StudentFacultyViewModel 
                {
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    FacultyName = s.Faculty != null ? s.Faculty.FacultyName : "Chưa có khoa"
                });

            var totalItems = await query.CountAsync();

            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new FirstWebMVC.ViewModels.PagedResult<StudentFacultyViewModel> 
            {
                Items = students,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return PartialView("_StudentTable", result);
        }

        // --- XEM CHI TIẾT SINH VIÊN (DETAILS) ---
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
        
            var student = await _context.Students
                .Include(s => s.Faculty)
                .FirstOrDefaultAsync(s => s.StudentCode == id);
            
            if (student == null)
            {
                return NotFound();
            }
        
            return View(student);
        }

        // --- THÊM MỚI DỮ LIỆU (CREATE) CHO AJAX ---
        [HttpGet]
        public IActionResult Create()
        {
            var danhSachKhoa = _context.Faculties.ToList();
            ViewBag.FacultyList = new SelectList(danhSachKhoa, "FacultyID", "FacultyName");
            // ĐÃ SỬA: Trả về PartialView để nhúng vào Modal
            return PartialView("_Create", new Student());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            ModelState.Remove("Faculty");

            if (ModelState.IsValid)
            {
                var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == student.StudentCode);
                if (existingStudent != null)
                {
                    ModelState.AddModelError("StudentCode", "Mã sinh viên này đã tồn tại trong hệ thống!");
                    ViewBag.FacultyList = new SelectList(_context.Faculties.ToList(), "FacultyID", "FacultyName", student.FacultyID);
                    // ĐÃ SỬA: Báo lỗi thẳng trên PartialView
                    return PartialView("_Create", student);
                }

                _context.Add(student); 
                await _context.SaveChangesAsync(); 
                // ĐÃ SỬA: Trả về cục Json báo thành công để Javascript đóng Modal
                return Json(new { success = true });
            }
            
            ViewBag.FacultyList = new SelectList(_context.Faculties.ToList(), "FacultyID", "FacultyName", student.FacultyID);
            // ĐÃ SỬA: Báo lỗi validation thẳng trên PartialView
            return PartialView("_Create", student);
        }

        // --- SỬA DỮ LIỆU (EDIT) CHO AJAX ---
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound("NotFound");

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound("NotFound");
            
            var danhSachKhoa = _context.Faculties.ToList();
            ViewBag.FacultyList = new SelectList(danhSachKhoa, "FacultyID", "FacultyName", student.FacultyID);

            // ĐÃ SỬA: Trả về PartialView thay vì View
            return PartialView("_Edit", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (student?.StudentCode == null) return Json(new { success = false, message = "Mã sinh viên không hợp lệ" });

            ModelState.Remove("Faculty");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { success = false, message = "Dữ liệu đã bị thay đổi, vui lòng thử lại." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }
            
            var errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return Json(new { success = false, message = "Validation failed: " + errors });
        }

        // --- XÓA DỮ LIỆU (DELETE) CHO AJAX ---
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound("NotFound");

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound("NotFound");

            // ĐÃ SỬA: Trả về PartialView thay vì View
            return PartialView("_Delete", student); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Student student)
        {
            if (student?.StudentCode == null) return NotFound("NotFound");

            try
            {
                var studentToDelete = await _context.Students.FindAsync(student.StudentCode);
                if (studentToDelete != null)
                {
                    _context.Students.Remove(studentToDelete); 
                    await _context.SaveChangesAsync();
                    // ĐÃ SỬA: Thành công thì trả về JSON
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Sinh viên không tồn tại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // --- ĐỌC DỮ LIỆU TỪ EXCEL VÀ LƯU VÀO CSDL ---
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile fileExcel)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một file Excel!";
                return RedirectToAction(nameof(Index));
            }

            var studentsList = new List<Student>();
            var duplicateCount = 0;
            var errorRows = new List<string>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await fileExcel.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RangeUsed().RowsUsed();

                        foreach (var row in rows.Skip(1))
                        {
                            try
                            {
                                var studentCode = row.Cell(1).Value.ToString().Trim();

                                var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);
                                if (existingStudent != null)
                                {
                                    duplicateCount++;
                                    errorRows.Add($"Dòng {row.RowNumber()}: Mã SV '{studentCode}' đã tồn tại");
                                    continue;
                                }

                                var newStudent = new Student
                                {
                                    StudentCode = studentCode,
                                    FullName = row.Cell(2).Value.ToString().Trim(),
                                    Age = int.Parse(row.Cell(3).Value.ToString().Trim()),
                                    Email = row.Cell(4).Value.ToString().Trim(),
                                    FacultyID = row.Cell(5).Value.ToString().Trim()
                                };

                                studentsList.Add(newStudent);
                            }
                            catch (System.Exception ex)
                            {
                                errorRows.Add($"Dòng {row.RowNumber()}: {ex.Message}");
                            }
                        }
                    }
                }

                if (studentsList.Any())
                {
                    _context.Students.AddRange(studentsList);
                    await _context.SaveChangesAsync();
                }

                string message = $"Import thành công {studentsList.Count} sinh viên!";
                if (duplicateCount > 0) message += $" (Bỏ qua {duplicateCount} mã trùng lặp)";
                if (errorRows.Any())
                {
                    message += $" - Lỗi: {string.Join("; ", errorRows.Take(3))}";
                    if (errorRows.Count > 3) message += "...";
                }

                TempData["Success"] = message;
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi đọc file. Vui lòng kiểm tra lại định dạng file Excel mẫu! Chi tiết: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
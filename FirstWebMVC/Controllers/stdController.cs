using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirstWebMVC.Controllers
{
    public class stdController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        public IActionResult Index()
        {
            return View();
        }
    
    public async Task<IActionResult> GetStudents(int page = 1, int pageSize = 10)
        {
            var query = _context.Students
                .AsNoTracking()
                .OrderByDescending(x => x.StudentCode);

            var totalItems = await query.CountAsync();

            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Student>
            {
                Items = students,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return PartialView("_StudentTable", result);
        }

        [HttpGet]
        public IActionResult Create()
        { 
            var falculties = _context.Faculties.ToList();
            ViewBag.Falcultylist = new SelectList(falculties, "FacultyID", "FacultyName");

            return PartialView("_Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
             ModelState.Remove("Faculty");

            if (!ModelState.IsValid)
            {
                var falculties = _context.Faculties.ToList();
                ViewBag.Falcultylist = new SelectList(falculties, "FacultyID", "FacultyName");

                return PartialView("_Create", student);
            }
            
            _context.Students.Add(student);

            await _context.SaveChangesAsync();

            return Json(new{ success = true});
        }
    } 
}
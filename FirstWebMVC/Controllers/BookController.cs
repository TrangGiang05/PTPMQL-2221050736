using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.ViewModels;
namespace FirstWebMVC.Controllers
{
    public class BookController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetBooks(int page = 1, int pageSize = 10)
        {
            var query = _context.Books
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate);

            var totalItems = await query.CountAsync();

            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<Book>
            {
                Items = books,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return PartialView("_BookTable", result);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", book);
            }

            book.CreatedDate = DateTime.Now;

            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return PartialView("_Edit", book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Book book)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", book);
            }

            var existingBook = await _context.Books.FindAsync(book.Id);

            if (existingBook == null)
            {
                return NotFound();
            }

            existingBook.ISBN = book.ISBN;
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Publisher = book.Publisher;
            existingBook.PublishYear = book.PublishYear;
            existingBook.Price = book.Price;
            existingBook.Quantity = book.Quantity;
            existingBook.Category = book.Category;
            existingBook.Description = book.Description;
            existingBook.IsAvailable = book.IsAvailable;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Book book)
        {
            var existingBook = await _context.Books
                .FindAsync(book.Id);

            if (existingBook == null)
            {
                return Json(new
                {
                    success = false
                });
            }

            _context.Books.Remove(existingBook);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }
    }
}
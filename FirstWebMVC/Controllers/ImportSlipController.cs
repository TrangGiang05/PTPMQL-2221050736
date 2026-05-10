using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstWebMVC.Data;
using FirstWebMVC.Models.Entities;
using FirstWebMVC.Models.ViewModels;

namespace FirstWebMVC.Controllers
{
    public class ImportSlipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImportSlipController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. HIỂN THỊ DANH SÁCH PHIẾU NHẬP ---
        public async Task<IActionResult> Index()
        {
            var slips = await _context.ImportSlips.Include(i => i.Supplier).ToListAsync();
            return View(slips);
        }

        // --- 2. HIỂN THỊ MÀN HÌNH LẬP PHIẾU MỚI ---
        public IActionResult Create()
        {
            ViewBag.SupplierId = new SelectList(_context.Suppliers, "Id", "Name");
            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            
            var model = new ImportSlipViewModel();
            model.Details.Add(new ImportSlipDetailViewModel());
            
            return View(model);
        }

        // --- 3. XỬ LÝ LƯU PHIẾU VÀO DATABASE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImportSlipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var importSlip = new ImportSlip
                {
                    ImportDate = model.ImportDate,
                    SupplierId = model.SupplierId
                };
                _context.ImportSlips.Add(importSlip);
                await _context.SaveChangesAsync(); 

                foreach (var item in model.Details)
                {
                    var detail = new ImportSlipDetail
                    {
                        ImportSlipId = importSlip.Id,
                        DeviceId = item.DeviceId,
                        Quantity = item.Quantity,
                        ImportPrice = item.ImportPrice
                    };
                    _context.ImportSlipDetails.Add(detail);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SupplierId = new SelectList(_context.Suppliers, "Id", "Name", model.SupplierId);
            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            return View(model);
        }

        // --- 4. XEM CHI TIẾT 1 PHIẾU NHẬP KHO ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var importSlip = await _context.ImportSlips
                .Include(i => i.Supplier)
                .Include(i => i.ImportSlipDetails)
                    .ThenInclude(d => d.Device)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (importSlip == null) return NotFound();

            return View(importSlip);
        }

        // --- 5. CHỨC NĂNG SỬA PHIẾU (EDIT) ---

        // GET: ImportSlip/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var importSlip = await _context.ImportSlips
                .Include(i => i.ImportSlipDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (importSlip == null) return NotFound();

            // Chuyển đổi dữ liệu từ Entity sang ViewModel
            var model = new ImportSlipViewModel
            {
                Id = importSlip.Id,
                ImportDate = importSlip.ImportDate,
                SupplierId = importSlip.SupplierId,
                Details = importSlip.ImportSlipDetails.Select(d => new ImportSlipDetailViewModel
                {
                    DeviceId = d.DeviceId,
                    Quantity = d.Quantity,
                    ImportPrice = d.ImportPrice
                }).ToList()
            };

            ViewBag.SupplierId = new SelectList(_context.Suppliers, "Id", "Name", importSlip.SupplierId);
            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            return View(model);
        }

        // POST: ImportSlip/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ImportSlipViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSlip = await _context.ImportSlips
                        .Include(i => i.ImportSlipDetails)
                        .FirstOrDefaultAsync(i => i.Id == id);

                    if (existingSlip == null) return NotFound();

                    // Cập nhật thông tin Master
                    existingSlip.ImportDate = model.ImportDate;
                    existingSlip.SupplierId = model.SupplierId;

                    // Xóa chi tiết cũ và thêm lại chi tiết mới từ Form
                    _context.ImportSlipDetails.RemoveRange(existingSlip.ImportSlipDetails);

                    foreach (var item in model.Details)
                    {
                        existingSlip.ImportSlipDetails.Add(new ImportSlipDetail
                        {
                            ImportSlipId = id,
                            DeviceId = item.DeviceId,
                            Quantity = item.Quantity,
                            ImportPrice = item.ImportPrice
                        });
                    }

                    _context.Update(existingSlip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ImportSlips.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SupplierId = new SelectList(_context.Suppliers, "Id", "Name", model.SupplierId);
            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            return View(model);
        }

        // --- 6. CHỨC NĂNG XÓA PHIẾU (DELETE) ---

        // GET: ImportSlip/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var importSlip = await _context.ImportSlips
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (importSlip == null) return NotFound();

            return View(importSlip);
        }

        // POST: ImportSlip/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var importSlip = await _context.ImportSlips.FindAsync(id);
            if (importSlip != null)
            {
                // Note: SQL Server thường được cấu hình Cascade Delete nên sẽ tự xóa luôn Details, 
                // nhưng gọi Remove ở Slip là đủ.
                _context.ImportSlips.Remove(importSlip);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
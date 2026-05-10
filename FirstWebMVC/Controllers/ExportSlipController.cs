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
    public class ExportSlipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExportSlipController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. DANH SÁCH PHIẾU XUẤT ---
        public async Task<IActionResult> Index()
        {
            return View(await _context.ExportSlips.ToListAsync());
        }

        // --- 2. TẠO MỚI PHIẾU XUẤT (GET) ---
        public IActionResult Create()
        {
            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            var model = new ExportSlipViewModel();
            model.Details.Add(new ExportSlipDetailViewModel());
            return View(model);
        }

        // --- 3. TẠO MỚI PHIẾU XUẤT (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExportSlipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var exportSlip = new ExportSlip
                {
                    ExportDate = model.ExportDate,
                    ReceiverName = model.ReceiverName
                };
                _context.ExportSlips.Add(exportSlip);
                await _context.SaveChangesAsync();

                foreach (var item in model.Details)
                {
                    var detail = new ExportSlipDetail
                    {
                        ExportSlipId = exportSlip.Id,
                        DeviceId = item.DeviceId,
                        Quantity = item.Quantity,
                        ExportPrice = item.ExportPrice
                    };
                    _context.ExportSlipDetails.Add(detail);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            return View(model);
        }

        // --- 4. CHI TIẾT PHIẾU XUẤT ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exportSlip = await _context.ExportSlips
                .Include(e => e.ExportSlipDetails)
                    .ThenInclude(d => d.Device)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exportSlip == null) return NotFound();

            return View(exportSlip);
        }

        // --- 5. CHỈNH SỬA PHIẾU XUẤT (EDIT) ---

        // GET: ExportSlip/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exportSlip = await _context.ExportSlips
                .Include(e => e.ExportSlipDetails)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exportSlip == null) return NotFound();

            // Chuyển đổi sang ViewModel để hiện lên form
            var model = new ExportSlipViewModel
            {
                Id = exportSlip.Id,
                ExportDate = exportSlip.ExportDate,
                ReceiverName = exportSlip.ReceiverName,
                Details = exportSlip.ExportSlipDetails.Select(d => new ExportSlipDetailViewModel
                {
                    DeviceId = d.DeviceId,
                    Quantity = d.Quantity,
                    ExportPrice = d.ExportPrice
                }).ToList()
            };

            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            return View(model);
        }

        // POST: ExportSlip/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExportSlipViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSlip = await _context.ExportSlips
                        .Include(e => e.ExportSlipDetails)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existingSlip == null) return NotFound();

                    // Cập nhật thông tin Master
                    existingSlip.ExportDate = model.ExportDate;
                    existingSlip.ReceiverName = model.ReceiverName;

                    // Xử lý Detail: Xóa sạch cái cũ, add cái mới từ form về
                    _context.ExportSlipDetails.RemoveRange(existingSlip.ExportSlipDetails);

                    foreach (var item in model.Details)
                    {
                        existingSlip.ExportSlipDetails.Add(new ExportSlipDetail
                        {
                            ExportSlipId = id,
                            DeviceId = item.DeviceId,
                            Quantity = item.Quantity,
                            ExportPrice = item.ExportPrice
                        });
                    }

                    _context.Update(existingSlip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ExportSlips.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DeviceId = new SelectList(_context.Devices, "Id", "DeviceName");
            return View(model);
        }

        // --- 6. XÓA PHIẾU XUẤT (DELETE) ---

        // GET: ExportSlip/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exportSlip = await _context.ExportSlips
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exportSlip == null) return NotFound();

            return View(exportSlip);
        }

        // POST: ExportSlip/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exportSlip = await _context.ExportSlips.FindAsync(id);
            if (exportSlip != null)
            {
                _context.ExportSlips.Remove(exportSlip);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
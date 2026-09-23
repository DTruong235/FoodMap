using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodMap.Data;
using FoodMap.Models;

namespace FoodMap.Controllers
{
    public class HoadonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoadonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hoadons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hoadon.Include(h => h.Khachhang);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Hoadons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadon
                .Include(h => h.Khachhang)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }

        // GET: Hoadons/Create
        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.Khachhang, "MaKh", "Ten");
            return View();
        }

        // POST: Hoadons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,Ngay,TongTien,MaKh,TrangThai")] Hoadon hoadon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoadon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.Khachhang, "MaKh", "Ten", hoadon.MaKh);
            return View(hoadon);
        }

        // GET: Hoadons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadon.FindAsync(id);
            if (hoadon == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.Khachhang, "MaKh", "Ten", hoadon.MaKh);
            return View(hoadon);
        }

        // POST: Hoadons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHd,Ngay,TongTien,MaKh,TrangThai")] Hoadon hoadon)
        {
            if (id != hoadon.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoadonExists(hoadon.MaHd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.Khachhang, "MaKh", "Ten", hoadon.MaKh);
            return View(hoadon);
        }

        // GET: Hoadons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadon
                .Include(h => h.Khachhang)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }

        // POST: Hoadons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoadon = await _context.Hoadon.FindAsync(id);
            if (hoadon != null)
            {
                _context.Hoadon.Remove(hoadon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoadonExists(int id)
        {
            return _context.Hoadon.Any(e => e.MaHd == id);
        }
    }
}

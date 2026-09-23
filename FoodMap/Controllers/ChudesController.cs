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
    public class ChudesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Chudes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Chude.Include(c => c.Danhmuc);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Chudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chude = await _context.Chude
                .Include(c => c.Danhmuc)
                .FirstOrDefaultAsync(m => m.MaCd == id);
            if (chude == null)
            {
                return NotFound();
            }

            return View(chude);
        }

        // GET: Chudes/Create
        public IActionResult Create()
        {
            ViewData["MaDm"] = new SelectList(_context.Danhmuc, "MaDm", "Ten");
            return View();
        }

        // POST: Chudes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCd,TenCd,MaDm")] Chude chude)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDm"] = new SelectList(_context.Danhmuc, "MaDm", "Ten", chude.MaDm);
            return View(chude);
        }

        // GET: Chudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chude = await _context.Chude.FindAsync(id);
            if (chude == null)
            {
                return NotFound();
            }
            ViewData["MaDm"] = new SelectList(_context.Danhmuc, "MaDm", "Ten", chude.MaDm);
            return View(chude);
        }

        // POST: Chudes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCd,TenCd,MaDm")] Chude chude)
        {
            if (id != chude.MaCd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chude);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChudeExists(chude.MaCd))
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
            ViewData["MaDm"] = new SelectList(_context.Danhmuc, "MaDm", "Ten", chude.MaDm);
            return View(chude);
        }

        // GET: Chudes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chude = await _context.Chude
                .Include(c => c.Danhmuc)
                .FirstOrDefaultAsync(m => m.MaCd == id);
            if (chude == null)
            {
                return NotFound();
            }

            return View(chude);
        }

        // POST: Chudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chude = await _context.Chude.FindAsync(id);
            if (chude != null)
            {
                _context.Chude.Remove(chude);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChudeExists(int id)
        {
            return _context.Chude.Any(e => e.MaCd == id);
        }
    }
}

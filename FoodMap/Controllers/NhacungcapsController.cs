using FoodMap.Data;
using FoodMap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Controllers
{
    public class NhacungcapsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhacungcapsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Nhacungcaps
        // BẢNG BÁO CÁO DANH SÁCH VENDOR DÀNH CHO ADMIN
        public async Task<IActionResult> Index()
        {
            // Kiểm tra phân quyền Admin (UserRole == 2)
            int? role = HttpContext.Session.GetInt32("UserRole");
            if (role == null || role != 2)
            {
                return RedirectToAction("DangNhap", "Khachhangs");
            }

            return View(await _context.Nhacungcap.ToListAsync());
        }

        // GET: Nhacungcaps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhacungcap = await _context.Nhacungcap
                .FirstOrDefaultAsync(m => m.MaNcc == id);
            if (nhacungcap == null)
            {
                return NotFound();
            }

            return View(nhacungcap);
        }

        // GET: Nhacungcaps/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nhacungcaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNcc,TenCongTy,DienThoai,Email,DiaChi,TrangThai")] Nhacungcap nhacungcap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhacungcap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhacungcap);
        }

        // GET: Nhacungcaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhacungcap = await _context.Nhacungcap.FindAsync(id);
            if (nhacungcap == null)
            {
                return NotFound();
            }
            return View(nhacungcap);
        }

        // POST: Nhacungcaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNcc,TenCongTy,DienThoai,Email,DiaChi,TrangThai")] Nhacungcap nhacungcap)
        {
            if (id != nhacungcap.MaNcc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhacungcap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhacungcapExists(nhacungcap.MaNcc))
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
            return View(nhacungcap);
        }

        // GET: Nhacungcaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhacungcap = await _context.Nhacungcap
                .FirstOrDefaultAsync(m => m.MaNcc == id);
            if (nhacungcap == null)
            {
                return NotFound();
            }

            return View(nhacungcap);
        }

        // POST: Nhacungcaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nhacungcap = await _context.Nhacungcap.FindAsync(id);
            if (nhacungcap != null)
            {
                _context.Nhacungcap.Remove(nhacungcap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhacungcapExists(int id)
        {
            return _context.Nhacungcap.Any(e => e.MaNcc == id);
        }
    }
}

using FoodMap.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm n?p d? li?u dùng chung cho Layout Dropdown Menu
        private async Task LoadHeaderData()
        {
            ViewBag.danhmuc = await _context.Danhmuc.ToListAsync();
            ViewBag.chude = await _context.Chude.ToListAsync();
            ViewBag.tinmoi = await _context.Mathang.OrderByDescending(m => m.MaMh).Take(5).ToListAsync();
        }

        // Trang ch?: Hi?n th? t?t c? món ?n / ??c s?n
        public async Task<IActionResult> Index()
        {
            await LoadHeaderData();
            var mathangs = await _context.Mathang.Include(m => m.Chude).Include(m => m.Nhacungcap).ToListAsync();
            return View(mathangs);
        }

        // Xem món ?n theo Ch? ??
        public async Task<IActionResult> Chude(int id)
        {
            await LoadHeaderData();
            var mathangs = await _context.Mathang
                .Where(m => m.MaCd == id)
                .Include(m => m.Chude)
                .Include(m => m.Nhacungcap)
                .ToListAsync();
            return View("Index", mathangs);
        }

        // Xem chi ti?t m?t món ?n / tour
        public async Task<IActionResult> Details(int id)
        {
            await LoadHeaderData();
            var mathang = await _context.Mathang
                .Include(m => m.Chude)
                .Include(m => m.Nhacungcap)
                .FirstOrDefaultAsync(m => m.MaMh == id);

            if (mathang == null) return NotFound();
            return View(mathang);
        }

        // Ch?c n?ng Tìm ki?m theo t? khóa (Tim)
        public async Task<IActionResult> Tim(string tukhoa)
        {
            await LoadHeaderData();
            var mathangs = await _context.Mathang
                .Where(m => m.Ten.Contains(tukhoa) || (m.MoTa != null && m.MoTa.Contains(tukhoa)))
                .Include(m => m.Chude)
                .Include(m => m.Nhacungcap)
                .ToListAsync();

            ViewBag.TuKhoa = tukhoa;
            return View("Index", mathangs);
        }
    }
}
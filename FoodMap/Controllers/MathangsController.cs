using FoodMap.Data;
using FoodMap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Controllers
{

    public class MathangsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MathangsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mathangs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Mathang.Include(m => m.Chude).Include(m => m.Nhacungcap);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Mathangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathang
                .Include(m => m.Chude)
                .Include(m => m.Nhacungcap)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }

            return View(mathang);
        }

        // GET: Mathangs/Create
        public IActionResult Create()
        {
            ViewData["MaCd"] = new SelectList(_context.Chude, "MaCd", "TenCd");
            ViewData["MaNcc"] = new SelectList(_context.Nhacungcap, "MaNcc", "TenCongTy");
            return View();
        }

        // POST: Mathangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMh,Ten,GiaGoc,GiaBan,SoLuong,MoTa,MaCd,MaNcc")] Mathang mathang, IFormFile fHinhAnh)
        {
            if (ModelState.IsValid)
            {
                // Gọi hàm Upload ảnh
                string? fileName = Upload(fHinhAnh);
                if (fileName != null)
                {
                    mathang.HinhAnh = fileName;
                }

                _context.Add(mathang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaCd"] = new SelectList(_context.Chude, "MaCd", "TenCd", mathang.MaCd);
            ViewData["MaNcc"] = new SelectList(_context.Nhacungcap, "MaNcc", "TenCongTy", mathang.MaNcc);
            return View(mathang);
        }

        // GET: Mathangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathang.FindAsync(id);
            if (mathang == null)
            {
                return NotFound();
            }
            ViewData["MaCd"] = new SelectList(_context.Chude, "MaCd", "TenCd", mathang.MaCd);
            ViewData["MaNcc"] = new SelectList(_context.Nhacungcap, "MaNcc", "TenCongTy", mathang.MaNcc);
            return View(mathang);
        }

        // POST: Mathangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaMh,Ten,GiaGoc,GiaBan,SoLuong,MoTa,HinhAnh,MaCd,MaNcc")] Mathang mathang)
        {
            if (id != mathang.MaMh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mathang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MathangExists(mathang.MaMh))
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
            ViewData["MaCd"] = new SelectList(_context.Chude, "MaCd", "TenCd", mathang.MaCd);
            ViewData["MaNcc"] = new SelectList(_context.Nhacungcap, "MaNcc", "TenCongTy", mathang.MaNcc);
            return View(mathang);
        }

        // GET: Mathangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathang
                .Include(m => m.Chude)
                .Include(m => m.Nhacungcap)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }

            return View(mathang);
        }

        // POST: Mathangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mathang = await _context.Mathang.FindAsync(id);
            if (mathang != null)
            {
                _context.Mathang.Remove(mathang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MathangExists(int id)
        {
            return _context.Mathang.Any(e => e.MaMh == id);
        }

        // Phương thức xử lý Upload file ảnh vào wwwroot/images
        public string? Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Tạo tên file duy nhất bằng GUID để tránh trùng lặp
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                // Tạo thư mục wwwroot/images nếu chưa tồn tại
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return fileName;
            }
            return null;
        }
    }
}

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMh,Ten,GiaGoc,GiaBan,MoTa,MaCd,MaNcc")] Mathang mathang, IFormFile? fHinh)
        {


            // LẤY MÃ NHÀ CUNG CẤP TỪ SESSION ĐĂNG NHẬP
            int? maNccSession = HttpContext.Session.GetInt32("MaNcc"); // Hoặc HttpContext.Session.GetInt32("MaNcc")

            // Kiểm tra xem mã NCC trong Session có thực sự tồn tại trong CSDL không
            var nccExist = await _context.Nhacungcap.FirstOrDefaultAsync(n => n.MaNcc == maNccSession);

            if (nccExist != null)
            {
                mathang.MaNcc = nccExist.MaNcc; // Gán đúng khóa ngoại tồn tại trong CSDL
            }
            else
            {
                // NẾU CHƯA CÓ SESSION HOẶC KHÔNG TÌM THẤY NCC: Lấy tạm mã Nhà cung cấp đầu tiên trong CSDL để tránh lỗi
                var nccMacDinh = await _context.Nhacungcap.FirstOrDefaultAsync();
                if (nccMacDinh != null)
                {
                    mathang.MaNcc = nccMacDinh.MaNcc;
                }
                else
                {
                    TempData["ErrorMsg"] = "Hệ thống chưa có Nhà cung cấp nào! Vui lòng tạo Nhà cung cấp trước.";
                    return RedirectToAction("Index", "Vendors");
                }
            }

            if (ModelState.IsValid)
            {
                // 3. Xử lý Upload ảnh
                if (fHinh != null && fHinh.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fHinh.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await fHinh.CopyToAsync(stream);
                    }
                    mathang.HinhAnh = fileName;
                }

                _context.Add(mathang);
                await _context.SaveChangesAsync();
                TempData["SuccessMsg"] = "Đăng bán mặt hàng mới thành công!";

                return RedirectToAction("Index", "Vendors");
            }

            ViewData["MaCd"] = new SelectList(_context.Chude, "MaCd", "Ten", mathang.MaCd);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaMh,Ten,GiaGoc,GiaBan,MoTa,Hinh,MaCd,MaNcc")] Mathang mathang, IFormFile? fHinh)
        {
            if (id != mathang.MaMh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // XỬ LÝ UPLOAD HÌNH ẢNH MỚI NẾU VENDOR CHỌN FILE
                    if (fHinh != null && fHinh.Length > 0)
                    {
                        // Tạo tên file duy nhất bằng Guid để tránh trùng lặp file cũ
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fHinh.FileName);
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                        // Lưu file ảnh mới vào thư mục wwwroot/images/
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await fHinh.CopyToAsync(stream);
                        }

                        // Cập nhật tên file ảnh mới vào thuộc tính Hinh
                        mathang.HinhAnh = fileName;
                    }
                    // Nếu không chọn file mới, mathang.Hinh giữ nguyên tên ảnh cũ từ hidden field

                    _context.Update(mathang);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMsg"] = "Cập nhật thông tin mặt hàng thành công!";
                    return RedirectToAction("Index", "Vendors");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Mathang.Any(e => e.MaMh == mathang.MaMh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["MaCd"] = new SelectList(_context.Chude, "MaCd", "Ten", mathang.MaCd);
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
            return RedirectToAction("Index", "Vendors");
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

using FoodMap.Data;
using FoodMap.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Controllers
{
    public class NhacungcapsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Nhacungcap> _passwordHasher;

        public NhacungcapsController(ApplicationDbContext context, IPasswordHasher<Nhacungcap> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nhacungcaps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Nhacungcap nhacungcap, string MatKhauNhap)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra Email xem đã tồn tại chưa
                var existing = await _context.Nhacungcap.FirstOrDefaultAsync(n => n.Email == nhacungcap.Email);
                if (existing != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng làm Tài khoản Gian hàng!");
                    return View(nhacungcap);
                }

                nhacungcap.MatKhau = _passwordHasher.HashPassword(nhacungcap, MatKhauNhap);

                // 2. Mặc định trạng thái CHỜ ADMIN DUYỆT (0)
                nhacungcap.TrangThaiXacThuc = 0; // 0: Chờ duyệt, 1: Đã duyệt
                nhacungcap.TrangThai = true;
                nhacungcap.NgayDangKy = DateTime.Now;

                _context.Add(nhacungcap);
                await _context.SaveChangesAsync();

                // 3. Tự động ĐĂNG NHẬP NGAY & Chuyển vào Vendor Dashboard
                HttpContext.Session.SetInt32("MaNcc", nhacungcap.MaNcc);
                HttpContext.Session.SetInt32("UserRole", 1); // 1: Vendor
                HttpContext.Session.SetString("UserName", nhacungcap.TenCongTy);
                HttpContext.Session.SetInt32("TrangThaiXacThuc", 0);

                return RedirectToAction("Index", "Vendors");
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

        // POST: Nhacungcaps/DuyetVendor/5
        [HttpPost]
        public async Task<IActionResult> DuyetVendor(int id)
        {
            var vendor = await _context.Nhacungcap.FindAsync(id);
            if (vendor == null) return NotFound();

            vendor.TrangThaiXacThuc = 1;
            vendor.NgayDuyet = DateTime.Now;

            _context.Update(vendor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Nhacungcaps/DangNhap
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }

        // POST: Nhacungcaps/DangNhap
        [HttpPost]
        public async Task<IActionResult> DangNhap(string email, string matKhau)
        {
            // 1. Tìm tài khoản gian hàng theo Email
            var vendor = await _context.Nhacungcap.FirstOrDefaultAsync(n => n.Email == email);

            if (vendor == null)
            {
                ViewBag.Loi = "Email gian hàng không tồn tại trong hệ thống!";
                return View();
            }

            // 2. Xác thực mật khẩu đã mã hóa bằng PasswordHasher
            var result = _passwordHasher.VerifyHashedPassword(vendor, vendor.MatKhau ?? "", matKhau);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Loi = "Mật khẩu không chính xác!";
                return View();
            }

            // 3. Kiểm tra tài khoản có bị khóa không
            if (vendor.TrangThai == false)
            {
                ViewBag.Loi = "Tài khoản Gian hàng của bạn tạm thời đang bị khóa!";
                return View();
            }

            // 4. Lưu Session đăng nhập cho Vendor
            HttpContext.Session.SetInt32("VendorId", vendor.MaNcc);
            HttpContext.Session.SetInt32("UserRole", 1); // 1 = Quyền Vendor
            HttpContext.Session.SetString("UserName", vendor.TenCongTy ?? "Gian hàng");
            HttpContext.Session.SetInt32("TrangThaiXacThuc", vendor.TrangThaiXacThuc);

            // 5. Chuyển hướng vào Cổng quản lý Gian hàng
            return RedirectToAction("Index", "Vendors");
        }
    }
}

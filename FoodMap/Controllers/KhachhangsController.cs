using FoodMap.Data;
using FoodMap.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FoodMap.Controllers
{
    public class KhachhangsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Khachhang> _passwordHasher;

        public KhachhangsController(ApplicationDbContext context, IPasswordHasher<Khachhang> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: Khachhangs/Index
        public async Task<IActionResult> Index()
        {
            var dsKhachHang = await _context.Khachhang
                .Where(k => k.VaiTro == 0) // Chỉ lấy danh sách Khách hàng
                .OrderByDescending(k => k.MaKh)
                .ToListAsync();

            return View(dsKhachHang);
        }

        // GET: Khachhangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khachhang = await _context.Khachhang.FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachhang == null) return NotFound();

            return View(khachhang);
        }

        // GET: Khachhangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Khachhangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khachhang khachhang, string MatKhauNhap)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = await _context.Khachhang.FirstOrDefaultAsync(k => k.Email == khachhang.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được đăng ký!");
                    return View(khachhang);
                }

                khachhang.MatKhau = _passwordHasher.HashPassword(khachhang, MatKhauNhap);
                khachhang.VaiTro = 0; // 0 = Khách hàng

                _context.Add(khachhang);
                await _context.SaveChangesAsync();

                TempData["SuccessMsg"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(DangNhap));
            }
            return View(khachhang);
        }


        // GET: Khachhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var khachhang = await _context.Khachhang.FindAsync(id);
            if (khachhang == null) return NotFound();

            return View(khachhang);
        }

        // POST: Khachhangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKh,Ten,DienThoai,Email,MatKhau")] Khachhang khachhang)
        {
            if (id != khachhang.MaKh) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachhangExists(khachhang.MaKh)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khachhang);
        }

        // GET: Khachhangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var khachhang = await _context.Khachhang.FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachhang == null) return NotFound();

            return View(khachhang);
        }

        // POST: Khachhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachhang = await _context.Khachhang.FindAsync(id);
            if (khachhang != null)
            {
                _context.Khachhang.Remove(khachhang);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachhangExists(int id)
        {
            return _context.Khachhang.Any(e => e.MaKh == id);
        }

        // =========================================================================
        // QUẢN LÝ GIỎ HÀNG (SESSION STATE)
        // =========================================================================

        public List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string? jsoncart = session.GetString("shopcart");
            if (!string.IsNullOrEmpty(jsoncart))
            {
                return JsonSerializer.Deserialize<List<CartItem>>(jsoncart) ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }

        private void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonSerializer.Serialize(list);
            session.SetString("shopcart", jsoncart);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove("shopcart");
        }

        // 1. Thêm đặc sản / voucher vào giỏ hàng
        public async Task<IActionResult> ThemGioHang(int id)
        {
            string? role = HttpContext.Session.GetString("Role");

            if (role == "Vendor" || role == "Admin")
            {
                TempData["ErrorMsg"] = "Tài khoản Quản trị / Nhà cung cấp không được phép thực hiện mua hàng!";
                return RedirectToAction("Index", "Home");
            }

            var mathang = await _context.Mathang.FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null) return NotFound("Sản phẩm không tồn tại");

            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                item.SoLuong++;
            }
            else
            {
                cart.Add(new CartItem() { MatHang = mathang, SoLuong = 1 });
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(XemGioHang));
        }

        // 2. Xem Giỏ hàng
        public IActionResult XemGioHang()
        {
            var cart = GetCartItems();
            ViewBag.CartCount = cart.Sum(i => i.SoLuong);
            return View(cart);
        }

        // 3. Cập nhật số lượng món ăn
        [HttpPost]
        public IActionResult CapNhatGioHang(int id, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.SoLuong = quantity;
                }
                else
                {
                    cart.Remove(item);
                }
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(XemGioHang));
        }

        // 4. Xóa món ăn khỏi giỏ
        public IActionResult XoaGioHang(int id)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(XemGioHang));
        }

        // 5. Màn hình Thanh toán (GET)
        public IActionResult ThanhToan()
        {
            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                return RedirectToAction(nameof(XemGioHang));
            }
            return View(cart);
        }

        // 6. Xử lý Thanh toán (POST) - Lưu Hóa đơn & Sinh mã Voucher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(string diachi, string dienthoai)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            var cart = GetCartItems();
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // A. Tạo Hóa đơn mới gắn MaKh của người dùng đăng nhập
            var hoadon = new Hoadon
            {
                MaKh = userId.Value,
                Ngay = DateTime.Now,
                TongTien = (int)cart.Sum(c => c.ThanhTien),
                TrangThai = 0 // 0: Chờ quy đổi Voucher
            };

            _context.Hoadon.Add(hoadon);
            await _context.SaveChangesAsync();

            // B. Tạo Chi tiết hóa đơn & Sinh mã Voucher ngẫu nhiên
            foreach (var item in cart)
            {
                var cthd = new Cthoadon
                {
                    MaHd = hoadon.MaHd,
                    MaMh = item.MatHang.MaMh,
                    SoLuong = (short)item.SoLuong,
                    DonGia = (int)(item.MatHang.GiaBan ?? 0),
                    ThanhTien = (int)item.ThanhTien,
                    MaVoucher = "VOUCHER-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                };
                _context.Cthoadon.Add(cthd);
            }

            await _context.SaveChangesAsync();

            // C. Xóa giỏ hàng trong Session sau khi thanh toán thành công
            ClearCart();

            return RedirectToAction(nameof(DonHangVoucher));
        }

        // =========================================================================
        // TRANG DANH SÁCH VOUCHER & ĐƠN HÀNG CỦA KHÁCH HÀNG
        // =========================================================================
        public async Task<IActionResult> DonHangVoucher()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            // Truy vấn danh sách Hóa đơn kèm Chi tiết hóa đơn
            var dsHoadon = await _context.Hoadon
                .Include(h => h.Cthoadons)
                    .ThenInclude(c => c.Mathang)
                .Where(h => h.MaKh == userId.Value)
                .OrderByDescending(h => h.Ngay)
                .ToListAsync();

            return View(dsHoadon);
        }

        // =========================================================================
        // TÀI KHOẢN (ĐĂNG KÝ, ĐĂNG NHẬP, ĐĂNG XUẤT)
        // =========================================================================


        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(string email, string matkhau)
        {
            var user = await _context.Khachhang.FirstOrDefaultAsync(k => k.Email == email);
            if (user != null && user.MatKhau != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.MatKhau, matkhau);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("khachhang", user.Email);
                    HttpContext.Session.SetString("TenNguoiDung", user.Ten ?? user.Email);
                    HttpContext.Session.SetInt32("UserId", user.MaKh);
                    HttpContext.Session.SetInt32("UserRole", user.VaiTro ?? 0);

                    if (user.VaiTro == 2) return RedirectToAction("Index", "Admin");
                    if (user.VaiTro == 1) return RedirectToAction("Index", "Vendors");

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ThongBaoLoi = "Email hoặc mật khẩu không chính xác!";
            return View();
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
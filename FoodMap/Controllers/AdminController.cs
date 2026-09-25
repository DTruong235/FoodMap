using FoodMap.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. BẢNG ĐIỀU HÀNH DASHBOARD ADMIN
        // =========================================================================
        // GET: Admin/Index
        public async Task<IActionResult> Index()
        {
            // 1. Thống kê 3 chỉ số chính
            ViewBag.TongDoanhThu = await _context.Hoadon.SumAsync(h => (decimal?)h.TongTien) ?? 0;
            ViewBag.TongVendor = await _context.Nhacungcap.CountAsync();
            ViewBag.TongMatHang = await _context.Mathang.CountAsync();

            // 2. Lấy 5 Vendor mới tham gia nhất
            ViewBag.VendorMoi = await _context.Nhacungcap
                .OrderByDescending(n => n.MaNcc)
                .Take(5)
                .ToListAsync();

            // 3. Lấy 5 Sản phẩm / Tour vừa được đăng gần đây
            var sanPhamMoi = await _context.Mathang
                .Include(m => m.Nhacungcap)
                .Include(m => m.Chude)
                .OrderByDescending(m => m.MaMh)
                .Take(5)
                .ToListAsync();

            return View(sanPhamMoi);
        }

        // =========================================================================
        // 2. QUẢN LÝ DANH SÁCH NHÀ CUNG CẤP / GIAN HÀNG
        // =========================================================================
        public async Task<IActionResult> QuanLyVendor()
        {
            int? role = HttpContext.Session.GetInt32("UserRole");
            if (role == null || role != 2)
            {
                return RedirectToAction("DangNhap", "Khachhangs");
            }

            var dsVendor = await _context.Nhacungcap.ToListAsync();
            return View(dsVendor);
        }

        // GET: Admin/DanhSachSanPhamVendor/5
        public async Task<IActionResult> DanhSachSanPhamVendor(int id)
        {
            // 1. Kiểm tra thông tin Nhà cung cấp (Vendor)
            var vendor = await _context.Nhacungcap.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            // 2. Lấy danh sách mặt hàng thuộc Vendor, kèm thông tin Chủ đề (Chude)
            var dsSanPham = await _context.Mathang
                .Include(m => m.Chude) // Nối bảng Chủ đề để hiển thị phân loại
                .Where(m => m.MaNcc == id)
                .OrderByDescending(m => m.MaMh)
                .ToListAsync();

            ViewBag.TenVendor = vendor.TenCongTy;
            ViewBag.MaVendor = id;

            return View(dsSanPham);
        }

        // GET: Admin/ThongKeDoanhThuVendor
        public async Task<IActionResult> ThongKeDoanhThuVendor(string loaiLoc = "tatca", int? thang = null, int? quy = null, int? nam = null, DateTime? ngay = null)
        {
            int currentYear = nam ?? DateTime.Now.Year;
            int currentMonth = thang ?? DateTime.Now.Month;
            DateTime currentDate = ngay ?? DateTime.Now.Date;

            var query = _context.Cthoadon
                .Include(ct => ct.Hoadon)
                .Include(ct => ct.Mathang)
                    .ThenInclude(mh => mh.Nhacungcap)
                .Where(ct => ct.Hoadon != null)
                .AsQueryable();

            // --- LỌC THEO THỜI GIAN ---
            switch (loaiLoc.ToLower())
            {
                case "ngay":
                    query = query.Where(ct => ct.Hoadon.Ngay.Date == currentDate);
                    break;

                case "tuan":
                    var tuNgay = currentDate.AddDays(-6);
                    query = query.Where(ct => ct.Hoadon.Ngay.Date >= tuNgay && ct.Hoadon.Ngay.Date <= currentDate);
                    break;

                case "thang":
                    query = query.Where(ct => ct.Hoadon.Ngay.Year == currentYear &&
                                              ct.Hoadon.Ngay.Month == currentMonth);
                    break;

                case "quy":
                    int selectedQuy = quy ?? ((currentMonth - 1) / 3 + 1);
                    int startMonth = (selectedQuy - 1) * 3 + 1;
                    int endMonth = startMonth + 2;
                    query = query.Where(ct => ct.Hoadon.Ngay.Year == currentYear &&
                                              ct.Hoadon.Ngay.Month >= startMonth &&
                                              ct.Hoadon.Ngay.Month <= endMonth);
                    ViewBag.Quy = selectedQuy;
                    break;

                case "nam":
                    query = query.Where(ct => ct.Hoadon.Ngay.Year == currentYear);
                    break;

                case "tatca":
                default:
                    // Không lọc thời gian -> Lấy toàn bộ dữ liệu mẫu trong CSDL
                    break;
            }

            // --- GOM NHÓM THEO VENDOR ---
            var dsDoanhThu = await query
                .GroupBy(ct => new { ct.Mathang.MaNcc, ct.Mathang.Nhacungcap.TenCongTy })
                .Select(g => new DoanhThuVendorViewModel
                {
                    MaNcc = g.Key.MaNcc,
                    TenNcc = g.Key.TenCongTy ?? "Chưa xác định",
                    TongSoLuong = g.Sum(x => x.SoLuong) ?? 0,
                    TongDoanhThu = g.Sum(x => (decimal)x.SoLuong * (x.DonGia ?? x.Mathang.GiaBan ?? 0))
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .ToListAsync();

            ViewBag.LoaiLoc = loaiLoc;
            ViewBag.Thang = currentMonth;
            ViewBag.Nam = currentYear;
            ViewBag.Ngay = currentDate.ToString("yyyy-MM-dd");

            return View(dsDoanhThu);
        }
        public class DoanhThuVendorViewModel
        {
            public int MaNcc { get; set; }
            public string TenNcc { get; set; } = string.Empty;
            public int TongSoLuong { get; set; }
            public decimal TongDoanhThu { get; set; }
        }

        // ==================== QUẢN LÝ TÀI KHOẢN KHÁCH HÀNG ====================

        // GET: Admin/QuanLyKhachHang
        public async Task<IActionResult> QuanLyKhachHang()
        {
            var dsKhachHang = await _context.Khachhang
                .OrderByDescending(k => k.MaKh)
                .ToListAsync();

            return View(dsKhachHang);
        }

        // GET: Admin/DoiTrangThaiKhachHang/5 (Mở khóa / Khóa tài khoản)
        public async Task<IActionResult> DoiTrangThaiKhachHang(int id)
        {
            var khachHang = await _context.Khachhang.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            _context.Update(khachHang);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(QuanLyKhachHang));
        }

        // ==================== KHÓA / MỞ KHÓA TÀI KHOẢN VENDOR ====================

        // GET: Admin/DoiTrangThaiVendor/5
        public async Task<IActionResult> DoiTrangThaiVendor(int id)
        {
            var vendor = await _context.Nhacungcap.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            // Đảo ngược trạng thái hoạt động của Vendor
            vendor.TrangThai = !(vendor.TrangThai);

            _context.Update(vendor);
            await _context.SaveChangesAsync();

            return RedirectToAction("QuanLyVendor");
        }
    }

}
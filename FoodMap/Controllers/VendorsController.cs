using FoodMap.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Controllers
{
    public class VendorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. DASHBOARD GIAN HÀNG VENDOR
        // =========================================================================
        public async Task<IActionResult> Index()
        {
            // Kiểm tra phân quyền Vendor (UserRole == 1)
            int? role = HttpContext.Session.GetInt32("UserRole");
            if (role == null || role != 1)
            {
                return RedirectToAction("DangNhap", "Khachhangs");
            }

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Lấy danh sách món ăn/tour do gian hàng này sở hữu
            var dsSanPham = await _context.Mathang
                .Include(m => m.Chude)
                .Include(m => m.Nhacungcap)
                .ToListAsync();

            ViewBag.TongSanPham = dsSanPham.Count;
            return View(dsSanPham);
        }

        // =========================================================================
        // 2. MÀN HÌNH QUÉT & NHẬP MÃ QR VOUCHER
        // =========================================================================
        public IActionResult QuetQR()
        {
            int? role = HttpContext.Session.GetInt32("UserRole");
            if (role == null || role != 1)
            {
                return RedirectToAction("DangNhap", "Khachhangs");
            }

            return View();
        }

        // =========================================================================
        // 3. API XÁC THỰC MÃ VOUCHER (AJAX POST)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> XacThucVoucher(string maVoucher)
        {
            if (string.IsNullOrWhiteSpace(maVoucher))
            {
                return Json(new { success = false, message = "Vui lòng quét hoặc nhập mã Voucher!" });
            }

            maVoucher = maVoucher.Trim().ToUpper();

            // Tìm chi tiết hóa đơn chứa mã Voucher tương ứng
            var cthd = await _context.Cthoadon
                .Include(c => c.Mathang)
                .Include(c => c.Hoadon)
                .FirstOrDefaultAsync(c => c.MaVoucher == maVoucher);

            if (cthd == null)
            {
                return Json(new
                {
                    success = false,
                    message = $"Mã Voucher [{maVoucher}] không tồn tại trên hệ thống FoodMap!"
                });
            }

            // Kiểm tra xem Voucher đã được quy đổi chưa (TrangThai == 1)
            bool daSuDung = cthd.Hoadon.TrangThai == 1;

            if (daSuDung)
            {
                return Json(new
                {
                    success = false,
                    isUsed = true,
                    message = $"Mã Voucher [{maVoucher}] ĐÃ ĐƯỢC QUY ĐỔI SỬ DỤNG TRƯỚC ĐÓ!",
                    tenMon = cthd.Mathang?.Ten,
                    tenKhach = cthd.Hoadon?.Khachhang?.Ten ?? "Du khách",
                    ngayDat = cthd.Hoadon?.Ngay.ToString("dd/MM/yyyy HH:mm")
                });
            }

            // GẠCH NỢ / CẬP NHẬT TRẠNG THÁI QUY ĐỔI
            cthd.Hoadon.TrangThai = 1; // 1: Đã quy đổi thành công
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "XÁC THỰC & ĐỔI VOUCHER THÀNH CÔNG!",
                maVoucher = cthd.MaVoucher,
                tenMon = cthd.Mathang?.Ten,
                soLuong = cthd.SoLuong,
                donGia = cthd.DonGia?.ToString("N0"),
                thanhTien = cthd.ThanhTien?.ToString("N0"),
                tenKhach = cthd.Hoadon?.Khachhang?.Ten ?? "Du khách",
                dienthoai = cthd.Hoadon?.Khachhang?.DienThoai ?? "Chưa cập nhật",
                ngayDat = cthd.Hoadon?.Ngay.ToString("dd/MM/yyyy HH:mm")
            });
        }

        // GET: Vendor/QuanLyDonHang
        public async Task<IActionResult> QuanLyDonHang()
        {
            // 1. Kiểm tra phân quyền Vendor
            int? role = HttpContext.Session.GetInt32("UserRole");
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (role == null || role != 1 || userId == null)
            {
                return RedirectToAction("DangNhap", "Khachhangs");
            }

            // 2. Lấy danh sách Hóa đơn có chứa sản phẩm của Vendor
            // (Ở đây lấy toàn bộ Hóa đơn hệ thống hoặc lọc theo MaNcc nếu có liên kết)
            var dsDonHang = await _context.Hoadon
                .Include(h => h.Khachhang)
                .Include(h => h.Cthoadons)
                    .ThenInclude(ct => ct.Mathang)
                .OrderByDescending(h => h.Ngay)
                .ToListAsync();

            return View(dsDonHang);
        }
    }
}
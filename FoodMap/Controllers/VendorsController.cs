using FoodMap.Data;
using FoodMap.Models;
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

            int userId = HttpContext.Session.GetInt32("MaNcc") ?? 0;

            // Lấy danh sách món ăn/tour do gian hàng này sở hữu
            var dsSanPham = await _context.Mathang
                .Include(m => m.Chude)
                .Include(m => m.Nhacungcap)
                .Where(m => m.MaNcc == userId)
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
            int? userId = HttpContext.Session.GetInt32("MaNcc");
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

        // POST: Vendor/CapNhatTrangThai
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int maHd, int trangThaiMoi)
        {
            int? role = HttpContext.Session.GetInt32("UserRole");
            if (role == null || role != 1) return RedirectToAction("DangNhap", "Khachhangs");

            var hoadon = await _context.Hoadon.FindAsync(maHd);
            if (hoadon != null)
            {
                hoadon.TrangThai = trangThaiMoi; // 0: Chờ xác nhận, 1: Đã xác nhận/Đang chuẩn bị, 2: Đã hoàn thành, 3: Đã hủy
                await _context.SaveChangesAsync();
                TempData["SuccessMsg"] = $"Cập nhật trạng thái đơn hàng #HD{maHd} thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Không tìm thấy thông tin đơn hàng!";
            }

            return RedirectToAction(nameof(QuanLyDonHang));
        }

        // GET: Vendor/ThongKe
        public async Task<IActionResult> ThongKe(string loaiThoiGian = "7ngay")
        {
            int? role = HttpContext.Session.GetInt32("UserRole");
            if (role == null || role != 1) return RedirectToAction("DangNhap", "Khachhangs");

            var dsHoadon = await _context.Hoadon.ToListAsync();
            var now = DateTime.Now;

            List<string> chartLabels = new List<string>();
            List<decimal> chartData = new List<decimal>();
            List<Hoadon> filteredOrders = new List<Hoadon>();

            // XỬ LÝ THEO MỐC THỜI GIAN ĐƯỢC CHỌN
            switch (loaiThoiGian?.ToLower())
            {
                case "tuan": // 4 TUẦN GẦN NHẤT
                    filteredOrders = dsHoadon.Where(h => h.Ngay.Date >= now.Date.AddDays(-27)).ToList();
                    for (int i = 3; i >= 0; i--)
                    {
                        var endOfWeek = now.Date.AddDays(-i * 7);
                        var startOfWeek = endOfWeek.AddDays(-6);
                        chartLabels.Add($"{startOfWeek:dd/MM} - {endOfWeek:dd/MM}");
                        var sum = dsHoadon.Where(h => h.TrangThai == 2 && h.Ngay.Date >= startOfWeek && h.Ngay.Date <= endOfWeek)
                                          .Sum(h => h.TongTien ?? 0);
                        chartData.Add(sum);
                    }
                    break;

                case "thang": // 12 THÁNG TRONG NĂM HỆN TẠI
                    filteredOrders = dsHoadon.Where(h => h.Ngay.Year == now.Year).ToList();
                    for (int m = 1; m <= 12; m++)
                    {
                        chartLabels.Add($"T{m}");
                        var sum = dsHoadon.Where(h => h.TrangThai == 2 && h.Ngay.Year == now.Year && h.Ngay.Month == m)
                                          .Sum(h => h.TongTien ?? 0);
                        chartData.Add(sum);
                    }
                    break;

                case "quy": // 4 QUÝ TRONG NĂM HỆN TẠI
                    filteredOrders = dsHoadon.Where(h => h.Ngay.Year == now.Year).ToList();
                    for (int q = 1; q <= 4; q++)
                    {
                        chartLabels.Add($"Quý {q}");
                        int startMonth = (q - 1) * 3 + 1;
                        int endMonth = startMonth + 2;
                        var sum = dsHoadon.Where(h => h.TrangThai == 2 && h.Ngay.Year == now.Year && h.Ngay.Month >= startMonth && h.Ngay.Month <= endMonth)
                                          .Sum(h => h.TongTien ?? 0);
                        chartData.Add(sum);
                    }
                    break;

                case "7ngay":
                default: // 7 NGÀY GẦN NHẤT (MẶC ĐỊNH)
                    loaiThoiGian = "7ngay";
                    var last7Days = Enumerable.Range(0, 7).Select(i => now.Date.AddDays(-6 + i)).ToList();
                    chartLabels = last7Days.Select(d => d.ToString("dd/MM")).ToList();
                    filteredOrders = dsHoadon.Where(h => h.Ngay.Date >= now.Date.AddDays(-6)).ToList();
                    chartData = last7Days.Select(d => (decimal)dsHoadon.Where(h => h.TrangThai == 2 && h.Ngay.Date == d).Sum(h => h.TongTien ?? 0)).ToList();
                    break;
            }

            // Cập nhật các thẻ KPI theo mốc thời gian đã lọc
            ViewBag.TongDoanhThu = filteredOrders.Where(h => h.TrangThai == 2).Sum(h => h.TongTien ?? 0);
            ViewBag.TongDonHang = filteredOrders.Count;
            ViewBag.DonHoanThanh = filteredOrders.Count(h => h.TrangThai == 2);
            ViewBag.DonChoXacNhan = filteredOrders.Count(h => h.TrangThai == 0);
            ViewBag.LoaiThoiGian = loaiThoiGian;

            ViewBag.ChartLabels = System.Text.Json.JsonSerializer.Serialize(chartLabels);
            ViewBag.ChartData = System.Text.Json.JsonSerializer.Serialize(chartData);

            return View();
        }
    }
}
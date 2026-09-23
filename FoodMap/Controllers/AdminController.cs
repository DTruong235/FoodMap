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
        public async Task<IActionResult> Index()
        {
            // Kiểm tra phân quyền Admin (UserRole == 2)
            int? role = HttpContext.Session.GetInt32("UserRole");
            if (role == null || role != 2)
            {
                return RedirectToAction("DangNhap", "Khachhangs");
            }

            // Thống kê số liệu toàn hệ thống
            ViewBag.TongDoanhThu = await _context.Hoadon.SumAsync(h => (decimal?)h.TongTien) ?? 0;
            ViewBag.TongDonHang = await _context.Hoadon.CountAsync();
            ViewBag.TongVendor = await _context.Nhacungcap.CountAsync();
            ViewBag.TongKhachHang = await _context.Khachhang.Where(k => k.VaiTro == 0 || k.VaiTro == null).CountAsync();

            // Lấy 5 đơn hàng mới nhất trên toàn hệ thống
            var dsDonHangMoi = await _context.Hoadon
                .OrderByDescending(h => h.Ngay)
                .Take(5)
                .ToListAsync();

            return View(dsDonHangMoi);
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
    }
}
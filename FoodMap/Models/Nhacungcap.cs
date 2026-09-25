using System.ComponentModel.DataAnnotations;

namespace FoodMap.Models
{
    public class Nhacungcap
    {
        [Key]
        public int MaNcc { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Tên công ty / Cở sở")]
        public string TenCongTy { get; set; } = null!;

        [StringLength(20)]
        public string? DienThoai { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public string? MatKhau { get; set; }           // Mật khẩu đăng nhập gian hàng
        public string? NguoiDaiDien { get; set; }       // Chủ gian hàng / Người đại diện
        public string? MaSoThue { get; set; }          // Mã số thuế / Số GPKD
        public int TrangThaiXacThuc { get; set; } = 0; // 0: Chờ Admin duyệt, 1: Đã duyệt, 2: Bị từ chối
        public bool? TrangThai { get; set; } = true;     // true: Hoạt động, false: Bị khóa
        public DateTime? NgayDangKy { get; set; } = DateTime.Now;
        public DateTime? NgayDuyet { get; set; }

        public virtual ICollection<Mathang> Mathangs { get; set; } = new List<Mathang>();
    }
}

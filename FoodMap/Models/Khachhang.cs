using System.ComponentModel.DataAnnotations;

namespace FoodMap.Models
{
    public class Khachhang
    {
        [Key]
        public int MaKh { get; set; }

        [Required]
        [StringLength(100)]
        public string Ten { get; set; } = null!;

        [StringLength(20)]
        public string? DienThoai { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? MatKhau { get; set; }
        // 0 = Khách hàng, 1 = Vendor, 2 = Admin
        public int? VaiTro { get; set; } = 0;

        public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
    }
}

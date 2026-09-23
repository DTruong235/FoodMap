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

        public bool TrangThai { get; set; } = true;

        public virtual ICollection<Mathang> Mathangs { get; set; } = new List<Mathang>();
    }
}

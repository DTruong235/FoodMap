using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodMap.Models
{
    public class Mathang
    {
        [Key]
        public int MaMh { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên món / Tour")]
        public string Ten { get; set; } = null!;

        [Display(Name = "Giá gốc")]
        public int? GiaGoc { get; set; }

        [Display(Name = "Giá bán")]
        public int? GiaBan { get; set; }

        [Display(Name = "Số lượng")]
        public short? SoLuong { get; set; }

        public string? MoTa { get; set; }

        [StringLength(255)]
        public string? HinhAnh { get; set; }

        public int MaCd { get; set; }
        [ForeignKey("MaCd")]
        public virtual Chude? Chude { get; set; }

        public int MaNcc { get; set; }
        [ForeignKey("MaNcc")]
        public virtual Nhacungcap? Nhacungcap { get; set; }

        public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();
    }
}

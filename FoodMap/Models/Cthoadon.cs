using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodMap.Models
{
    public class Cthoadon
    {
        [Key]
        public int MaCthd { get; set; }

        public int MaHd { get; set; }
        [ForeignKey("MaHd")]
        public virtual Hoadon? Hoadon { get; set; }

        public int MaMh { get; set; }
        [ForeignKey("MaMh")]
        public virtual Mathang? Mathang { get; set; }

        public int? DonGia { get; set; }

        public short? SoLuong { get; set; }

        public int? ThanhTien { get; set; }

        [StringLength(50)]
        public string? MaVoucher { get; set; }
    }
}

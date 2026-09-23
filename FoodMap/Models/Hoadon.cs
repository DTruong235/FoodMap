using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodMap.Models
{
    public class Hoadon
    {
        [Key]
        public int MaHd { get; set; }

        public DateTime Ngay { get; set; } = DateTime.Now;

        public int? TongTien { get; set; }

        public int MaKh { get; set; }
        [ForeignKey("MaKh")]
        public virtual Khachhang? Khachhang { get; set; }

        public int TrangThai { get; set; } = 0;

        public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    }
}

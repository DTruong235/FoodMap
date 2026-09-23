using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodMap.Models
{
    public class Chude
    {
        [Key]
        public int MaCd { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên chủ đề")]
        public string TenCd { get; set; } = null!;

        public int MaDm { get; set; }

        [ForeignKey("MaDm")]
        public virtual Danhmuc? Danhmuc { get; set; }

        public virtual ICollection<Mathang> Mathangs { get; set; } = new List<Mathang>();
    }
}

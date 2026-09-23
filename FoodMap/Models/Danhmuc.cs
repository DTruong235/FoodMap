using System.ComponentModel.DataAnnotations;

namespace FoodMap.Models
{
    public class Danhmuc
    {
        [Key]
        public int MaDm { get; set; }

        [Required(ErrorMessage = "Tên vùng miền không được để trống")]
        [StringLength(100)]
        [Display(Name = "Vùng miền")]
        public string Ten { get; set; } = null!;

        public virtual ICollection<Chude> Chudes { get; set; } = new List<Chude>();
    }
}

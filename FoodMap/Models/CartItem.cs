namespace FoodMap.Models
{
    public class CartItem
    {
        public Mathang MatHang { get; set; } = null!;
        public int SoLuong { get; set; }
        public double GiaBan => MatHang?.GiaBan ?? 0;
        public double ThanhTien => GiaBan * SoLuong;
    }
}

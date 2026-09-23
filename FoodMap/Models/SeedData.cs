using FoodMap.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Danhmuc.Any()) return; // Đã có dữ liệu

                var dm1 = new Danhmuc { Ten = "Ẩm thực Miền Bắc" };
                var dm2 = new Danhmuc { Ten = "Ẩm thực Miền Nam" };
                context.Danhmuc.AddRange(dm1, dm2);
                context.SaveChanges();

                var cd1 = new Chude { TenCd = "Food Tour Phố Cổ", MaDm = dm1.MaDm };
                var cd2 = new Chude { TenCd = "Đặc sản làm quà", MaDm = dm2.MaDm };
                context.Chude.AddRange(cd1, cd2);
                context.SaveChanges();

                var ncc = new Nhacungcap { TenCongTy = "Công ty Lữ hành FoodMap", Email = "contact@foodmap.vn" };
                context.Nhacungcap.Add(ncc);
                context.SaveChanges();

                context.Mathang.AddRange(
                    new Mathang { Ten = "Tour Trải Nghiệm Phố Cổ Hà Nội", GiaBan = 350000, SoLuong = 20, HinhAnh = "foodtour.png", MaCd = cd1.MaCd, MaNcc = ncc.MaNcc },
                    new Mathang { Ten = "Bánh Pía Sóc Trăng Thượng Hạng", GiaBan = 120000, SoLuong = 50, HinhAnh = "banhpia.webp", MaCd = cd2.MaCd, MaNcc = ncc.MaNcc }
                );
                context.SaveChanges();
            }
        }
    }
}

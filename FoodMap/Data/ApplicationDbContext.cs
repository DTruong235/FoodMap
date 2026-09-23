using FoodMap.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodMap.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Danhmuc> Danhmuc { get; set; } = null!;
    public DbSet<Chude> Chude { get; set; } = null!;
    public DbSet<Nhacungcap> Nhacungcap { get; set; } = null!;
    public DbSet<Mathang> Mathang { get; set; } = null!;
    public DbSet<Khachhang> Khachhang { get; set; } = null!;
    public DbSet<Hoadon> Hoadon { get; set; } = null!;
    public DbSet<Cthoadon> Cthoadon { get; set; } = null!;
}

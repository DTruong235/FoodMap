using FoodMap.Data;
using FoodMap.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// 2.1. ??ng ký D?ch v? Mã hóa M?t kh?u cho Khachhang (dùng Scoped)
builder.Services.AddScoped<IPasswordHasher<Khachhang>, PasswordHasher<Khachhang>>();

// 2.2. ??ng ký Caching & Session qu?n lý Gi? hàng + Phiên ??ng nh?p
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "FoodMapCartSession";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2.3. ??ng ký Controllers & Views (MVC) + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// B?T BU?C: UseSession ph?i ??ng TR??C UseAuthorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 3.1. ??nh tuy?n Route cho Controllers (MVC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "L?i khi n?p SeedData.");
    }
}

app.Run();
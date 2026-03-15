using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

namespace SunriseHotelApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================================================
            // PHẦN 1: ĐĂNG KÝ DỊCH VỤ (SERVICES)
            // ============================================================

            // 1. Đăng ký kết nối Database SQL Server
            // Lưu ý: Nếu máy bạn dùng SQL Express thì đổi "Server=." thành "Server=.\\SQLEXPRESS"
            builder.Services.AddDbContext<HotelManagementDbContext>(options =>
                options.UseSqlServer("Server=.;Database=HotelManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"));

            // 2. Đăng ký HttpContextAccessor (QUAN TRỌNG CHO TRANG ADMIN)
            // Giúp lấy thông tin User/Role từ Session tại file Layout
            builder.Services.AddHttpContextAccessor();

            // 3. Đăng ký Distributed Memory Cache (BẮT BUỘC ĐỂ DÙNG SESSION ỔN ĐỊNH)
            // (Bạn nên thêm dòng này)
            builder.Services.AddDistributedMemoryCache();

            // 4. Đăng ký Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Hết phiên sau 30 phút
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 5. Đăng ký MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // ============================================================
            // PHẦN 2: CẤU HÌNH PIPELINE (MIDDLEWARE)
            // ============================================================

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 6. Kích hoạt Session (Đặt TRƯỚC UseAuthorization)
            app.UseSession();

            app.UseAuthorization();

            // Định tuyến mặc định (Chạy vào trang chủ trước)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
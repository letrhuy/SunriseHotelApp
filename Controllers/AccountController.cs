using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

namespace SunriseHotelApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public AccountController(HotelManagementDbContext context)
        {
            _context = context;
        }

        // 1. HIỂN THỊ TRANG LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            var currentRole = HttpContext.Session.GetString("Role");

            // Nếu đã đăng nhập, điều hướng về đúng trang theo chức vụ
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                if (currentRole == "Admin" || currentRole == "Manager")
                    return RedirectToAction("Index", "Dashboard");
                else
                    return RedirectToAction("Dashboard", "Bookings");
            }

            return View();
        }

        // 2. XỬ LÝ ĐĂNG NHẬP
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Tìm user: Khớp Username, Password và Tài khoản phải đang Hoạt động (IsActive == true)
            var user = await _context.SystemUsers
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password && u.IsActive == true);

            if (user != null)
            {
                string role = user.UserRole ?? "Staff";

                // ==========================================
                // BƯỚC CHẶN QUYỀN: CHỈ CHO ADMIN, MANAGER VÀ STAFF VÀO
                // ==========================================
                if (role != "Admin" && role != "Manager" && role != "Staff")
                {
                    ViewBag.Error = "Truy cập bị từ chối! Chức vụ của bạn không có quyền đăng nhập vào hệ thống này.";
                    return View();
                }

                // Lưu thông tin vào Session
                HttpContext.Session.SetString("UserName", user.Username);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("FullName", user.FullName ?? "Nhân viên");

                // ==========================================
                // BƯỚC ĐIỀU HƯỚNG: PHÂN LUỒNG GIAO DIỆN
                // ==========================================
                if (role == "Admin" || role == "Manager")
                {
                    // Quản lý và Admin -> Bay vào trang Tổng quan (Thống kê, Báo cáo...)
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (role == "Staff")
                {
                    // Lễ tân (Staff) -> Bay thẳng vào trang Sơ đồ phòng để làm việc
                    return RedirectToAction("Dashboard", "Bookings");
                }
            }

            // Đăng nhập thất bại (Sai tài khoản, mật khẩu hoặc bị khóa)
            ViewBag.Error = "Sai tài khoản, mật khẩu hoặc tài khoản đã bị khóa!";
            return View();
        }

        // 3. ĐĂNG XUẤT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
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

        [HttpGet]
        public IActionResult Login()
        {
            var currentRole = HttpContext.Session.GetString("Role");

        
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                if (currentRole == "Admin" || currentRole == "Manager")
                    return RedirectToAction("Index", "Dashboard");
                else
                    return RedirectToAction("Dashboard", "Bookings");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
          
            var user = await _context.SystemUsers
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password && u.IsActive == true);

            if (user != null)
            {
                string role = user.UserRole ?? "Staff";

                if (role != "Admin" && role != "Manager" && role != "Staff")
                {
                    ViewBag.Error = "Truy cập bị từ chối! Chức vụ của bạn không có quyền đăng nhập vào hệ thống này.";
                    return View();
                }

      
                HttpContext.Session.SetString("UserName", user.Username);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("FullName", user.FullName ?? "Nhân viên");

                if (role == "Admin" || role == "Manager")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (role == "Staff")
                {
                    return RedirectToAction("Dashboard", "Bookings");
                }
            }

            ViewBag.Error = "Sai tài khoản, mật khẩu hoặc tài khoản đã bị khóa!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
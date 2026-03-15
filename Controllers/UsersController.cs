using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

namespace SunriseHotelApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public UsersController(HotelManagementDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH NHÂN SỰ
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("Dashboard", "Bookings");
            }

            var users = await _context.SystemUsers.ToListAsync();
            return View(users);
        }

        // 2. THÊM NHÂN VIÊN MỚI (GET)
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Dashboard", "Bookings");
            return View();
        }

        // 3. XỬ LÝ LƯU NHÂN VIÊN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemUser user)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Dashboard", "Bookings");

            if (ModelState.IsValid)
            {
                var exists = await _context.SystemUsers.AnyAsync(u => u.Username == user.Username);
                if (exists)
                {
                    ModelState.AddModelError("Username", "Tên tài khoản này đã tồn tại!");
                    return View(user);
                }

                user.IsActive = true; 
                _context.SystemUsers.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // 4. CHỈNH SỬA NHÂN VIÊN (GET) 
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Dashboard", "Bookings");

            if (id == null) return NotFound();

            var user = await _context.SystemUsers.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // 5. XỬ LÝ CẬP NHẬT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SystemUser user)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Dashboard", "Bookings");

            if (id != user.UserId) return NotFound();

            var existingUser = await _context.SystemUsers.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.FullName = user.FullName;
            existingUser.UserRole = user.UserRole;
            existingUser.IsActive = user.IsActive;

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = user.PasswordHash;
            }

            try
            {
                _context.Update(existingUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        // 6. KHÓA / MỞ KHÓA TÀI KHOẢN
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Dashboard", "Bookings");

            var user = await _context.SystemUsers.FindAsync(id);
            if (user != null)
            {
                var currentUser = HttpContext.Session.GetString("Username");
                if (user.Username == currentUser)
                {
                    return RedirectToAction(nameof(Index));
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
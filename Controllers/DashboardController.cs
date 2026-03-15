using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

namespace SunriseHotelApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public DashboardController(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Kiểm tra đăng nhập 
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Today;

            // A. Doanh thu ngày hôm nay
            var dailyRevenue = await _context.Bookings
                .Where(b => b.BookingStatus == "Completed" && b.CheckOutDate.Date == today)
                .SumAsync(b => b.TotalAmount);

            // B. Đếm số phòng đang có khách 
            var occupiedCount = await _context.Rooms.CountAsync(r => r.RoomStatus == "Occupied");
            var totalRooms = await _context.Rooms.CountAsync();

            // C. Cảnh báo kho 
            var lowStockCount = await _context.Products.CountAsync(p => p.CurrentStock <= p.MinStockLevel);

            ViewBag.DailyRevenue = dailyRevenue;
            ViewBag.OccupiedCount = occupiedCount;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.LowStockCount = lowStockCount;
            ViewBag.UserName = HttpContext.Session.GetString("FullName") ?? HttpContext.Session.GetString("UserName");

            return View();
        }

        // ==========================================
        // THÊM HÀM "GIẢ" ĐỂ CHỐNG LỖI 404 Ở FRONTEND
        // ==========================================
        [HttpGet]
        public IActionResult GetUnreadNotifications()
        {
            // Trả về danh sách rỗng để JavaScript không bị lỗi 404
            return Json(new List<object>());
        }

        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            // Trả về OK để JavaScript không báo lỗi
            return Ok();
        }

        // ==========================================
        // TÍNH NĂNG: BÁO CÁO DOANH THU
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> RevenueReport(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.PaymentStatus == "Paid" || b.BookingStatus == "Completed")
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(b => b.CheckOutDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(b => b.CheckOutDate.Date <= toDate.Value.Date);

            var bookings = await query.OrderByDescending(b => b.CheckOutDate).ToListAsync();

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.TotalRevenue = bookings.Sum(b => b.FinalAmount);

            return View(bookings);
        }

        // ==========================================
        // BÁO CÁO NHÂN SỰ: LỌC THEO CHỨC VỤ
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> EmployeeReport(string? roleFilter)
        {
            // Lấy toàn bộ danh sách nhân viên
            var query = _context.SystemUsers.AsQueryable();

            // Nếu người dùng có chọn chức vụ để lọc thì áp dụng bộ lọc
            if (!string.IsNullOrEmpty(roleFilter))
            {
                query = query.Where(u => u.UserRole == roleFilter);
            }

            // Sắp xếp theo chức vụ rồi đến tên
            var employees = await query.OrderBy(u => u.UserRole).ThenBy(u => u.FullName).ToListAsync();

            // Truyền dữ liệu ra View
            ViewBag.TotalEmployees = employees.Count;
            ViewBag.CurrentRoleFilter = roleFilter;

            return View(employees);
        }
    }
}
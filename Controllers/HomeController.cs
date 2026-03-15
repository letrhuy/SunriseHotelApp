using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;
using System.Diagnostics;

namespace SunriseHotelApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public HomeController(HotelManagementDbContext context)
        {
            _context = context;
        }

        // GET: /
        // Trang chủ chỉ hiển thị danh sách các Hạng phòng để giới thiệu (Showcase)
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả loại phòng để hiển thị phần "Hạng Phòng Nổi Bật"
            var roomTypes = await _context.RoomTypes.ToListAsync();
            return View(roomTypes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
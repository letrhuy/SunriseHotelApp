using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Nhớ dòng này để dùng Include
using SunriseHotelApp.Models;

namespace SunriseHotelApp.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public RoomsController(HotelManagementDbContext context)
        {
            _context = context;
        }

        // GET: Hiển thị Sơ đồ phòng
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách phòng và kèm theo tên Loại phòng (Deluxe/VIP...)
            var rooms = await _context.Rooms
                                      .Include(r => r.RoomType)
                                      .OrderBy(r => r.RoomNumber) // Sắp xếp theo số phòng
                                      .ToListAsync();
            return View(rooms);
        }
    }
}
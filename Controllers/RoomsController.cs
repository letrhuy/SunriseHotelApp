using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                                      .Include(r => r.RoomType)
                                      .OrderBy(r => r.RoomNumber) 
                                      .ToListAsync();
            return View(rooms);
        }
    }
}
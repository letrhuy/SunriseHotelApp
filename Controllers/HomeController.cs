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

        public async Task<IActionResult> Index()
        {
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
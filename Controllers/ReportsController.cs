using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

namespace SunriseHotelApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public ReportsController(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bestSellers = await _context.BookingServices
                .AsNoTracking()
                .Include(bs => bs.Product)
                .GroupBy(bs => bs.Product.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => (decimal)x.Quantity * x.PriceAtOrder)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync();

            ViewBag.BestSellers = bestSellers;
            var paymentHistory = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.BookingStatus == "Completed")
                .OrderByDescending(b => b.CheckOutDate)
                .ToListAsync();

            ViewBag.TotalAccumulatedRevenue = paymentHistory.Sum(b => b.FinalAmount);

            return View(paymentHistory);
        }
    }
}
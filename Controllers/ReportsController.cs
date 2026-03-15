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
            // 1. Lấy danh sách 10 sản phẩm/dịch vụ bán chạy nhất (Best Sellers)
            var bestSellers = await _context.BookingServices
                .AsNoTracking()
                .Include(bs => bs.Product)
                .GroupBy(bs => bs.Product.ProductName)
                .Select(g => new {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    // Ép kiểu (decimal) để đảm bảo phép nhân chính xác
                    TotalRevenue = g.Sum(x => (decimal)x.Quantity * x.PriceAtOrder)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync();

            ViewBag.BestSellers = bestSellers;

            // 2. Lấy danh sách lịch sử thanh toán thành công
            var paymentHistory = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.BookingStatus == "Completed")
                .OrderByDescending(b => b.CheckOutDate)
                .ToListAsync();

            // --- SỬA LỖI TẠI ĐÂY: Xóa "?? 0m" vì FinalAmount không bao giờ null ---
            ViewBag.TotalAccumulatedRevenue = paymentHistory.Sum(b => b.FinalAmount);

            return View(paymentHistory);
        }
    }
}
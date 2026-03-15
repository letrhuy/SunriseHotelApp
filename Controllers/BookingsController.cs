using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

namespace SunriseHotelApp.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public BookingsController(HotelManagementDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // PHẦN 1: DÀNH CHO KHÁCH HÀNG (CLIENT)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> Search(DateTime? checkIn, DateTime? checkOut, int adults = 1)
        {
            var dCheckIn = checkIn ?? DateTime.Now;
            var dCheckOut = checkOut ?? DateTime.Now.AddDays(1);
            var availableRoomTypes = await _context.RoomTypes
                .Where(rt => rt.Rooms.Any(r => r.RoomStatus == "Available"))
                .ToListAsync();

            ViewBag.CheckIn = dCheckIn;
            ViewBag.CheckOut = dCheckOut;
            return View(availableRoomTypes);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? roomTypeId, DateTime? checkIn, DateTime? checkOut)
        {
            if (roomTypeId == null) return RedirectToAction("Search");
            var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
            if (roomType == null) return NotFound();

            var dCheckIn = checkIn ?? DateTime.Now;
            var dCheckOut = checkOut ?? DateTime.Now.AddDays(1);
            var totalDays = (dCheckOut - dCheckIn).Days;
            if (totalDays <= 0) totalDays = 1;

            ViewBag.RoomType = roomType;
            ViewBag.CheckIn = dCheckIn;
            ViewBag.CheckOut = dCheckOut;
            ViewBag.TotalDays = totalDays;
            ViewBag.RoomTotal = roomType.PricePerNight * totalDays;
            ViewBag.ShuttlePrice = 200000;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking, string CustomerName, string CustomerPhone, string CustomerEmail, string CustomerIdCard, bool IsShuttleService, int RoomTypeId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == CustomerPhone);
            if (customer == null)
            {
                customer = new Customer { FullName = CustomerName, PhoneNumber = CustomerPhone, Email = CustomerEmail, IdentityCard = CustomerIdCard };
                _context.Add(customer);
                await _context.SaveChangesAsync();
            }
            booking.CustomerId = customer.CustomerId;

            decimal extraFee = 0;
            if (IsShuttleService) { extraFee = 200000; booking.Notes += " [XE ĐƯA ĐÓN]"; }
            booking.TotalAmount += extraFee;
            booking.FinalAmount = booking.TotalAmount;

            var availableRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomTypeId == RoomTypeId && r.RoomStatus == "Available");
            if (availableRoom != null)
            {
                booking.RoomId = availableRoom.RoomId;
                availableRoom.RoomStatus = "Occupied";
                _context.Update(availableRoom);
            }
            else return RedirectToAction("Search");

            booking.BookingDate = DateTime.Now;
            booking.BookingStatus = "Occupied";
            booking.PaymentStatus = "Unpaid";

            _context.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("BookingSuccess");
        }

        public IActionResult BookingSuccess() => View();

        // ==========================================
        // PHẦN 2: DÀNH CHO ADMIN (DASHBOARD, SERVICE, CHECKOUT)
        // ==========================================

        // 1. DASHBOARD (SƠ ĐỒ PHÒNG)
        public async Task<IActionResult> Dashboard()
        {
            // Sửa key thành "UserName" (N hoa) để khớp với AccountController
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rooms = await _context.Rooms.Include(r => r.RoomType).OrderBy(r => r.RoomNumber).ToListAsync();
            return View(rooms);
        }

        // 2. GỌI DỊCH VỤ (VIEW)
        [HttpGet]
        public async Task<IActionResult> AddService(int roomId)
        {
            if (HttpContext.Session.GetString("UserName") == null) return RedirectToAction("Login", "Account");

            var booking = await _context.Bookings.Include(b => b.Room).Include(b => b.Customer)
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefaultAsync(b => b.RoomId == roomId && b.BookingStatus == "Occupied");

            if (booking == null) return Content("Phòng này hiện không có khách!");

            ViewBag.Products = await _context.Products.Where(p => p.CurrentStock > 0).ToListAsync();
            return View(booking);
        }

        // 3. GỌI DỊCH VỤ (POST)
        [HttpPost]
        public async Task<IActionResult> AddService(int bookingId, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null && quantity > 0)
            {
                var service = new BookingService { BookingId = bookingId, ProductId = productId, Quantity = quantity, PriceAtOrder = product.UnitPrice, OrderDate = DateTime.Now };
                _context.Add(service);
                product.CurrentStock -= quantity;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        // 4. TRẢ PHÒNG (CHECKOUT VIEW)
        [HttpGet]
        public async Task<IActionResult> Checkout(int roomId)
        {
            if (HttpContext.Session.GetString("UserName") == null) return RedirectToAction("Login", "Account");

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room).ThenInclude(r => r.RoomType)
                .Include(b => b.BookingServices).ThenInclude(s => s.Product)
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefaultAsync(b => b.RoomId == roomId && b.BookingStatus == "Occupied");

            if (booking == null) return Content("Không tìm thấy đơn để trả phòng!");

            int days = (DateTime.Now - booking.CheckInDate).Days;
            if (days <= 0) days = 1;
            decimal roomFee = booking.Room.RoomType.PricePerNight * days;
            decimal serviceFee = booking.BookingServices.Sum(s => s.PriceAtOrder * s.Quantity);

            decimal otherFee = 0;
            if (booking.Notes != null && booking.Notes.Contains("XE ĐƯA ĐÓN")) otherFee = 200000;

            booking.TotalAmount = roomFee + serviceFee + otherFee;
            booking.FinalAmount = booking.TotalAmount;

            ViewBag.Days = days;
            ViewBag.RoomFee = roomFee;
            ViewBag.ServiceFee = serviceFee;
            ViewBag.OtherFee = otherFee;

            return View(booking);
        }

        // 5. XÁC NHẬN TRẢ PHÒNG
        [HttpPost]
        public async Task<IActionResult> ConfirmCheckout(int bookingId)
        {
            var booking = await _context.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking != null)
            {
                booking.BookingStatus = "Completed";
                booking.PaymentStatus = "Paid";
                booking.CheckOutDate = DateTime.Now;
                if (booking.Room != null) booking.Room.RoomStatus = "Cleaning";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        // 6. DỌN PHÒNG XONG (MỚI THÊM)
        // Chuyển trạng thái từ Cleaning (Vàng) -> Available (Xanh)
        public async Task<IActionResult> CleanRoom(int roomId)
        {
            if (HttpContext.Session.GetString("UserName") == null) return RedirectToAction("Login", "Account");

            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null && room.RoomStatus == "Cleaning")
            {
                room.RoomStatus = "Available"; // Phòng đã sẵn sàng đón khách mới
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }
        // 7. DANH SÁCH ĐƠN ĐẶT PHÒNG (Để xem lịch sử)
        public async Task<IActionResult> Index()
        {
            // Kiểm tra đăng nhập
            if (HttpContext.Session.GetString("UserName") == null) return RedirectToAction("Login", "Account");

            // Lấy danh sách booking, sắp xếp đơn mới nhất lên đầu
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }
    }
}
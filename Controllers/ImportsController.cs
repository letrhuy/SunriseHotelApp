using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

namespace SunriseHotelApp.Controllers
{
    public class ImportsController : Controller
    {
        private readonly HotelManagementDbContext _context;

        public ImportsController(HotelManagementDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách phiếu nhập
        public async Task<IActionResult> Index()
        {
            var imports = await _context.ImportReceipts
                .Include(i => i.Supplier)
                .Include(i => i.CreatedByUser)
                .OrderByDescending(i => i.ImportDate)
                .ToListAsync();
            return View(imports);
        }

        // 2. Trang tạo phiếu nhập mới (GET)
        public IActionResult Create()
        {
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
            ViewBag.Products = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // 3. Xử lý lưu phiếu nhập và cập nhật kho (POST)
        [HttpPost]
        public async Task<IActionResult> Create(int supplierId, int productId, int quantity, decimal importPrice)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 1;

            // A. Tạo phiếu nhập tổng (Receipt)
            var receipt = new ImportReceipt
            {
                SupplierId = supplierId,
                ImportDate = DateTime.Now,
                CreatedByUserId = userId,
                TotalAmount = quantity * importPrice
            };
            _context.ImportReceipts.Add(receipt);
            await _context.SaveChangesAsync();

            // B. Tạo chi tiết phiếu nhập (Detail)
            var detail = new ImportReceiptDetail
            {
                ReceiptId = receipt.ReceiptId,
                ProductId = productId,
                Quantity = quantity,
                ImportPrice = importPrice,
                SubTotal = quantity * importPrice
            };
            _context.ImportReceiptDetails.Add(detail);

            // C. QUAN TRỌNG: Cập nhật số lượng tồn kho trong bảng Products
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.CurrentStock += quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsBySupplier(int supplierId)
        {
            var products = await _context.Products
                .Where(p => p.SupplierID == supplierId)
                .Select(p => new
                {
                    productId = p.ProductId,
                    productName = p.ProductName + " (Tồn: " + p.CurrentStock + ")"
                })
                .ToListAsync();

            return Json(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var importReceipt = await _context.ImportReceipts
                .Include(i => i.Supplier)
                .Include(i => i.CreatedByUser)
                .Include(i => i.ImportReceiptDetails)
                    .ThenInclude(d => d.Product) 
                .FirstOrDefaultAsync(m => m.ReceiptId == id);

            if (importReceipt == null)
            {
                return NotFound(); 
            }

            return View(importReceipt);
        }
    }
}
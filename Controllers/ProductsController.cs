using Microsoft.AspNetCore.Mvc;
using SunriseHotelApp.Models;
using Microsoft.EntityFrameworkCore;

public class ProductsController : Controller
{
    private readonly HotelManagementDbContext _context;
    public ProductsController(HotelManagementDbContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }
}
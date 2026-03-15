using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunriseHotelApp.Models;

public class ProductsController : Controller
{
    private readonly HotelManagementDbContext _context;

    public ProductsController(HotelManagementDbContext context)
    { _context = context; }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }
}
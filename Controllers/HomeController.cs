using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;
using OnlineStoreMVC.Helpers;


namespace OnlineStoreMVC.Controllers;

public class HomeController : Controller
{
  private readonly OnlineStoreDBContext _context;
  private readonly ILogger<HomeController> _logger;

  public HomeController(OnlineStoreDBContext context, ILogger<HomeController> logger)
  {
    _logger = logger;
    _context = context;
  }

  public IActionResult Index()
  {
    var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
    ViewBag.CartCount = cart.Sum(c => c.Quantity);

    // 1. Featured products
    var featuredProducts = _context.Products
        .Where(p => p.IsFeatured)
        .OrderByDescending(p => p.ProductID)
        .Include(p => p.ProductImages)
        .Include(p => p.Category)
        .Take(4)
        .ToList();

    // 2. Promotion products
    var promotions = _context.Promotions
        .Where(p => p.StartDate <= DateTime.Now &&
                    (p.EndDate == null || p.EndDate >= DateTime.Now))
        .Include(p => p.Product)
            .ThenInclude(p => p.ProductImages)
        .OrderByDescending(p => p.PromotionID)
        .ToList();

    ViewBag.FeaturedProducts = featuredProducts;
    ViewBag.PromotionProducts = promotions.Take(4).ToList();
    ViewBag.Promotions = promotions;

    return View();
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

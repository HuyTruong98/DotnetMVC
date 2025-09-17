using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Controllers
{
  public class HomeProductController : Controller
  {
    private readonly OnlineStoreDBContext _context;

    public HomeProductController(OnlineStoreDBContext context)
    {
      _context = context;
    }

    public IActionResult ByCategory(string categoryIds)
    {
      if (string.IsNullOrEmpty(categoryIds))
        return View("~/Views/Home/Products/ByCategory.cshtml", new List<Product>());

      var ids = categoryIds
          .Split(',')
          .Select(id => int.TryParse(id, out var i) ? i : 0)
          .Where(i => i > 0)
          .ToList();

      var products = _context.Products
          .Where(p => ids.Contains(p.CategoryID))
          .Include(p => p.Category)
          .Include(p => p.ProductImages)
          .Include(p => p.Promotions)
          .ToList();

      var promotions = _context.Promotions
          .Where(p => p.StartDate <= DateTime.Now &&
                     (p.EndDate == null || p.EndDate >= DateTime.Now))
          .ToList();
      ViewBag.Promotions = promotions;

      return View("~/Views/Home/Products/ByCategory.cshtml", products);
    }

    [HttpGet("Details/{id}")]
    public IActionResult Detail(int id)
    {
      var product = _context.Products
        .Include(p => p.Category)
        .Include(p => p.ProductImages)
        .Include(p => p.Promotions)
        .Include(p => p.Variants)
        .FirstOrDefault(p => p.ProductID == id);

      if (product == null)
        return NotFound();

      var promo = product.Promotions
          .FirstOrDefault(x =>
              x.StartDate <= DateTime.Now &&
              (x.EndDate == null || x.EndDate >= DateTime.Now));

      ViewBag.ActivePromotion = promo;

      ViewBag.Colors = product.Variants
          .Select(v => v.Color)
          .Distinct()
          .ToList();

      return View("~/Views/Home/Products/Detail.cshtml", product);
    }
  }
}
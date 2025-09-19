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
    public IActionResult Detail(int id, string? selectedColor = null)
    {
      var product = _context.Products
          .Include(p => p.Category)
          .Include(p => p.ProductImages)
          .Include(p => p.Promotions)
          .Include(p => p.Variants)
          .FirstOrDefault(p => p.ProductID == id);

      if (product == null) return NotFound();

      var colors = product.Variants.Select(v => v.Color).Distinct().ToList();
      ViewBag.Colors = colors;

      selectedColor ??= colors.FirstOrDefault();
      ViewBag.SelectedColor = selectedColor;

      var sizeRank = new Dictionary<string, int>
      {
        ["XS"] = 0,
        ["S"] = 1,
        ["M"] = 2,
        ["L"] = 3,
        ["XL"] = 4,
        ["XXL"] = 5
      };

      ViewBag.Sizes = product.Variants
        .Where(v => v.Color == selectedColor)
        .ToList()
        .OrderBy(v => sizeRank.ContainsKey(v.Size)
            ? sizeRank[v.Size]
            : int.MaxValue)
        .ToList();

      var activePromotion = product.Promotions
          .FirstOrDefault(p => p.StartDate <= DateTime.Now &&
                              (p.EndDate == null || p.EndDate >= DateTime.Now));

      ViewBag.ActivePromotion = activePromotion;

      return View("~/Views/Home/Products/Detail.cshtml", product);
    }
  }
}
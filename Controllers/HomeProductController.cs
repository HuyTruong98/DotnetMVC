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
          .ToList();

      return View("~/Views/Home/Products/ByCategory.cshtml", products);
    }
    public IActionResult Detail(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                 .Include(p => p.ProductImages)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            return View("~/Views/Home/Products/Detail.cshtml", product);
        }
    }
}
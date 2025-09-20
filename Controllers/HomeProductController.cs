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

    public IActionResult ByCategory(
     [FromQuery] string categoryIds,
      string search = "",
      string sort = "default",
      int page = 1,
      int pageSize = 8)
    {
      // 1. Promotions (active)
      var promotions = _context.Promotions
          .Where(p => p.StartDate <= DateTime.Now &&
                     (p.EndDate == null || p.EndDate >= DateTime.Now))
          .ToList();
      ViewBag.Promotions = promotions;

      // 2. Base query sản phẩm (Include để lấy quan hệ)
      var query = _context.Products
          .Include(p => p.Category)
          .Include(p => p.ProductImages)
          .Include(p => p.Promotions)
          .AsQueryable();

      // 3. Filter theo category
      List<int> ids = new();
      if (!string.IsNullOrEmpty(categoryIds))
      {
        ids = categoryIds.Split(',')
                         .Select(id => int.TryParse(id, out var x) ? x : 0)
                         .Where(x => x > 0)
                         .ToList();

        if (ids.Any())
        {
          query = query.Where(p => ids.Contains(p.CategoryID));
        }
      }

      // 4. Search theo tên
      if (!string.IsNullOrWhiteSpace(search))
      {
        query = query.Where(p => p.ProductName.Contains(search));
      }

      // 5. Sort
      query = sort switch
      {
        "asc" => query.OrderBy(p => p.Price),
        "desc" => query.OrderByDescending(p => p.Price),
        _ => query.OrderBy(p => p.ProductName)
      };

      // 6. Pagination
      int totalCount = query.Count();
      var products = query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      // 7. Truyền dữ liệu sang View
      ViewBag.TotalCount = totalCount;
      ViewBag.Page = page;
      ViewBag.PageSize = pageSize;
      ViewBag.CategoryIds = ids;
      ViewBag.Search = search;
      ViewBag.Sort = sort;
      ViewData["Title"] = "Sản phẩm theo danh mục";

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

      // Lấy danh sách ID sản phẩm đã xem từ cookie
      var viewedCookie = Request.Cookies["RecentlyViewed"];
      List<int> viewedIds = string.IsNullOrEmpty(viewedCookie)
          ? new List<int>()
          : viewedCookie.Split(',').Select(int.Parse).ToList();

      // Nếu sản phẩm chưa có thì thêm vào
      if (!viewedIds.Contains(id))
      {
        viewedIds.Insert(0, id); // chèn đầu danh sách
        if (viewedIds.Count > 10) // chỉ giữ tối đa 10 sản phẩm
          viewedIds = viewedIds.Take(10).ToList();
      }

      Response.Cookies.Append("RecentlyViewed", string.Join(",", viewedIds),
        new CookieOptions { Expires = DateTime.Now.AddDays(7) });

      var recentlyViewed = _context.Products
     .Include(p => p.ProductImages)
     .Include(p => p.Category)
     .Include(p => p.Promotions)
     .Where(p => viewedIds.Contains(p.ProductID))
     .ToList();

      // Sắp xếp đúng thứ tự cookie
      recentlyViewed = viewedIds
          .Select(vid => recentlyViewed.FirstOrDefault(p => p.ProductID == vid))
          .Where(p => p != null)
          .ToList();

      ViewBag.RecentlyViewed = recentlyViewed;

      var promotions = _context.Promotions
      .Where(p => p.StartDate <= DateTime.Now &&
                  (p.EndDate == null || p.EndDate >= DateTime.Now))
      .Include(p => p.Product)
          .ThenInclude(pr => pr.ProductImages)
      .OrderByDescending(p => p.PromotionID)
      .Take(4)
      .ToList();

      ViewBag.Promotions = promotions;

      return View("~/Views/Home/Products/Detail.cshtml", product);
    }

    public IActionResult PromotionProducts(
             string search = "",
             string sort = "default",
             int page = 1,
             int pageSize = 8)
    {
      var now = DateTime.Now;

      // 1. Promotions active
      var promotions = _context.Promotions
          .Where(p => p.StartDate <= now &&
                     (p.EndDate == null || p.EndDate >= now))
          .ToList();
      ViewBag.Promotions = promotions;

      // 2. Query sản phẩm có promotion
      var query = _context.Products
          .Include(p => p.Category)
          .Include(p => p.ProductImages)
          .Include(p => p.Promotions)
          .Where(p => p.Promotions.Any(pr =>
              pr.StartDate <= now &&
              (pr.EndDate == null || pr.EndDate >= now)))
          .AsQueryable();

      // 3. Search
      if (!string.IsNullOrWhiteSpace(search))
      {
        query = query.Where(p => p.ProductName.Contains(search));
      }

      // 4. Sort
      query = sort switch
      {
        "asc" => query.OrderBy(p => p.Price),
        "desc" => query.OrderByDescending(p => p.Price),
        _ => query.OrderBy(p => p.ProductName)
      };

      // 5. Pagination
      int totalCount = query.Count();
      var products = query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      // 6. Data for View
      ViewBag.TotalCount = totalCount;
      ViewBag.Page = page;
      ViewBag.PageSize = pageSize;
      ViewBag.CategoryId = null; // Outlet không lọc theo category
      ViewBag.Search = search;
      ViewBag.Sort = sort;
      ViewBag.IsOutlet = true;
      ViewData["Title"] = "Sản phẩm khuyến mãi (OUTLET)";

      return View("~/Views/Home/Products/ByCategory.cshtml", products);
    }

    public IActionResult All(
     string search = "",
     string sort = "default",
     int page = 1,
     int pageSize = 8)
    {
      var now = DateTime.Now;

      // 1. Promotions active
      var promotions = _context.Promotions
          .Where(p => p.StartDate <= now &&
                     (p.EndDate == null || p.EndDate >= now))
          .ToList();
      ViewBag.Promotions = promotions;

      // 2. Base query: tất cả sản phẩm
      var query = _context.Products
          .Include(p => p.Category)
          .Include(p => p.ProductImages)
          .Include(p => p.Promotions)
          .AsQueryable();

      // 3. Search
      if (!string.IsNullOrWhiteSpace(search))
      {
        query = query.Where(p => p.ProductName.Contains(search));
      }

      // 4. Sort
      query = sort switch
      {
        "asc" => query.OrderBy(p => p.Price),
        "desc" => query.OrderByDescending(p => p.Price),
        _ => query.OrderBy(p => p.ProductName)
      };

      // 5. Pagination
      int totalCount = query.Count();
      var products = query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      // 6. Data for View
      ViewBag.TotalCount = totalCount;
      ViewBag.Page = page;
      ViewBag.PageSize = pageSize;
      ViewBag.Search = search;
      ViewBag.Sort = sort;
      ViewData["Title"] = "Tất cả sản phẩm";

      return View("~/Views/Home/Products/All.cshtml", products);
    }

  }
}
using Microsoft.AspNetCore.Mvc;
using OnlineStoreMVC.Models;
using OnlineStoreMVC.Helpers;
using OnlineStoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Models.ViewModels;

namespace OnlineStoreMVC.Controllers
{
  public class HomeCartController : Controller
  {
    private readonly OnlineStoreDBContext _context;
    private readonly ILogger<HomeCartController> _logger;
    public HomeCartController(OnlineStoreDBContext context, ILogger<HomeCartController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [HttpGet, Route("Cart")]
    public async Task<IActionResult> Index()
    {
      var userName = HttpContext?.Session?.GetString("Username");
      if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login", "Auth");

      // 1. Lấy cart từ session
      var sessionCart = HttpContext.Session
          .GetObject<List<CartItem>>("Cart")
          ?? new List<CartItem>();
      _logger.LogInformation("Vào Index, SessionCart có {count} items", sessionCart?.Count ?? 0);

      // 2. Lấy danh sách VariantID đang có trong giỏ
      var variantIds = sessionCart.Select(ci => ci.VariantID).ToList();

      // 3. Query ProductVariant kèm Product + ProductImages
      var variants = await _context.ProductVariants
          .Include(v => v.Product)
          .ThenInclude(p => p.ProductImages)
          .Where(v => variantIds.Contains(v.VariantID))
          .ToListAsync();

      // 4. Map sang ViewModel để render
      var model = new CartViewModel();
      foreach (var ci in sessionCart)
      {
        var v = variants.FirstOrDefault(x => x.VariantID == ci.VariantID);
        if (v == null) continue;

        var imageUrl = v.Product.ProductImages
            .FirstOrDefault()?.ImageURL
            ?? "/images/no-image.png";

        model.Items.Add(new CartLineItem
        {
          VariantID = v.VariantID,
          ProductName = v.Product.ProductName,
          Size = v.Size,
          Color = v.Color,
          UnitPrice = v.Product.Price,
          Quantity = ci.Quantity,
          ImageUrl = imageUrl
        });
      }

      return View("~/Views/Home/Carts/Index.cshtml", model);
    }

    [HttpPost]
    public IActionResult AddToCart(int variantId, int quantity = 1)
    {
      _logger.LogInformation("Thêm variantId={variantId} vào Cart", variantId);

      var cart = HttpContext.Session
          .GetObject<List<CartItem>>("Cart")
          ?? new List<CartItem>();

      _logger.LogInformation("SessionCart trước khi thêm có {count} items", cart.Count);

      var item = cart.FirstOrDefault(c => c.VariantID == variantId);
      if (item != null)
        item.Quantity += quantity;
      else
        cart.Add(new CartItem { VariantID = variantId, Quantity = quantity });

      HttpContext.Session.SetObject("Cart", cart);

      TempData["Success"] = "Thêm vào giỏ hàng thành công.";
      return Json(new
      {
        success = true,
        cartCount = cart.Sum(c => c.Quantity)
      });
    }
  }
}
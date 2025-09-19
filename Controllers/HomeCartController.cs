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

      // 3. Query ProductVariant kèm Product + ProductImages + Promotions
      var variants = await _context.ProductVariants
          .Include(v => v.Product)
              .ThenInclude(p => p.ProductImages)
          .Include(v => v.Product)
              .ThenInclude(p => p.Promotions)
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

        // tìm promotion còn hiệu lực
        var promo = v.Product.Promotions
            .FirstOrDefault(p => p.StartDate <= DateTime.Now &&
                                (p.EndDate == null || p.EndDate >= DateTime.Now));

        decimal? promoPrice = null;
        if (promo != null)
        {
          promoPrice = Math.Round(v.Product.Price * (1 - promo.DiscountPercent / 100m), 0);
        }

        model.Items.Add(new CartLineItem
        {
          VariantID = v.VariantID,
          ProductName = v.Product.ProductName,
          Size = v.Size,
          Color = v.Color,
          UnitPrice = v.Product.Price,
          PromotionPrice = promoPrice,
          Quantity = ci.Quantity,
          ImageUrl = imageUrl
        });
      }

      var user = await _context.Users
        .Where(u => u.Username == userName)
        .Select(u => new
        {
          u.UserID,
          u.Username,
          u.FullName,
          u.Email,
          u.Phone,
          u.Address,
          u.Role,
          u.CreatedAt
        })
        .FirstOrDefaultAsync();

      ViewBag.User = user;

      return View("~/Views/Home/Carts/Index.cshtml", model);
    }

    [HttpPost]
    public IActionResult AddToCart(int ProductID, int VariantID, int Quantity = 1, string SelectedColor = null)
    {
      var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

      var existing = cart.FirstOrDefault(c => c.VariantID == VariantID);
      if (existing != null) existing.Quantity += Quantity;
      else cart.Add(new CartItem { VariantID = VariantID, Quantity = Quantity });

      HttpContext.Session.SetObject("Cart", cart);

      TempData["Success"] = "Thêm vào giỏ hàng thành công.";

      return RedirectToAction("Detail", "HomeProduct", new { id = ProductID, selectedColor = SelectedColor });
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int variantId, int quantity)
    {
      var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
      var item = cart.FirstOrDefault(c => c.VariantID == variantId);
      if (item != null)
      {
        item.Quantity = quantity > 0 ? quantity : 1;
      }
      HttpContext.Session.SetObject("Cart", cart);

      return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RemoveItem(int variantId)
    {
      var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
      cart.RemoveAll(c => c.VariantID == variantId);
      HttpContext.Session.SetObject("Cart", cart);

      return RedirectToAction("Index");
    }

    [HttpPost, Route("Cart/Checkout")]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
      var username = HttpContext.Session.GetString("Username");
      if (string.IsNullOrEmpty(username))
        return RedirectToAction("Login", "Auth");

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
      if (user == null)
      {
        TempData["Error"] = "Không tìm thấy tài khoản.";
        return RedirectToAction("Index");
      }

      var sessionCart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
      if (!sessionCart.Any())
      {
        TempData["Error"] = "Giỏ hàng đang trống.";
        return RedirectToAction("Index");
      }

      var variantIds = sessionCart.Select(ci => ci.VariantID).ToList();
      var variants = await _context.ProductVariants
          .Include(v => v.Product)
          .Where(v => variantIds.Contains(v.VariantID))
          .ToListAsync();

      var order = new Order
      {
        UserID = user.UserID,
        Status = "Pending",
        OrderDate = DateTime.Now,
        TotalAmount = 0m,
        Description = model.Description ?? "Đặt hàng từ website",
        OrderDetails = new List<OrderDetail>()
      };

      decimal total = 0m;

      foreach (var ci in sessionCart)
      {
        var variant = variants.FirstOrDefault(v => v.VariantID == ci.VariantID);
        if (variant == null) continue;

        if (variant.Stock < ci.Quantity)
        {
          TempData["Error"] = $"Không đủ tồn kho cho {variant.Product.ProductName} ({variant.Size} - {variant.Color}). Chỉ còn {variant.Stock}.";
          return RedirectToAction("Index");
        }

        variant.Stock -= ci.Quantity;
        _context.ProductVariants.Update(variant);

        var activePromo = await _context.Promotions
            .FirstOrDefaultAsync(p => p.ProductID == variant.ProductID &&
                                      p.StartDate <= DateTime.Now &&
                                      (p.EndDate == null || p.EndDate >= DateTime.Now));

        decimal unitPrice = variant.Product.Price;
        if (activePromo != null)
        {
          unitPrice = Math.Round(unitPrice * (1 - activePromo.DiscountPercent / 100m), 0);
        }

        var detail = new OrderDetail
        {
          VariantID = variant.VariantID,
          Quantity = ci.Quantity,
          UnitPrice = unitPrice
        };

        total += detail.Quantity * detail.UnitPrice;
        order.OrderDetails.Add(detail);
      }

      decimal shippingFee = total < 300000 ? 20000 : 0;
      order.TotalAmount = total + shippingFee;

      _context.Orders.Add(order);
      await _context.SaveChangesAsync();

      HttpContext.Session.Remove("Cart");

      TempData["Success"] = "Đặt hàng thành công!";
      return View("~/Views/Home/Carts/OrderSuccess.cshtml", new { orderId = order.OrderID });
    }

  }
}
using Microsoft.AspNetCore.Mvc;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;
using OnlineStoreMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace OnlineStoreMVC.Controllers
{
  [Route("Dashboard/Products")]
  public class DashboardProductController : Controller
  {
    private readonly OnlineStoreDBContext _context;
    private readonly ILogger<DashboardProductController> _logger;

    public DashboardProductController(OnlineStoreDBContext context, ILogger<DashboardProductController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin")
        return RedirectToAction("Login", "Auth");

      var products = _context.Products
          .Include(p => p.Category)
          .Include(p => p.ProductImages)
          .Include(p => p.Variants)
          .ToList();

      var promotions = _context.Promotions
          .Where(p => p.StartDate <= DateTime.Now &&
                     (p.EndDate == null || p.EndDate >= DateTime.Now))
          .ToList();
      ViewBag.Promotions = promotions;

      return View("~/Views/Dashboard/Products/Index.cshtml", products);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin")
        return RedirectToAction("Login", "Auth");

      ViewBag.Categories = _context.Categories
          .Select(c => new SelectListItem
          {
            Value = c.CategoryID.ToString(),
            Text = c.CategoryName
          }).ToList();

      var model = new ProductViewModel();
      model.Variants.Add(new ProductVariantViewModel());

      return View("~/Views/Dashboard/Products/CreateProduct.cshtml", model);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin")
        return RedirectToAction("Login", "Auth");

      ViewBag.Categories = _context.Categories
      .Select(c => new SelectListItem
      {
        Value = c.CategoryID.ToString(),
        Text = c.CategoryName
      }).ToList();

      if (!ModelState.IsValid)
      {
        return View("~/Views/Dashboard/Products/CreateProduct.cshtml", model);
      }

      var product = new Product
      {
        ProductName = model.ProductName,
        Description = model.Description,
        Price = model.Price ?? 0,
        Status = model.Status,
        IsFeatured = model.IsFeatured,
        CategoryID = model.CategoryID ?? 0,
        CreatedAt = DateTime.Now
      };

      product.ProductImages = new List<ProductImage>();
      product.Variants = new List<ProductVariant>();

      if (model.Images != null && model.Images.Any())
      {
        var uploads = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot", "uploads", "products"
        );
        if (!Directory.Exists(uploads))
          Directory.CreateDirectory(uploads);

        for (int i = 0; i < model.Images.Count; i++)
        {
          var file = model.Images[i];
          var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
          var fullPath = Path.Combine(uploads, fileName);

          using var stream = new FileStream(fullPath, FileMode.Create);
          await file.CopyToAsync(stream);

          product.ProductImages.Add(new ProductImage
          {
            ImageURL = "/uploads/products/" + fileName,
            IsPrimary = (i == 0)
          });
        }
      }

      foreach (var v in model.Variants)
      {
        product.Variants.Add(new ProductVariant
        {
          Size = v.Size,
          Color = v.Color,
          Stock = v.Stock
        });
      }

      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      TempData["Success"] = "Tạo sản phẩm thành công!";
      return RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin")
        return RedirectToAction("Login", "Auth");

      var product = await _context.Products
         .Include(p => p.ProductImages)
         .Include(p => p.Variants)
             .ThenInclude(v => v.OrderDetails)
         .FirstOrDefaultAsync(p => p.ProductID == id);

      if (product == null)
        return NotFound();

      var protectedVariantIds = product.Variants
           .Where(v => v.OrderDetails.Any())
           .Select(v => v.VariantID)
           .ToList();

      var model = new ProductViewModel
      {
        ProductID = product.ProductID,
        ProductName = product.ProductName,
        Description = product.Description,
        Price = product.Price,
        CategoryID = product.CategoryID,
        Status = product.Status,
        IsFeatured = product.IsFeatured,
        CreatedAt = product.CreatedAt ?? DateTime.Now,
        ProtectedVariantIds = protectedVariantIds
      };

      foreach (var v in product.Variants)
      {
        model.Variants.Add(new ProductVariantViewModel
        {
          VariantID = v.VariantID,
          Size = v.Size,
          Color = v.Color,
          Stock = v.Stock
        });
      }


      PopulateViewBags(id, model);
      return View("~/Views/Dashboard/Products/EditProduct.cshtml", model);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, ProductViewModel model)
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin")
        return RedirectToAction("Login", "Auth");

      if (!ModelState.IsValid)
      {
        PopulateViewBags(id, model);
        return View("~/Views/Dashboard/Products/EditProduct.cshtml", model);
      }

      var product = await _context.Products
          .Include(p => p.ProductImages)
          .Include(p => p.Variants)
              .ThenInclude(v => v.OrderDetails)
          .FirstOrDefaultAsync(p => p.ProductID == id);

      if (product == null) return NotFound();

      var oldVariants = product.Variants.ToList();
      var newVariants = model.Variants ?? new List<ProductVariantViewModel>();

      // 1. Update và thêm mới
      foreach (var nv in newVariants)
      {
        if (nv.VariantID > 0)
        {
          // Tìm trong DB
          var existing = oldVariants.FirstOrDefault(v => v.VariantID == nv.VariantID);
          if (existing != null)
          {
            // Cập nhật tồn kho
            existing.Stock = nv.Stock;
            existing.Size = nv.Size;
            existing.Color = nv.Color;
          }
        }
        else
        {
          // Thêm mới
          product.Variants.Add(new ProductVariant
          {
            Size = nv.Size,
            Color = nv.Color,
            Stock = nv.Stock
          });
        }
      }

      // 2. Xóa những cái không còn trong model gửi lên
      foreach (var ov in oldVariants)
      {
        if (!newVariants.Any(v => v.VariantID == ov.VariantID))
        {
          _context.ProductVariants.Remove(ov);
        }
      }

      product.ProductName = model.ProductName;
      product.Description = model.Description;
      product.Price = model.Price ?? product.Price;
      product.CategoryID = model.CategoryID ?? product.CategoryID;
      product.Status = model.Status;
      product.IsFeatured = model.IsFeatured;

      // ==========================
      // ✅ Xử lý hình ảnh
      // (nếu không cần thì bạn có thể bỏ qua cả đoạn này)
      // ==========================
      if (model.Images?.Count > 0)
      {
        // Xóa ảnh cũ trong thư mục
        foreach (var oldImage in product.ProductImages)
        {
          var oldPath = Path.Combine(
              Directory.GetCurrentDirectory(),
              "wwwroot",
              oldImage.ImageURL.TrimStart('/')
          );
          if (System.IO.File.Exists(oldPath))
            System.IO.File.Delete(oldPath);
        }

        _context.ProductImages.RemoveRange(product.ProductImages);
        product.ProductImages.Clear();

        var uploads = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot", "uploads", "products");
        if (!Directory.Exists(uploads))
          Directory.CreateDirectory(uploads);

        for (int i = 0; i < model.Images.Count; i++)
        {
          var file = model.Images[i];
          var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
          var fullPath = Path.Combine(uploads, fileName);

          using var stream = new FileStream(fullPath, FileMode.Create);
          await file.CopyToAsync(stream);

          product.ProductImages.Add(new ProductImage
          {
            ImageURL = "/uploads/products/" + fileName,
            IsPrimary = (i == 0)
          });
        }
      }

      await _context.SaveChangesAsync();
      TempData["Success"] = "Cập nhật sản phẩm thành công!";
      return RedirectToAction("Index");
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      if (HttpContext.Session.GetString("Role")?.ToLower() != "admin")
        return RedirectToAction("Login", "Auth");

      // Load product with all variants, order details, images, promotions
      var product = await _context.Products
          .Include(p => p.Variants)
            .ThenInclude(v => v.OrderDetails)
          .Include(p => p.ProductImages)
          .Include(p => p.Promotions)
          .FirstOrDefaultAsync(p => p.ProductID == id);

      if (product == null)
      {
        TempData["Error"] = "Không tìm thấy sản phẩm.";
        return RedirectToAction("Index");
      }

      _logger.LogInformation("abc", product);

      bool hasOrder = product.Variants
          .SelectMany(v => v.OrderDetails)
          .Any();

      if (hasOrder)
      {
        TempData["Error"] = "Không thể xóa sản phẩm này vì đã có đơn hàng sử dụng.";
        return RedirectToAction("Index");
      }


      _context.ProductVariants.RemoveRange(product.Variants);
      _context.Products.Remove(product);
      await _context.SaveChangesAsync();

      TempData["Success"] = "Xóa sản phẩm thành công.";
      return RedirectToAction("Index");
    }

    private void PopulateViewBags(int productId, ProductViewModel model, IEnumerable<int>? protectedIds = null)
    {
      ViewBag.Categories = _context.Categories
          .Select(c => new SelectListItem
          {
            Value = c.CategoryID.ToString(),
            Text = c.CategoryName
          })
          .ToList();

      ViewBag.ExistingImages = _context.ProductImages
          .Where(img => img.ProductID == productId)
          .ToList();

      if (protectedIds != null)
        model.ProtectedVariantIds = protectedIds.ToList();
    }
  }
}
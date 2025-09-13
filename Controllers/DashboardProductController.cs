using Microsoft.AspNetCore.Mvc;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;
using OnlineStoreMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var products = _context.Products
        .Include(p => p.Category)
        .Include(p => p.ProductImages)
        .ToList();



      return View("~/Views/Dashboard/Products/Index.cshtml", products);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var categories = _context.Categories
                      .Select(c => new SelectListItem
                      {
                        Value = c.CategoryID.ToString(),
                        Text = c.CategoryName
                      }).ToList();

      ViewBag.Categories = categories;

      return View("~/Views/Dashboard/Products/CreateProduct.cshtml");
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.Categories = _context.Categories
        .Select(c => new SelectListItem
        {
          Value = c.CategoryID.ToString(),
          Text = c.CategoryName
        }).ToList();

        return View("~/Views/Dashboard/Products/CreateProduct.cshtml", model);
      }

      var product = new Product
      {
        ProductName = model.ProductName,
        Description = model.Description,
        Price = model.Price.Value,
        Stock = model.Stock.Value,
        CategoryID = model.CategoryID.Value,
        Status = model.Status,
        CreatedAt = DateTime.Now,
        IsFeatured = model.IsFeatured
      };

      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      if (model.Images != null)
      {
        for (int i = 0; i < model.Images.Count; i++)
        {
          var file = model.Images[i];
          var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
          var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products", fileName);

          using (var stream = new FileStream(path, FileMode.Create))
          {
            await file.CopyToAsync(stream);
          }

          var image = new ProductImage
          {
            ProductID = product.ProductID,
            ImageURL = "/uploads/products/" + fileName,
            IsPrimary = (i == 0)
          };

          _context.ProductImages.Add(image);
        }

        await _context.SaveChangesAsync();
      }

      TempData["Success"] = "Tạo sản phẩm thành công.";
      return RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var product = _context.Products
          .Include(p => p.ProductImages)
          .FirstOrDefault(p => p.ProductID == id);

      if (product == null) return NotFound();

      var model = new ProductViewModel
      {
        ProductID = product.ProductID,
        ProductName = product.ProductName,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        CategoryID = product.CategoryID,
        Status = product.Status,
        IsFeatured = product.IsFeatured
      };

      ViewBag.Categories = _context.Categories
          .Select(c => new SelectListItem
          {
            Value = c.CategoryID.ToString(),
            Text = c.CategoryName
          }).ToList();

      ViewBag.ExistingImages = product.ProductImages ?? new List<ProductImage>();

      return View("~/Views/Dashboard/Products/EditProduct.cshtml", model);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, ProductViewModel model)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var product = await _context.Products
             .Include(p => p.ProductImages)
             .FirstOrDefaultAsync(p => p.ProductID == id);

      if (product == null) return NotFound();

      ViewBag.Categories = _context.Categories
        .Select(c => new SelectListItem
        {
          Value = c.CategoryID.ToString(),
          Text = c.CategoryName
        }).ToList();

      ViewBag.ExistingImages = product.ProductImages ?? new List<ProductImage>();

      if (!ModelState.IsValid)
      {
        return View("~/Views/Dashboard/Products/EditProduct.cshtml", model);
      }

      product.ProductName = model.ProductName;
      product.Description = model.Description;
      product.Price = model.Price ?? product.Price;
      product.Stock = model.Stock ?? product.Stock;
      product.CategoryID = model.CategoryID ?? product.CategoryID;
      product.Status = model.Status;
      product.IsFeatured = model.IsFeatured;


      if (model.Images != null && model.Images.Count > 0)
      {
        foreach (var oldImage in product.ProductImages)
        {
          var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImageURL.TrimStart('/'));
          if (System.IO.File.Exists(oldPath))
            System.IO.File.Delete(oldPath);
        }

        _context.ProductImages.RemoveRange(product.ProductImages);

        for (int i = 0; i < model.Images.Count; i++)
        {
          var file = model.Images[i];
          var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
          var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products", fileName);

          using (var stream = new FileStream(path, FileMode.Create))
          {
            await file.CopyToAsync(stream);
          }

          var image = new ProductImage
          {
            ProductID = product.ProductID,
            ImageURL = "/uploads/products/" + fileName,
            IsPrimary = (i == 0)
          };

          _context.ProductImages.Add(image);
        }
      }

      await _context.SaveChangesAsync();

      TempData["Success"] = "Cập nhật sản phẩm thành công.";
      return RedirectToAction("Index");
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var product = await _context.Products
          .Include(p => p.ProductImages)
          .FirstOrDefaultAsync(p => p.ProductID == id);

      if (product == null)
      {
        TempData["Error"] = "Không tìm thấy sản phẩm.";
        return RedirectToAction("Index");
      }

      foreach (var image in product.ProductImages)
      {
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageURL.TrimStart('/'));
        if (System.IO.File.Exists(imagePath))
        {
          System.IO.File.Delete(imagePath);
        }
      }

      _context.ProductImages.RemoveRange(product.ProductImages);

      _context.Products.Remove(product);

      await _context.SaveChangesAsync();

      TempData["Success"] = "Xóa sản phẩm thành công.";
      return RedirectToAction("Index");
    }

    [HttpGet("Details/{id}")]
    public IActionResult Details(int id)
    {
      if (id <= 0)
      {
        return BadRequest("ID sản phẩm không hợp lệ.");
      }

      var product = _context.Products
          .Include(p => p.Category)
          .Include(p => p.ProductImages)
          .FirstOrDefault(p => p.ProductID == id);

      if (product == null)
      {
        return NotFound("Không tìm thấy sản phẩm.");
      }

      return View("~/Views/Dashboard/Products/Details.cshtml", product);
    }

        [HttpGet("ByCategory/{categoryId}")]
        public IActionResult ByCategory(int categoryId)
        {
            var products = _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            if (!products.Any())
            {
                return NotFound();
            }

            return View("~/Views/Products/ByCategory.cshtml", products);
        }

    }
}
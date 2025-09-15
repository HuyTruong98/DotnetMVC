using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Controllers
{
  [Route("Dashboard/Promotions")]
  public class DashboardPromotionController : Controller
  {
    private readonly OnlineStoreDBContext _context;

    public DashboardPromotionController(OnlineStoreDBContext context)
    {
      _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var promotions = _context.Promotions
          .Include(p => p.Product)
          .ToList();

      return View("~/Views/Dashboard/Promotions/Index.cshtml", promotions);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      ViewBag.ProductList = new SelectList(_context.Products, "ProductID", "ProductName");

      var model = new PromotionViewModel
      {
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(7)
      };

      return View("~/Views/Dashboard/Promotions/CreatePromotions.cshtml", model);
    }

    [HttpPost("Create")]
    public IActionResult Create(PromotionViewModel model)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      if (model.StartDate >= model.EndDate)
      {
        ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
      }

      if (ModelState.IsValid)
      {
        var promotion = new Promotion
        {
          ProductID = model.ProductID,
          DiscountPercent = model.DiscountPercent,
          StartDate = model.StartDate,
          EndDate = model.EndDate
        };

        _context.Promotions.Add(promotion);
        _context.SaveChanges();

        TempData["Success"] = "Tạo mới thành công.";
        return RedirectToAction("Index");
      }

      ViewBag.ProductList = new SelectList(_context.Products, "ProductID", "ProductName", model.ProductID);
      return View("~/Views/Dashboard/Promotions/CreatePromotions.cshtml", model);
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var promo = _context.Promotions.Include(p => p.Product).FirstOrDefault(p => p.PromotionID == id);
      if (promo == null) return NotFound();

      var model = new PromotionViewModel
      {
        PromotionID = promo.PromotionID,
        ProductID = promo.ProductID,
        DiscountPercent = promo.DiscountPercent,
        StartDate = promo.StartDate,
        EndDate = promo.EndDate ?? DateTime.Now.AddDays(7)
      };

      ViewBag.ProductList = new SelectList(_context.Products, "ProductID", "ProductName", promo.ProductID);
      return View("~/Views/Dashboard/Promotions/EditPromotions.cshtml", model);
    }

    [HttpPost("Edit/{id}")]
    public IActionResult Edit(int id, PromotionViewModel model)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      if (model.StartDate >= model.EndDate)
      {
        ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
      }

      if (!ModelState.IsValid)
      {
        ViewBag.ProductList = new SelectList(_context.Products, "ProductID", "ProductName", model.ProductID);
        return View("~/Views/Dashboard/Promotions/EditPromotions.cshtml", model);
      }

      var promo = _context.Promotions.FirstOrDefault(p => p.PromotionID == id);
      if (promo == null) return NotFound();

      promo.ProductID = model.ProductID;
      promo.DiscountPercent = model.DiscountPercent;
      promo.StartDate = model.StartDate;
      promo.EndDate = model.EndDate;

      _context.SaveChanges();
      TempData["Success"] = "Cập nhật thành công.";
      return RedirectToAction("Index");
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var promo = _context.Promotions.FirstOrDefault(p => p.PromotionID == id);
      if (promo == null) return NotFound();

      _context.Promotions.Remove(promo);
      _context.SaveChanges();

      TempData["Success"] = "Xóa thành công.";
      return RedirectToAction("Index");
    }
  }
}
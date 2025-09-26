using Microsoft.AspNetCore.Mvc;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Controllers
{
  [Route("Dashboard/Categories")]
  public class DashboardCategoryController : Controller
  {
    private readonly OnlineStoreDBContext _context;

    public DashboardCategoryController(OnlineStoreDBContext context)
    {
      _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin") return RedirectToAction("Login", "Auth");

      var categories = _context.Categories.ToList();
      return View("~/Views/Dashboard/Categories/Index.cshtml", categories);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin") return RedirectToAction("Login", "Auth");

      return View("~/Views/Dashboard/Categories/CreateCategory.cshtml");
    }

    [HttpPost("Create")]
    public IActionResult Create(CategoryViewModel model)
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin") return RedirectToAction("Login", "Auth");

      if (!ModelState.IsValid)
      {
        return View("~/Views/Dashboard/Categories/CreateCategory.cshtml", model);
      }

      var isDuplicate = _context.Categories.Any(c => c.CategoryName.ToLower() == model.CategoryName.ToLower());
      if (isDuplicate)
      {
        ModelState.AddModelError("CategoryName", "Tên loại đã tồn tại.");
        return View("~/Views/Dashboard/Categories/CreateCategory.cshtml", model);
      }

      var category = new Category
      {
        CategoryName = model.CategoryName,
        Description = model.Description
      };

      _context.Categories.Add(category);
      _context.SaveChanges();

      TempData["Success"] = "Tạo mới loại thành công.";
      return RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin") return RedirectToAction("Login", "Auth");

      var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
      if (category == null)
      {
        TempData["Error"] = "Không tìm thấy loại cần sửa.";
        return RedirectToAction("Index");
      }

      var model = new CategoryViewModel
      {
        CategoryName = category.CategoryName,
        Description = category.Description
      };

      return View("~/Views/Dashboard/Categories/EditCategory.cshtml", model);
    }

    [HttpPost("Edit/{id}")]
    public IActionResult Edit(int id, CategoryViewModel model)
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin") return RedirectToAction("Login", "Auth");

      if (!ModelState.IsValid)
      {
        ViewData["CategoryID"] = id;
        return View("~/Views/Dashboard/Categories/EditCategory.cshtml", model);
      }

      var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
      if (category == null)
      {
        TempData["Error"] = "Không tìm thấy loại cần cập nhật.";
        return RedirectToAction("Index");
      }

      var isDuplicate = _context.Categories.Any(c =>
          c.CategoryID != id &&
          c.CategoryName.ToLower() == model.CategoryName.ToLower());

      if (isDuplicate)
      {
        ModelState.AddModelError("CategoryName", "Tên loại đã tồn tại.");
        ViewData["CategoryID"] = id;
        return View("~/Views/Dashboard/Categories/EditCategory.cshtml", model);
      }

      category.CategoryName = model.CategoryName;
      category.Description = model.Description;

      _context.SaveChanges();

      TempData["Success"] = "Cập nhật loại thành công.";
      return RedirectToAction("Index");
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
      var role = HttpContext.Session.GetString("Role")?.ToLower();
      if (role != "admin") return RedirectToAction("Login", "Auth");

      var category = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
      if (category == null)
      {
        TempData["Error"] = "Không tìm thấy loại cần xóa.";
        return RedirectToAction("Index");
      }

      _context.Categories.Remove(category);
      _context.SaveChanges();

      TempData["Success"] = "Xóa loại thành công.";
      return RedirectToAction("Index");
    }
  }
}
using Microsoft.AspNetCore.Mvc;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[Route("Dashboard/User")]
public class DashboardUserController : Controller
{
  private readonly OnlineStoreDBContext _context;
  private readonly PasswordHasher<User> _hasher;

  public DashboardUserController(OnlineStoreDBContext context)
  {
    _context = context;
    _hasher = new PasswordHasher<User>();
  }

  // GET: /Dashboard/User
  [HttpGet("")]
  public IActionResult Index()
  {
    var role = HttpContext.Session.GetString("Role");
    if (role != "Admin") return RedirectToAction("Login", "Auth");

    var users = _context.Users.ToList();

    // ✅ Render view từ Views/Dashboard/Users/Index.cshtml
    return View("~/Views/Dashboard/Users/Index.cshtml", users);
  }

  [HttpGet("Create")]
  public IActionResult Create()
  {
    var role = HttpContext.Session.GetString("Role");
    if (role != "Admin") return RedirectToAction("Login", "Auth");

    return View("~/Views/Dashboard/Users/CreateUsers.cshtml");
  }

  [HttpPost("Create")]
  public IActionResult Create(RegisterViewModel model)
  {
    var role = HttpContext.Session.GetString("Role");
    if (role != "Admin") return RedirectToAction("Login", "Auth");

    if (!ModelState.IsValid)
    {
      return View("~/Views/Dashboard/Users/CreateUsers.cshtml", model);
    }

    var existingUser = _context.Users.FirstOrDefault(u => u.Username == model.Username);
    if (existingUser != null)
    {
      ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
      return View("~/Views/Dashboard/Users/CreateUsers.cshtml", model);
    }

    var newUser = new User
    {
      Username = model.Username,
      FullName = model.FullName,
      Email = model.Email,
      Phone = model.Phone,
      Address = model.Address,
    };

    newUser.PasswordHash = _hasher.HashPassword(newUser, model.Password);

    _context.Users.Add(newUser);
    _context.SaveChanges();

    TempData["Success"] = "Tạo mới người dùng thành công.";
    return RedirectToAction("Index");
  }

  [HttpGet("Edit/{id}")]
  public IActionResult Edit(int id)
  {
    var role = HttpContext.Session.GetString("Role");
    if (role != "Admin") return RedirectToAction("Login", "Auth");

    var user = _context.Users.FirstOrDefault(u => u.UserID == id);
    if (user == null) return NotFound();

    var model = new RegisterViewModel
    {
      Username = user.Username,
      FullName = user.FullName,
      Email = user.Email,
      Phone = user.Phone,
      Address = user.Address,
    };

    return View("~/Views/Dashboard/Users/EditUsers.cshtml", model);
  }

  [HttpPost("Edit/{id}")]
  public IActionResult Edit(int id, RegisterViewModel model)
  {
    var role = HttpContext.Session.GetString("Role");
    if (role != "Admin") return RedirectToAction("Login", "Auth");

    if (!ModelState.IsValid)
    {
      return View("~/Views/Dashboard/Users/EditUsers.cshtml", model);
    }

    var user = _context.Users.FirstOrDefault(u => u.UserID == id);
    if (user == null) return NotFound();

    user.FullName = model.FullName;
    user.Email = model.Email;
    user.Phone = model.Phone;
    user.Address = model.Address;

    // if (!string.IsNullOrEmpty(model.Password))
    // {
    //   user.PasswordHash = _hasher.HashPassword(user, model.Password);
    // }

    _context.SaveChanges();
    TempData["Success"] = "Cập nhật thông tin người dùng thành công.";
    return RedirectToAction("Index");
  }

  [HttpGet("Delete/{id}")]
  public IActionResult Delete(int id)
  {
    var role = HttpContext.Session.GetString("Role");
    if (role != "Admin") return RedirectToAction("Login", "Auth");

    var user = _context.Users.FirstOrDefault(u => u.UserID == id);
    if (user == null) return NotFound();

    _context.Users.Remove(user);
    _context.SaveChanges();

    TempData["Success"] = "Đã xóa người dùng thành công.";

    return RedirectToAction("Index");
  }
}
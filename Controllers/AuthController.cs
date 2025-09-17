using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Controllers
{
  public class AuthController : Controller
  {
    private readonly OnlineStoreDBContext _context;
    private readonly PasswordHasher<User> _hasher;

    public AuthController(OnlineStoreDBContext context)
    {
      _context = context;
      _hasher = new PasswordHasher<User>();
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
      if (HttpContext.Session.GetString("Username") != null)
      {
        var role = HttpContext.Session.GetString("Role");
        if (role == "Admin")
          return RedirectToAction("Index", "Dashboard");
        else
          return RedirectToAction("Index", "Home");
      }
      return View();
    }


    [HttpPost("/login")]
    public IActionResult Login(LoginViewModel model)
    {
      if (!ModelState.IsValid) return View(model);

      var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
      if (user == null)
      {
        ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
        return View(model);
      }

      var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
      if (result != PasswordVerificationResult.Success)
      {
        ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
        return View(model);
      }

      HttpContext.Session.SetString("Username", user.Username);
      HttpContext.Session.SetString("Role", user.Role);

      if (user.Role == "Admin")
        return RedirectToAction("Index", "Dashboard");
      else
        return RedirectToAction("Index", "Home");

    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
      if (HttpContext.Session.GetString("Username") != null)
      {
        return RedirectToAction("Index", "Home");
      }
      return View();
    }


    [HttpPost("/register")]
    public IActionResult Register(RegisterViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      if (_context.Users.Any(u => u.Username == model.Username))
      {
        ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
        return View(model);
      }

      var user = new User
      {
        Username = model.Username,
        FullName = model.FullName,
        Email = model.Email,
        Phone = model.Phone,
        Address = model.Address,
        Role = "User",
        CreatedAt = DateTime.Now
      };

      user.PasswordHash = _hasher.HashPassword(user, model.Password);

      _context.Users.Add(user);
      _context.SaveChanges();

      return RedirectToAction("Login");
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
      HttpContext.Session.Clear();
      return RedirectToAction("Login");
    }

  }
}

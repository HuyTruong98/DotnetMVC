using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;
using OnlineStoreMVC.Models.ViewModels;
using System.Linq;

namespace OnlineStoreMVC.Controllers
{
  public class HomeUserController : Controller
  {
    private readonly OnlineStoreDBContext _context;
    private readonly PasswordHasher<User> _hasher;

    public HomeUserController(OnlineStoreDBContext context)
    {
      _context = context;
      _hasher = new PasswordHasher<User>();
    }


    [HttpGet("Profile")]
    public IActionResult Profile()
    {
      var username = HttpContext.Session.GetString("Username");
      if (string.IsNullOrEmpty(username))
        return RedirectToAction("Login", "Auth");

      var user = _context.Users.FirstOrDefault(u => u.Username == username);
      if (user == null) return NotFound();

      return View("~/Views/Home/Users/Profile.cshtml", user);
    }

    [HttpGet("ChangePassword")]
    public IActionResult ChangePassword()
    {
      var username = HttpContext.Session.GetString("Username");
      if (string.IsNullOrEmpty(username))
        return RedirectToAction("Login", "Auth");

      return View("~/Views/Home/Users/ChangePassword.cshtml");
    }

    [HttpPost("ChangePassword")]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
      var username = HttpContext.Session.GetString("Username");
      if (string.IsNullOrEmpty(username))
        return RedirectToAction("Login", "Auth");

      if (!ModelState.IsValid)
        return View("~/Views/Home/Users/ChangePassword.cshtml", model);

      var user = _context.Users.FirstOrDefault(u => u.Username == username);
      if (user == null) return NotFound();


      var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);
      if (result == PasswordVerificationResult.Failed)
      {
        ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng");
        return View("~/Views/Dashboard/Users/ChangePassword.cshtml", model);
      }


      user.PasswordHash = _hasher.HashPassword(user, model.NewPassword);
      _context.SaveChanges();

      TempData["Success"] = "Đổi mật khẩu thành công!";
      return RedirectToAction("Profile");
    }
  }
}
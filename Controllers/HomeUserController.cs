using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;
using OnlineStoreMVC.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdatePhoneAddress(string Phone, string Address)
    {
      var sessionUser = HttpContext.Session.GetString("Username");
      if (string.IsNullOrEmpty(sessionUser))
        return RedirectToAction("Login", "Auth");

      var user = _context.Users.FirstOrDefault(u => u.Username == sessionUser);
      if (user == null) return NotFound();

      if (string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Address))
      {
        TempData["Error"] = "Số điện thoại và địa chỉ không được để trống.";
        return RedirectToAction("Profile");
      }

      user.Phone = Phone;
      user.Address = Address;
      _context.SaveChanges();

      TempData["Success"] = "Cập nhật số điện thoại và địa chỉ thành công.";
      return RedirectToAction("Profile");
    }

    [HttpGet("MyOrders")]
    public IActionResult MyOrders()
    {
      var username = HttpContext.Session.GetString("Username");
      if (string.IsNullOrEmpty(username))
        return RedirectToAction("Login", "Auth");

      var user = _context.Users.FirstOrDefault(u => u.Username == username);
      if (user == null) return NotFound();

      var orders = _context.Orders
          .Include(o => o.OrderDetails)
              .ThenInclude(od => od.Variant)
                  .ThenInclude(v => v.Product)
          .Where(o => o.UserID == user.UserID)
          .ToList();

      return View("~/Views/Home/Users/MyOrders.cshtml", orders);
    }
  }
}
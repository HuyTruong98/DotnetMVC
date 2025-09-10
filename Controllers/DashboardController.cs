// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
  public IActionResult Index()
  {
    var role = HttpContext.Session.GetString("Role");
    if (role != "Admin")
    {
      return RedirectToAction("Login", "Auth");
    }

    return View();
  }
}

using Microsoft.AspNetCore.Mvc;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models;
using System.Linq;

public class DashboardController : Controller
{
  private readonly OnlineStoreDBContext _context;

  public DashboardController(OnlineStoreDBContext context)
  {
    _context = context;
  }

  public IActionResult Index()
  {
    var role = HttpContext.Session.GetString("Role")?.ToLower();
    if (role != "admin")
    {
      return RedirectToAction("Login", "Auth");
    }

    var userCount = _context.Users.Count();
    var productCount = _context.Products.Count();
    var orderCount = _context.Orders.Count();
    var totalRevenue = _context.Orders.Sum(o => o.TotalAmount);

    // Lấy 5 đơn hàng gần nhất làm hoạt động
    var recentOrders = _context.Orders
        .OrderByDescending(o => o.OrderDate)
        .Take(5)
        .Select(o => new RecentActivity
        {
          IconClass = "fas fa-shopping-cart",
          Message = $"Đơn hàng mới #{o.OrderID}",
          Timestamp = o.OrderDate
        })
        .ToList();

    // Tạo ViewModel
    var vm = new DashboardViewModel
    {
      UserCount = userCount,
      ProductCount = productCount,
      OrderCount = orderCount,
      TotalRevenue = totalRevenue,
      RecentActivities = recentOrders
    };

    return View(vm);
  }
}

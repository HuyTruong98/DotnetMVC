using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models.ViewModels;

namespace OnlineStoreMVC.Controllers
{
  [Route("Dashboard/Orders")]
  public class DashboardOrderController : Controller
  {
    private readonly OnlineStoreDBContext _context;

    public DashboardOrderController(OnlineStoreDBContext context)
    {
      _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var orders = _context.Orders
          .Include(o => o.User)
          .Include(o => o.OrderDetails)
              .ThenInclude(od => od.Product)
          .Select(o => new OrderManageViewModel
          {
            OrderID = o.OrderID,
            UserID = o.UserID,
            UserName = o.User.FullName,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TotalAmount = o.TotalAmount,
            Items = o.OrderDetails.Select(od => new OrderDetailItem
            {
              ProductID = od.ProductID,
              ProductName = od.Product.ProductName,
              Quantity = od.Quantity,
              UnitPrice = od.UnitPrice
            }).ToList()
          }).ToList();

      return View("~/Views/Dashboard/Orders/Index.cshtml", orders);
    }
  }
}

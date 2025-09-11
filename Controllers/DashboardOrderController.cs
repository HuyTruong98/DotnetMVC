using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Data;
using OnlineStoreMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStoreMVC.Models;

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

    [HttpGet("Create")]
    public IActionResult Create()
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      ViewBag.Products = _context.Products
          .Select(p => new SelectListItem
          {
            Value = p.ProductID.ToString(),
            Text = p.ProductName
          }).ToList();

      ViewBag.Users = _context.Users
          .Select(u => new SelectListItem
          {
            Value = u.UserID.ToString(),
            Text = u.FullName
          }).ToList();

      var model = new OrderFormViewModel
      {
        Items = new List<OrderItemInput> { new() }
      };

      return View("~/Views/Dashboard/Orders/CreateOrders.cshtml", model);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(OrderFormViewModel model)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      // Nếu model không hợp lệ -> load lại dropdown + return view
      if (!ModelState.IsValid || model.Items == null || !model.Items.Any())
      {
        ViewBag.Products = _context.Products
            .Select(p => new SelectListItem
            {
              Value = p.ProductID.ToString(),
              Text = p.ProductName
            }).ToList();

        ViewBag.Users = _context.Users
            .Select(u => new SelectListItem
            {
              Value = u.UserID.ToString(),
              Text = u.FullName
            }).ToList();

        return View("~/Views/Dashboard/Orders/CreateOrders.cshtml", model);
      }

      var order = new Order
      {
        UserID = model.UserID,
        OrderDate = DateTime.Now,
        Status = model.Status,
        TotalAmount = 0
      };

      _context.Orders.Add(order);
      await _context.SaveChangesAsync();

      decimal total = 0;
      foreach (var item in model.Items)
      {
        var product = await _context.Products.FindAsync(item.ProductID);
        if (product == null) continue;

        var orderDetail = new OrderDetail
        {
          OrderID = order.OrderID,
          ProductID = item.ProductID,
          Quantity = item.Quantity,
          UnitPrice = product.Price
        };

        total += product.Price * item.Quantity;

        _context.OrderDetails.Add(orderDetail);


        product.Stock -= item.Quantity;
      }

      order.TotalAmount = total;

      await _context.SaveChangesAsync();

      TempData["Success"] = "Tạo đơn hàng thành công.";
      return RedirectToAction("Index");
    }


    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var order = _context.Orders
          .Include(o => o.OrderDetails)
          .FirstOrDefault(o => o.OrderID == id);

      if (order == null) return NotFound();

      var model = new OrderFormViewModel
      {
        OrderID = order.OrderID,
        UserID = order.UserID,
        Status = order.Status,
        Items = order.OrderDetails.Select(od => new OrderItemInput
        {
          ProductID = od.ProductID,
          Quantity = od.Quantity
        }).ToList()
      };

      ViewBag.Products = _context.Products
          .Select(p => new SelectListItem
          {
            Value = p.ProductID.ToString(),
            Text = p.ProductName
          }).ToList();

      ViewBag.Users = _context.Users
          .Select(u => new SelectListItem
          {
            Value = u.UserID.ToString(),
            Text = u.FullName
          }).ToList();

      return View("~/Views/Dashboard/Orders/EditOrders.cshtml", model);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, OrderFormViewModel model)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      if (!ModelState.IsValid || model.Items == null || !model.Items.Any(i => i.ProductID > 0 && i.Quantity > 0))
      {
        ViewBag.Products = _context.Products
            .Select(p => new SelectListItem
            {
              Value = p.ProductID.ToString(),
              Text = p.ProductName
            }).ToList();

        ViewBag.Users = _context.Users
            .Select(u => new SelectListItem
            {
              Value = u.UserID.ToString(),
              Text = u.FullName
            }).ToList();

        return View("~/Views/Dashboard/Orders/EditOrders.cshtml", model);
      }

      var order = await _context.Orders
          .Include(o => o.OrderDetails)
          .FirstOrDefaultAsync(o => o.OrderID == id);

      if (order == null) return NotFound();

      // Lưu lại chi tiết cũ để xử lý tồn kho
      var oldDetails = order.OrderDetails.ToList();

      // Cập nhật thông tin đơn hàng
      order.UserID = model.UserID;
      order.Status = model.Status;
      order.OrderDate = DateTime.Now;

      // Xử lý tồn kho trước khi xóa chi tiết cũ
      foreach (var old in oldDetails)
      {
        var product = await _context.Products.FindAsync(old.ProductID);
        if (product != null)
        {
          product.Stock += old.Quantity; // ✅ Hoàn lại tồn kho cũ
        }
      }

      // Xóa chi tiết cũ
      _context.OrderDetails.RemoveRange(oldDetails);

      decimal total = 0;

      foreach (var item in model.Items)
      {
        var product = await _context.Products.FindAsync(item.ProductID);
        if (product == null) continue;

        var detail = new OrderDetail
        {
          OrderID = order.OrderID,
          ProductID = item.ProductID,
          Quantity = item.Quantity,
          UnitPrice = product.Price
        };

        total += product.Price * item.Quantity;
        _context.OrderDetails.Add(detail);

        product.Stock -= item.Quantity; // ✅ Trừ tồn kho mới
      }

      order.TotalAmount = total;
      await _context.SaveChangesAsync();

      TempData["Success"] = "Cập nhật đơn hàng thành công.";
      return RedirectToAction("Index");
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var order = await _context.Orders
          .Include(o => o.OrderDetails)
          .FirstOrDefaultAsync(o => o.OrderID == id);

      if (order == null) return NotFound();

      foreach (var detail in order.OrderDetails)
      {
        var product = await _context.Products.FindAsync(detail.ProductID);
        if (product != null)
        {
          product.Stock += detail.Quantity;
        }
      }

      _context.OrderDetails.RemoveRange(order.OrderDetails);
      _context.Orders.Remove(order);

      await _context.SaveChangesAsync();

      TempData["Success"] = "Đã xóa đơn hàng và hoàn lại tồn kho.";
      return RedirectToAction("Index");
    }
  }
}

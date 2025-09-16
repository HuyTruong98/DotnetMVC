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
    public async Task<IActionResult> Index()
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      var now = DateTime.Now;

      var orders = await _context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderDetails)
          .ThenInclude(od => od.Variant)
            .ThenInclude(v => v.Product)
        .Select(o => new OrderManageViewModel
        {
          OrderID = o.OrderID,
          UserName = o.User.FullName,
          OrderDate = o.OrderDate,
          Status = o.Status,
          TotalAmount = o.TotalAmount,
          Items = o.OrderDetails.Select(od => new OrderDetailItem
          {
            VariantID = od.VariantID,
            ProductName = od.Variant.Product.ProductName,
            Size = od.Variant.Size,
            Color = od.Variant.Color,
            Quantity = od.Quantity,
            UnitPrice = od.UnitPrice,

            // Lấy khuyến mãi đang có cho Product này
            SalePercent = _context.Promotions
              .Where(p =>
                p.ProductID == od.Variant.ProductID
                && p.StartDate <= now
                && p.EndDate >= now)
              .Select(p => p.DiscountPercent)
              .FirstOrDefault()
          }).ToList()
        })
        .ToListAsync();

      return View("~/Views/Dashboard/Orders/Index.cshtml", orders);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin") return RedirectToAction("Login", "Auth");

      ViewBag.Users = _context.Users
     .Select(u => new SelectListItem
     {
       Value = u.UserID.ToString(),
       Text = u.FullName
     }).ToList();

      ViewBag.Variants = _context.ProductVariants
       .Include(v => v.Product)
       .Select(v => new SelectListItem
       {
         Value = v.VariantID.ToString(),
         Text = $"{v.Product.ProductName} – {v.Size} – {v.Color}"
       }).ToList();

      return View("~/Views/Dashboard/Orders/CreateOrders.cshtml", new OrderFormViewModel());
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(OrderFormViewModel model)
    {
      var role = HttpContext.Session.GetString("Role");
      if (role != "Admin")
        return RedirectToAction("Login", "Auth");

      ViewBag.Users = _context.Users
     .Select(u => new SelectListItem
     {
       Value = u.UserID.ToString(),
       Text = u.FullName
     }).ToList();

      ViewBag.Variants = await _context.ProductVariants
         .Include(v => v.Product)
         .Select(v => new SelectListItem
         {
           Value = v.VariantID.ToString(),
           Text = $"{v.Product.ProductName} – {v.Size} – {v.Color}"
         })
         .ToListAsync();

      if (!ModelState.IsValid)
      {
        return View("~/Views/Dashboard/Orders/CreateOrders.cshtml", model);
      }

      var order = new Order
      {
        UserID = model.UserID,
        Status = model.Status,
        OrderDate = DateTime.Now,
        TotalAmount = 0m,
        OrderDetails = new List<OrderDetail>()
      };

      decimal total = 0m;

      foreach (var item in model.Items)
      {
        var variant = await _context.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.VariantID == item.VariantID);

        if (variant == null)
        {
          ModelState.AddModelError("", $"Không tìm thấy biến thể ID = {item.VariantID}");
          return View("~/Views/Dashboard/Orders/CreateOrders.cshtml", model);
        }

        if (variant.Stock < item.Quantity)
        {
          ModelState.AddModelError("",
              $"Không đủ tồn kho cho {variant.Product.ProductName} ({variant.Size} - {variant.Color}). " +
              $"Hiện chỉ còn {variant.Stock} chiếc.");
          return View("~/Views/Dashboard/Orders/CreateOrders.cshtml", model);
        }

        variant.Stock -= item.Quantity;
        _context.ProductVariants.Update(variant);

        var detail = new OrderDetail
        {
          VariantID = item.VariantID,
          Quantity = item.Quantity,
          UnitPrice = variant.Product.Price
        };
        total += detail.Quantity * detail.UnitPrice;
        order.OrderDetails.Add(detail);
      }


      order.TotalAmount = total;
      _context.Orders.Add(order);
      await _context.SaveChangesAsync();

      TempData["Success"] = "Tạo đơn hàng thành công!";
      return RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
      if (HttpContext.Session.GetString("Role") != "Admin")
        return RedirectToAction("Login", "Auth");

      var order = await _context.Orders
          .Include(o => o.OrderDetails)
          .ThenInclude(od => od.Variant)
          .ThenInclude(v => v.Product)
          .FirstOrDefaultAsync(o => o.OrderID == id);

      if (order == null) return NotFound();

      var vm = new OrderFormViewModel
      {
        OrderID = order.OrderID,
        UserID = order.UserID,
        Status = order.Status,
        Items = order.OrderDetails
                            .Select(od => new OrderItemInput
                            {
                              VariantID = od.VariantID,
                              Quantity = od.Quantity
                            })
                            .ToList()
      };

      ViewBag.Users = await _context.Users
          .Select(u => new SelectListItem
          {
            Value = u.UserID.ToString(),
            Text = u.FullName
          }).ToListAsync();

      ViewBag.Variants = await _context.ProductVariants
          .Include(v => v.Product)
          .Select(v => new SelectListItem
          {
            Value = v.VariantID.ToString(),
            Text = $"{v.Product.ProductName} – {v.Size} – {v.Color}"
          }).ToListAsync();

      return View("~/Views/Dashboard/Orders/EditOrders.cshtml", vm);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, OrderFormViewModel model)
    {
      if (id != model.OrderID) return BadRequest();
      if (HttpContext.Session.GetString("Role") != "Admin")
        return RedirectToAction("Login", "Auth");

      // Reload dropdown
      ViewBag.Users = await _context.Users
          .Select(u => new SelectListItem
          {
            Value = u.UserID.ToString(),
            Text = u.FullName
          }).ToListAsync();

      if (!ModelState.IsValid) return View("EditOrders", model);

      var order = await _context.Orders
      .Include(o => o.OrderDetails)
      .FirstOrDefaultAsync(o => o.OrderID == id);
      if (order == null) return NotFound();

      var locked = order.Status == "Shipping" || order.Status == "Completed";
      if (locked)
      {
        // Nếu locked chỉ update Status
        if (order.Status != model.Status)
        {
          order.Status = model.Status;
          _context.Orders.Update(order);
          await _context.SaveChangesAsync();
          TempData["Success"] = "Cập nhật trạng thái thành công!";
        }
        return RedirectToAction("Index");
      }

      var oldDetails = order.OrderDetails.ToDictionary(od => od.VariantID);

      foreach (var old in oldDetails.Values)
      {
        if (!model.Items.Any(mi => mi.VariantID == old.VariantID))
        {
          var variant = await _context.ProductVariants.FindAsync(old.VariantID);
          variant.Stock += old.Quantity;
          _context.ProductVariants.Update(variant);

          _context.OrderDetails.Remove(old);
        }
      }

      decimal total = 0m;
      foreach (var item in model.Items)
      {
        // lấy variant + product
        var variant = await _context.ProductVariants
          .Include(v => v.Product)
          .FirstOrDefaultAsync(v => v.VariantID == item.VariantID);
        if (variant == null)
        {
          ModelState.AddModelError("", $"Biến thể {item.VariantID} không tồn tại.");
          return View("~/Views/Dashboard/Orders/EditOrders.cshtml", model);
        }

        oldDetails.TryGetValue(item.VariantID, out var existingDetail);

        if (existingDetail != null)
        {
          // đã có detail cũ → tính diff
          var diff = item.Quantity - existingDetail.Quantity;
          if (diff > 0)
          {
            if (variant.Stock < diff)
            {
              ModelState.AddModelError("",
                $"Không đủ stock {variant.Product.ProductName} ({variant.Size}/{variant.Color}).");
              return View("~/Views/Dashboard/Orders/EditOrders.cshtml", model);
            }
            variant.Stock -= diff;
          }
          else if (diff < 0)
          {
            variant.Stock += -diff;
          }
          _context.ProductVariants.Update(variant);

          existingDetail.Quantity = item.Quantity;
          existingDetail.UnitPrice = variant.Product.Price;
          _context.OrderDetails.Update(existingDetail);
          total += existingDetail.Quantity * existingDetail.UnitPrice;
        }
        else
        {
          // hoàn toàn mới
          if (variant.Stock < item.Quantity)
          {
            ModelState.AddModelError("",
              $"Không đủ stock {variant.Product.ProductName} ({variant.Size}/{variant.Color}).");
            return View("~/Views/Dashboard/Orders/EditOrders.cshtml", model);
          }
          variant.Stock -= item.Quantity;
          _context.ProductVariants.Update(variant);

          var detail = new OrderDetail
          {
            OrderID = order.OrderID,
            VariantID = item.VariantID,
            Quantity = item.Quantity,
            UnitPrice = variant.Product.Price
          };
          _context.OrderDetails.Add(detail);
          total += detail.Quantity * detail.UnitPrice;
        }
      }

      order.UserID = model.UserID;
      order.Status = model.Status;
      order.TotalAmount = total;
      _context.Orders.Update(order);

      await _context.SaveChangesAsync();
      TempData["Success"] = "Cập nhật đơn hàng thành công!";
      return RedirectToAction("Index");
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      if (HttpContext.Session.GetString("Role") != "Admin")
        return RedirectToAction("Login", "Auth");

      var order = await _context.Orders
          .Include(o => o.OrderDetails)
          .FirstOrDefaultAsync(o => o.OrderID == id);
      if (order == null)
      {
        TempData["Error"] = "Không tìm thấy đơn hàng để xóa.";
        return RedirectToAction("Index");
      }

      foreach (var od in order.OrderDetails)
      {
        var variant = await _context.ProductVariants
            .FirstOrDefaultAsync(v => v.VariantID == od.VariantID);
        if (variant != null)
        {
          variant.Stock += od.Quantity;
          _context.ProductVariants.Update(variant);
        }
      }

      _context.OrderDetails.RemoveRange(order.OrderDetails);
      _context.Orders.Remove(order);

      await _context.SaveChangesAsync();
      TempData["Success"] = "Xóa đơn hàng thành công.";
      return RedirectToAction("Index");
    }
  }

}

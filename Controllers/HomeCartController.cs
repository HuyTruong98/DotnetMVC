using Microsoft.AspNetCore.Mvc;
using OnlineStoreMVC.Models;
using OnlineStoreMVC.Helpers;

namespace OnlineStoreMVC.Controllers
{
  public class HomeCartController : Controller
  {
    [HttpPost]
    public IActionResult AddToCart(int productId)
    {
      var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

      var existingItem = cart.FirstOrDefault(c => c.ProductID == productId);
      if (existingItem != null)
      {
        existingItem.Quantity++;
      }
      else
      {
        cart.Add(new CartItem { ProductID = productId, Quantity = 1 });
      }

      // Lưu lại giỏ hàng vào Session
      HttpContext.Session.SetObject("Cart", cart);

      // Trả về JSON để cập nhật badge
      return Json(new
      {
        success = true,
        cartCount = cart.Sum(c => c.Quantity)
      });
    }
  }
}
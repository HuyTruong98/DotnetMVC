using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Models.ViewModels
{
  public class CartLineItem
  {
    public int VariantID { get; set; }
    public string ProductName { get; set; } = "";
    public string Size { get; set; } = "";
    public string Color { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public decimal? PromotionPrice { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = "";

    // Luôn dùng PromotionPrice nếu có, ngược lại dùng UnitPrice
    public decimal SubTotal => (PromotionPrice ?? UnitPrice) * Quantity;
  }
}

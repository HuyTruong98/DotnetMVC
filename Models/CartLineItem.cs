using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Models.ViewModels
{
  public class CartLineItem
  {
    public int VariantID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    public decimal SubTotal => UnitPrice * Quantity;
  }
}
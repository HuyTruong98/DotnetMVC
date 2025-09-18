using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Models.ViewModels
{
  public class OrderFormViewModel
  {
    public int? OrderID { get; set; }
    public int UserID { get; set; }
    public string Status { get; set; } = "Pending";

    public string? Description { get; set; }

    public List<OrderItemInput> Items { get; set; } = new()
    {
      new OrderItemInput()
    };
  }

  public class OrderItemInput
  {
    public int VariantID { get; set; }
    public int Quantity { get; set; }
  }
}
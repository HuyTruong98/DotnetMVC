using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Models.ViewModels
{
  public class OrderFormViewModel
  {
    public int? OrderID { get; set; }
    public int UserID { get; set; }
    public string Status { get; set; } = "Pending";

    public List<OrderItemInput> Items { get; set; } = new();
  }

  public class OrderItemInput
  {
    public int ProductID { get; set; }
    public int Quantity { get; set; }
  }
}
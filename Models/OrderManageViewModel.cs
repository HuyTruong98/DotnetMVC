namespace OnlineStoreMVC.Models.ViewModels
{
  public class OrderManageViewModel
  {
    public int OrderID { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }

    public List<OrderDetailItem> Items { get; set; } = new();
  }

  public class OrderDetailItem
  {
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
  }
}
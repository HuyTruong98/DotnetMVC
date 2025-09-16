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

    public decimal TotalAfterDiscount =>
    Items.Sum(x => x.DiscountedAmount);

    public List<OrderDetailItem> Items { get; set; } = new();
  }

  public class OrderDetailItem
  {
    public int VariantID { get; set; }
    public string ProductName { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public int SalePercent { get; set; }

    public decimal DiscountedUnitPrice =>
      UnitPrice * (100 - SalePercent) / 100m;

    public decimal Amount =>
      Quantity * UnitPrice;

    public decimal DiscountedAmount =>
      Quantity * DiscountedUnitPrice;
  }
}
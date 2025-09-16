using System.ComponentModel.DataAnnotations.Schema;


namespace OnlineStoreMVC.Models
{
  public class OrderDetail
  {
    public int OrderDetailID { get; set; }
    public int OrderID { get; set; }
    public int VariantID { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; }

    [ForeignKey(nameof(VariantID))]
    public ProductVariant Variant { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{
  public class ProductVariant
  {
    [Key]
    public int VariantID { get; set; }

    public int ProductID { get; set; }

    [MaxLength(10)]
    public string? Size { get; set; }

    [MaxLength(50)]
    public string? Color { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public virtual Product Product { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
  }
}

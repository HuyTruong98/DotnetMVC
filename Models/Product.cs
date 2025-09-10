using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{
  public class Product
  {
    public int ProductID { get; set; }
    public int CategoryID { get; set; }
    public string ProductName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Status { get; set; }
    public DateTime? CreatedAt { get; set; }

    public Category Category { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; }
  }
}
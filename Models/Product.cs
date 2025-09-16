using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{
  public class Product
  {
    public int ProductID { get; set; }
    public int CategoryID { get; set; }

    [Required]
    public string ProductName { get; set; }

    public string? Description { get; set; }

    [Required]
    [Range(1, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [RegularExpression("Available|OutOfStock|Promotion")]
    public string Status { get; set; }

    public bool IsFeatured { get; set; } = false;
    public DateTime? CreatedAt { get; set; }

    // 🔗 Quan hệ
    public Category Category { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; }
    public virtual ICollection<Promotion> Promotions { get; set; }

    // 🔗 Thêm quan hệ với biến thể (size, color, stock)
    public virtual ICollection<ProductVariant> Variants { get; set; }
  }
}

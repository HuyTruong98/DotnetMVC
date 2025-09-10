using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{
  public class ProductImage
  {
    [Key] // 👈 Đây là khóa chính
    public int ImageID { get; set; }

    public int ProductID { get; set; }

    [Required]
    public string ImageURL { get; set; }

    public bool IsPrimary { get; set; }

    public virtual Product Product { get; set; }
  }
}
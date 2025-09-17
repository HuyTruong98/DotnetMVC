using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineStoreMVC.Models.ViewModels
{
  public class ProductViewModel
  {
    public int ProductID { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống")]
    public string ProductName { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Giá sản phẩm không được bỏ trống")]
    [Range(1, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 1")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn danh mục")]
    public int? CategoryID { get; set; }

    [Required(ErrorMessage = "Trạng thái không được bỏ trống")]
    [RegularExpression("Available|OutOfStock|Promotion", ErrorMessage = "Trạng thái không hợp lệ")]
    public string Status { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }

    // Upload nhiều ảnh
    public List<IFormFile>? Images { get; set; }

    [Required(ErrorMessage = "Phải có ít nhất một biến thể")]
    public List<ProductVariantViewModel> Variants { get; set; } = new();
    public List<int> ProtectedVariantIds { get; set; } = new();

  }

  public class ProductVariantViewModel
  {
    public int? VariantID { get; set; }

    [Required(ErrorMessage = "Size không được bỏ trống")]
    [MaxLength(10, ErrorMessage = "Size tối đa 10 ký tự")]
    public string? Size { get; set; }

    [Required(ErrorMessage = "Màu sắc không được bỏ trống")]
    [MaxLength(50, ErrorMessage = "Màu sắc tối đa 50 ký tự")]
    public string? Color { get; set; }

    [Required(ErrorMessage = "Tồn kho không được bỏ trống")]
    [Range(1, int.MaxValue, ErrorMessage = "Tồn kho phải lớn hơn hoặc bằng 1")]
    public int Stock { get; set; }
  }

}
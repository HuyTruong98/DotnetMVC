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

    [Required(ErrorMessage = "Tồn kho không được bỏ trống")]
    [Range(1, int.MaxValue, ErrorMessage = "Tồn kho phải lớn hơn hoặc bằng 1")]
    public int? Stock { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn danh mục")]
    public int? CategoryID { get; set; }

    [Required(ErrorMessage = "Trạng thái không được bỏ trống")]
    [RegularExpression("Available|OutOfStock|Promotion", ErrorMessage = "Trạng thái không hợp lệ")]
    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    // Upload nhiều ảnh
    public List<IFormFile>? Images { get; set; }
  }
}
using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{

  public class CategoryViewModel
  {
    [Required(ErrorMessage = "Tên loại không được bỏ trống")]
    public string CategoryName { get; set; }

    public string? Description { get; set; }
  }
}
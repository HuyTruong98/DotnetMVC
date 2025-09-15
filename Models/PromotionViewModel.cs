using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{
  public class PromotionViewModel
  {
    public int PromotionID { get; set; }

    [Required(ErrorMessage = "Sản phẩm không được bỏ trống")]
    public int ProductID { get; set; }

    [Required(ErrorMessage = "Giảm giá không được bỏ trống")]
    public int DiscountPercent { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu không được bỏ trống")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc không được bỏ trống")]
    public DateTime EndDate { get; set; }
  }
}
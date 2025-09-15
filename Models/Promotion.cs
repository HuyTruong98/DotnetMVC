using System;

namespace OnlineStoreMVC.Models
{
  public class Promotion
  {
    public int PromotionID { get; set; }
    public int ProductID { get; set; }
    public int DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public virtual Product Product { get; set; }

  }
}
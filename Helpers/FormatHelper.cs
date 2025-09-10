using System.Globalization;

namespace OnlineStoreMVC.Helpers
{
  public static class FormatHelper
  {
    public static string FormatCurrency(decimal value)
    {
      return string.Format(new CultureInfo("vi-VN"), "{0:N0} ₫", value);
    }

    public static string FormatDate(DateTime? date)
    {
      return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : "Chưa có";
    }

    public static string FormatStatusBadge(string status)
    {
      return status switch
      {
        "Available" => "<span class='badge bg-success'>Available</span>",
        "OutOfStock" => "<span class='badge bg-danger'>Out of Stock</span>",
        "Promotion" => "<span class='badge bg-warning text-dark'>Promotion</span>",
        _ => "<span class='badge bg-secondary'>Unknown</span>"
      };
    }
  }
}
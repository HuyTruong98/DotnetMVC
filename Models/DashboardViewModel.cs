using System;
using System.Collections.Generic;

namespace OnlineStoreMVC.Models
{
  public class DashboardViewModel
  {
    public int UserCount { get; set; }
    public int ProductCount { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<RecentActivity> RecentActivities { get; set; }
  }

  public class RecentActivity
  {
    public string IconClass { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
  }
}
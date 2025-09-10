using System;

namespace OnlineStoreMVC.Models
{
  public class User
  {
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "User";
    public DateTime? CreatedAt { get; set; }
  }
}
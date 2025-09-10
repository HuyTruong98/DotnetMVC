using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{
  public class RegisterViewModel
  {
    [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Họ và tên không được bỏ trống")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email không được bỏ trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
    public string Address { get; set; }
  }
}
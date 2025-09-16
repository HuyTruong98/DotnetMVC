using System.ComponentModel.DataAnnotations;

namespace OnlineStoreMVC.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại bắt buộc nhập")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới bắt buộc nhập")]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu bắt buộc nhập")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp")]
        public string? ConfirmPassword { get; set; }
    }
}

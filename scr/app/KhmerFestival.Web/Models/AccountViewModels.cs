using System.ComponentModel.DataAnnotations;

namespace KhmerFestival.Web.Models
{
 public class LoginViewModel
 {
 [Required(ErrorMessage = "Email bắt buộc")]
 [EmailAddress(ErrorMessage = "Email không hợp lệ")]
 public string Email { get; set; }

 [Required(ErrorMessage = "Mật khẩu bắt buộc")]
 [DataType(DataType.Password)]
 public string Password { get; set; }

 public bool RememberMe { get; set; }
 }

 public class RegisterViewModel
 {
 [Required(ErrorMessage = "Email bắt buộc")]
 [EmailAddress(ErrorMessage = "Email không hợp lệ")]
 public string Email { get; set; }

 [Required(ErrorMessage = "Mật khẩu bắt buộc")]
 [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength =6)]
 [DataType(DataType.Password)]
 public string Password { get; set; }

 [DataType(DataType.Password)]
 [Compare("Password", ErrorMessage = "Mật khẩu không khớp")] 
 public string ConfirmPassword { get; set; }
 }

 public class ForgotPasswordViewModel
 {
 [Required(ErrorMessage = "Email bắt buộc")]
 [EmailAddress(ErrorMessage = "Email không hợp lệ")]
 public string Email { get; set; }
 }
}

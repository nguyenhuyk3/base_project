using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Motel.Areas.UserAccount.Models
{
    public class RegisteredUserAccount
    {
        [Required(ErrorMessage = "Họ và tên không được bỏ trống")]
        [Display(Name = "Họ và tên")]
        [RegularExpression(@"^[\p{L}_\s]+$",
                ErrorMessage = "Họ và tên không được chứa ký tự đặc biệt hoặc số")]
        public string FullName { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime DayOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public bool Sex { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [Display(Name = "Email đăng kí")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email phải có đuôi là @gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [RegularExpression("^(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
                ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, " +
                                "bao gồm ít nhất một chữ thường, " +
                                "một số và một ký tự đặc biệt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu xác nhận không được bỏ trống")]
        [RegularExpression("^(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
                                ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, " +
                                "bao gồm ít nhất một chữ thường, " +
                                "một số và một ký tự đặc biệt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu xác nhận")]
        public string ConfirmedPassword { get; set; }
    }

    public class LoginUserAccount
    {
        [Required(ErrorMessage = "Email không được bỏ trống")]
        [Display(Name = "Email đăng nhập")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email phải có đuôi là @gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [RegularExpression("^(?=.*[a-z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
                                ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, " +
                                "bao gồm ít nhất một chữ thường, " +
                                "một số và một ký tự đặc biệt.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Nhớ mật khẩu")]
        public bool RememberMe { get; set; } = false;
        //public string ReturnUrl { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mã thông báo không được để trống")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm ít nhất một chữ thường, một số và một ký tự đặc biệt.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}

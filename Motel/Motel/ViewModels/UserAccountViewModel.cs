using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Motel.ViewModels
{
    public class RegisteredUserAccount
    {
        [Required(ErrorMessage = "Họ và tên không được bỏ trống")]
        [Display(Name = "Họ và tên")]
        [RegularExpression(@"^[\p{L}_\s]+$",
                ErrorMessage = "Họ và tên không được chứa ký tự đặc biệt hoặc số")]
        public string FullName { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime BirthDay { get; set; }

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
    }
}

using System.ComponentModel.DataAnnotations;

namespace ThueXeToanCau.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Bạn cần nhập Tên Đăng Nhập")]
        [Display(Name = "Tên Đăng Nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Bạn cần nhập Mật Khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật Khẩu")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
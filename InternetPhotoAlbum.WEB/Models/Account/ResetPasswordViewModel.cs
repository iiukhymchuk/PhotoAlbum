using System.ComponentModel.DataAnnotations;

namespace InternetPhotoAlbum.WEB.Models
{
    public class ResetPasswordViewModel : SharedLayoutViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} должен быть {2} символов длиной", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторить пароль")]
        [Compare("Password", ErrorMessage = "Введенные пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string UserId { get; set; }
    }
}
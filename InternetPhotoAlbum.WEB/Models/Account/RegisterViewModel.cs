using System.ComponentModel.DataAnnotations;

namespace InternetPhotoAlbum.WEB.Models
{
    public class RegisterViewModel : SharedLayoutViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string NickName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Почта")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен быть минимум {2} символов длиной", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторить пароль")]
        [Compare("Password", ErrorMessage = "Введенные пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
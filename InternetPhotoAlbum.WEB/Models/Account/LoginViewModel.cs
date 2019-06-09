using System.ComponentModel.DataAnnotations;

namespace InternetPhotoAlbum.WEB.Models
{
    public class LoginViewModel : SharedLayoutViewModel
    {
        [Required]
        [Display(Name = "Почта")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }
}
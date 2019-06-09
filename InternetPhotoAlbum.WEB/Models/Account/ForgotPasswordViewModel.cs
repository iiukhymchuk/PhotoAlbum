using System.ComponentModel.DataAnnotations;

namespace InternetPhotoAlbum.WEB.Models
{
    public class ForgotPasswordViewModel : SharedLayoutViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Почта")]
        public string Email { get; set; }
    }
}
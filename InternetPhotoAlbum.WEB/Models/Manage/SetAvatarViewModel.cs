using InternetPhotoAlbum.WEB.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace InternetPhotoAlbum.WEB.Models
{
    public class SetAvatarViewModel : SharedLayoutViewModel
    {
        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Выберите файл для аватара")]
        [FileLength(5, "Файл должен быть не больше 5 мегабайт")]
        [ImageExtensions("jpg|jpeg|png", "Это должен быть графический файл с расширением .jpg, .jpeg или .png")]
        public HttpPostedFileBase File { get; set; }
    }
}
using InternetPhotoAlbum.WEB.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace InternetPhotoAlbum.WEB.Models
{
    public class ImageViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileStoreName { get; set; }
        public DateTime UploadDate { get; set; }

        [Required]
        [Display(Name = "Имя картинки")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание картинки")]
        [MaxLength(1024)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Файл картинки")]
        [FileLength(5, "Файл должен быть не больше 5 мегабайт")]
        [ImageExtensions("jpg|jpeg|png", "Это должен быть графический файл с расширением .jpg, .jpeg или .png")]
        public HttpPostedFileBase File { get; set; }
    }
}
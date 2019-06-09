using System;
using System.ComponentModel.DataAnnotations;

namespace InternetPhotoAlbum.DAL.Entities
{
    public class ImageDescription
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(255)]
        public string FileName { get; set; }
        [MaxLength(8)]
        public string FileExtention { get; set; }
        [MaxLength(260)]
        public string FileStoreName { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string Description { get; set; }

        public DateTime UploadDate { get; set; }

        public bool IsAvatar { get; set; }

        [MaxLength(128)]
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}

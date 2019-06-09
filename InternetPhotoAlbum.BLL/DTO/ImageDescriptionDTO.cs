using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternetPhotoAlbum.DAL.Entities;

namespace InternetPhotoAlbum.BLL.DTO
{
    public class ImageDescriptionDTO
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileExtention { get; set; }
        public string FileStoreName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsAvatar { get; set; }
        public string OwnerId { get; set; }
    }
}

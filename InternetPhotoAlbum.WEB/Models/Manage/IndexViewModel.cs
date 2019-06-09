using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace InternetPhotoAlbum.WEB.Models
{
    public class IndexViewModel : SharedLayoutViewModel
    {
        public bool HasPassword { get; set; }
        public ImageViewModel Avatar { get; set; }
    }
}
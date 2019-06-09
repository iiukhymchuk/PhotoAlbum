using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetPhotoAlbum.WEB.Models
{
    public class HomeIndexViewModel : SharedLayoutViewModel
    {
        public List<Tuple<ImageViewModel, ImageViewModel>> ImagesWithAvatars { get; set; }
    }
}
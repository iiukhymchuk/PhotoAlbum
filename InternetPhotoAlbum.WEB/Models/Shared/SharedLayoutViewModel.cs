using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetPhotoAlbum.WEB.Models
{
    public class SharedLayoutViewModel
    {
        public string CurrentPageSpaceId { get; set; }
        public string UserSpaceId { get; set; }
        public string UserNickName { get; set; }
        public string CurrentController { get; set; }
        public string SearchString { get; set; }
    }
}
using System.Collections.Generic;

namespace InternetPhotoAlbum.WEB.Models
{
    public class UserSpaceIndexViewModel : SharedLayoutViewModel
    {
        public ImageViewModel Image { get; set; }
        public IEnumerable<ImageViewModel> ImagesOfThePage { get; set; }
        public ImageViewModel Avatar { get; set; }
    }
}
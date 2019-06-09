using InternetPhotoAlbum.BLL.DTO;
using System;
using System.Collections.Generic;


namespace InternetPhotoAlbum.BLL.Interfaces
{
    public interface IImageService : IDisposable
    {
        IEnumerable<ImageDescriptionDTO> GetAllUserImages(string userId = null);
        bool IsUserHasImage(string fileId, string userId);
        ImageDescriptionDTO GetImageById(string imageId);
        IEnumerable<ImageDescriptionDTO> Search(string query, string userId = null);
        ImageDescriptionDTO GetUserAvatar(string userId);
        void DeleteOldAvatar(string userId);
        IEnumerable<Tuple<ImageDescriptionDTO, ImageDescriptionDTO>> GetAllImagesWithAvatars();
        IEnumerable<Tuple<ImageDescriptionDTO, ImageDescriptionDTO>> SearchWithAvatars(string searchString);
        void DeleteImage(string imageStoreName, string imageOriginalPathAndName, string imageResizedPathAndName);
        string GetOriginalFileName(string imageStoreName);
    }
}
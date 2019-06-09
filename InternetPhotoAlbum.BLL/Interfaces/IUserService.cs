using System.Collections.Generic;
using InternetPhotoAlbum.BLL.DTO;
using System;
using System.IO;

namespace InternetPhotoAlbum.BLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        ApplicationUserDTO GetUserById(string Id);
        ApplicationUserDTO GetUserByUserSpaceId(int Id);
        IEnumerable<ImageDescriptionDTO> GetUserImages(string Id);
        IEnumerable<ImageDescriptionDTO> GetUserImagesBySpaceId(string spaceId);
        bool SaveImage(ref ImageDescriptionDTO image, Stream imageStream, string saveOriginalPath,
            string saveResizedPath);
        bool IsUserWithSpaceId(int spaceId);
        string GetUserIdBySpaceId(string spaceId);
        void AddImageToUser(ImageDescriptionDTO image, string userId);
        void ChangeNickName(string userId, string newNickName);
    }
}
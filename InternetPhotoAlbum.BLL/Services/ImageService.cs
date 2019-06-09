using System;
using System.Collections.Generic;
using System.Linq;

using InternetPhotoAlbum.BLL.Interfaces;
using InternetPhotoAlbum.DAL.Interfaces;
using InternetPhotoAlbum.DAL.Entities;
using InternetPhotoAlbum.BLL.DTO;
using System.IO;
using AutoMapper;

namespace InternetPhotoAlbum.BLL.Services
{
    public class ImageService : IImageService
    {
        #region Initialize
        private ImageDescriptionDTO defaultAvatar = new ImageDescriptionDTO()
        {
            FileName = "HeShe.gif",
            FileExtention = ".gif",
            FileStoreName = "avatar.gif",
            Name = "Default Avatar",
            Description = "Default Avatar",
            UploadDate = default(DateTime),
            OwnerId = "0ae0c57d-383a-4b47-9da9-2ffe55130ddf"
        };

        IRepository<ImageDescription> repository;

        public ImageService(IRepository<ImageDescription> repository)
        {
            this.repository = repository;
        }
        #endregion

        public IEnumerable<Tuple<ImageDescriptionDTO, ImageDescriptionDTO>> GetAllImagesWithAvatars()
        {
            var result = new List<Tuple<ImageDescriptionDTO, ImageDescriptionDTO>>();
            foreach (var image in GetAllUserImages(null))
            {
                var avatar = GetUserAvatar(image.OwnerId);
                result.Add(Tuple.Create(avatar, image));
            }
            return result;
        }

        public IEnumerable<ImageDescriptionDTO> GetAllUserImages(string userId = null)
        {
            var result = new List<ImageDescription>();
            if (userId == null)
            {
                result = repository.GetList.Where(img => !img.IsAvatar)
                    .OrderByDescending(img => img.UploadDate).ToList();
            }
            else
            {
                result = repository.GetList.Where(img => (img.Owner.Id == userId)
                    && (!img.IsAvatar))
                    .OrderByDescending(img => img.UploadDate).ToList();
            }
            Mapper.CreateMap<ImageDescription, ImageDescriptionDTO>();
            return Mapper.Map<IEnumerable<ImageDescription>, List<ImageDescriptionDTO>>(result);
        }

        public void DeleteImage(string imageStoreName, string imageOriginalPathAndName, string imageResizedPathAndName)
        {
            var imageId = Path.GetFileNameWithoutExtension(imageStoreName);
            bool isImageDeleted = true;
            try
            {
                File.Delete(imageOriginalPathAndName);
                File.Delete(imageResizedPathAndName);
            }
            catch (IOException)
            {
                isImageDeleted = false;
            }
            if (isImageDeleted)
            {
                repository.Delete(imageId);
                repository.Save();
            }
        }

        public string GetOriginalFileName(string imageStoreName)
        {
            if (imageStoreName == null)
            {
                return null;
            }
            var listOfImages = repository.GetList.Where(img => img.FileStoreName == imageStoreName);
            if (listOfImages == null)
            {
                return null;
            }
            var fileName = listOfImages.First().FileName;
            if (fileName == null)
            {
                return null;
            }
            return fileName;
        }

        public void DeleteOldAvatar(string userId)
        {
            var oldAvatars = repository.GetList.Where(img => (img.OwnerId == userId) && img.IsAvatar);
            if (oldAvatars != null)
            {
                // Нужно сделать oldAvatars без "lazy loading". Это приводит к не закрытому DataReader
                // http://stackoverflow.com/questions/4867602/entity-framework-there-is-already-an-open-datareader-associated-with-this-comma
                // Array не будет большим так как мы ожидаем всего один аватар при правильном развитии событий
                foreach (var avatar in oldAvatars.ToArray())
                {
                    repository.Delete(avatar.Id.ToString());
                }
                repository.Save();
            }
        }

        public bool IsUserHasImage(string imageId, string userId)
        {
            return repository.Get(imageId).Owner.Id == userId;
        }

        public ImageDescriptionDTO GetImageById(string imageId)
        {
            Mapper.CreateMap<ImageDescription, ImageDescriptionDTO>();
            return Mapper.Map<ImageDescription, ImageDescriptionDTO>(repository.Get(imageId));
        }

        public IEnumerable<Tuple<ImageDescriptionDTO, ImageDescriptionDTO>> SearchWithAvatars(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return null;
            }
            var result = new List<Tuple<ImageDescriptionDTO, ImageDescriptionDTO>>();
            foreach (var image in Search(searchString, null))
            {
                var avatar = GetUserAvatar(image.OwnerId);
                result.Add(Tuple.Create(avatar, image));
            }
            return result;
        }

        public IEnumerable<ImageDescriptionDTO> Search(string searchString, string userId = null)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return null;
            }
            Mapper.CreateMap<ImageDescription, ImageDescriptionDTO>();
            searchString = searchString.Trim().ToLower();
            List<ImageDescription> result = new List<ImageDescription>();
            if (userId == null)
            {
                result = repository.GetList.Where(img =>
                (img.Description.ToLower().Contains(searchString)
                || img.Name.ToLower().Contains(searchString))
                && !img.IsAvatar)
                .OrderByDescending(img => img.UploadDate).ToList();
            }
            else
            {
                result = repository.GetList.Where(img =>
                img.OwnerId == userId
                && (img.Description.ToLower().Contains(searchString)
                || img.Name.ToLower().Contains(searchString))
                && !img.IsAvatar)
                .OrderByDescending(img => img.UploadDate).ToList();
            }
            return Mapper.Map<IEnumerable<ImageDescription>, List<ImageDescriptionDTO>>(result);
        }

        public ImageDescriptionDTO GetUserAvatar(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return null;
            }
            Mapper.CreateMap<ImageDescription, ImageDescriptionDTO>();
            var avatar = repository.GetList.Where(img => (img.IsAvatar) && (img.OwnerId == userId))
                .FirstOrDefault();
            if (avatar == null)
            {
                var image = repository.GetList.Where(img => img.OwnerId == userId).FirstOrDefault();
                if (image == null)
                {
                    defaultAvatar.Name = "0";
                    defaultAvatar.Description = "";
                    return defaultAvatar;
                }
                var user = image.Owner;
                var avatarName = user.UserSpaceId.ToString();
                var avatarDescription = user.NickName;
                defaultAvatar.Name = avatarName;
                defaultAvatar.Description = avatarDescription;
                return defaultAvatar;
            }
            return Mapper.Map<ImageDescription, ImageDescriptionDTO>(avatar);
        }


        #region IDisposable
        bool disposed = false;

        public virtual void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (repository != null)
                {
                    repository.Dispose();
                    repository = null;
                }
            }
            disposed = true;
        }
        #endregion
    }
}

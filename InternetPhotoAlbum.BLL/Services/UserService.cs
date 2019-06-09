using AutoMapper;
using InternetPhotoAlbum.BLL.DTO;
using InternetPhotoAlbum.BLL.Interfaces;
using InternetPhotoAlbum.DAL.Entities;
using InternetPhotoAlbum.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web.Helpers;

namespace InternetPhotoAlbum.BLL.Services
{
    public class UserService : IUserService
    {
        #region Initialize
        private List<ImageDescriptionDTO> emptyImageList = new List<ImageDescriptionDTO>();

        IRepository<ApplicationUser> repository;
        public UserService(IRepository<ApplicationUser> repository)
        {
            this.repository = repository;
        }

        #endregion

        #region Methods
        public ApplicationUserDTO GetUserById(string Id)
        {
            Mapper.CreateMap<ApplicationUser, ApplicationUserDTO>();
            var userFromRepository = repository.Get(Id);
            var result = Mapper.Map<ApplicationUser, ApplicationUserDTO>(userFromRepository);
            return result;
        }

        public ApplicationUserDTO GetUserByUserSpaceId(int spaceId)
        {
            Mapper.CreateMap<ApplicationUser, ApplicationUserDTO>();
            return Mapper.Map<ApplicationUser, ApplicationUserDTO>(repository.GetList.FirstOrDefault(u => u.UserSpaceId == spaceId));
        }

        public bool IsUserWithSpaceId(int spaceId)
        {
            return GetUserByUserSpaceId(spaceId) != null;
        }

        public IEnumerable<ImageDescriptionDTO> GetUserImages(string Id)
        {
            Mapper.CreateMap<ImageDescription, ImageDescriptionDTO>();
            return Mapper.Map<IEnumerable<ImageDescription>, List<ImageDescriptionDTO>>(repository
                .Get(Id).UserImages.Where(img => !img.IsAvatar).OrderByDescending(u => u.UploadDate)).ToList();
        }

        public IEnumerable<ImageDescriptionDTO> GetUserImagesBySpaceId(string spaceId)
        {
            int Id;
            bool goodIdParseResult = Int32.TryParse(spaceId, out Id);
            if (goodIdParseResult)
            {
                var user = repository.GetList.FirstOrDefault(u => u.UserSpaceId == Id);
                if (user == null) return emptyImageList;
                Mapper.CreateMap<ImageDescription, ImageDescriptionDTO>();
                return Mapper.Map<IEnumerable<ImageDescription>, List<ImageDescriptionDTO>>(user
                    .UserImages.Where(img => !img.IsAvatar).OrderByDescending(u => u.UploadDate)).ToList();
            }
            return emptyImageList;
        }

        /// <summary>
        /// Сохраняет картинку пользователя на сервер как в оригинальном виде так и в ресайзинге.
        /// Заносит сведения в базу данных
        /// </summary>
        /// <returns>
        /// Возвращает true если операция сохранения картинки на сервер и инфо в базу данных.
        /// В ином случае возвращаем false.
        /// </returns>
        public bool SaveImage(ref ImageDescriptionDTO image, Stream imageStream, string saveOriginalPath,
            string saveResizedPath)
        {
            int imageLength = 800;
            int avatarLength = 150;
            int usedLength = image.IsAvatar ? avatarLength : imageLength;

            FileInfo fileInfo = new FileInfo(image.FileName);

            Guid guid = Guid.NewGuid();
            string stringGuid = guid.ToString();
            string extension = fileInfo.Extension.ToLower();
            string saveName = image.IsAvatar ? (image.OwnerId + extension) : (stringGuid + extension);
            string originalImageDestinationPathWithName = Path.Combine(saveOriginalPath, saveName);
            string resizedImageDestinationPathWithName = Path.Combine(saveResizedPath, saveName);

            image.Id = guid;
            image.FileExtention = extension;
            image.FileStoreName = saveName;
            image.UploadDate = DateTime.Now;

            // При ошибке выдаем результат false
            try
            {
                using (var originalImage = Image.FromStream(imageStream))
                {
                    originalImage.Save(originalImageDestinationPathWithName);
                    WebImage resizedImage = new WebImage(imageStream);
                    // Делаем ресайзинг только если картинка больше необходимой
                    // по ширине или высоте
                    if (resizedImage.Width > usedLength || resizedImage.Height > usedLength)
                    {
                        resizedImage.Resize(usedLength, usedLength, true, true);
                    }
                    resizedImage.Save(resizedImageDestinationPathWithName);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetUserIdBySpaceId(string spaceId)
        {
            var partialResult = repository.GetList.Where(usr => (usr.UserSpaceId.ToString() == spaceId));
            if (partialResult == null || partialResult.Count() == 0)
            {
                return null;
            }
            return partialResult.FirstOrDefault().Id;
        }

        public void AddImageToUser(ImageDescriptionDTO image, string userId)
        {
            if (image == null)
            {
                throw new ArgumentNullException("Параметр image не может быть null");
            }
            Mapper.CreateMap<ImageDescriptionDTO, ImageDescription>();
            var img = Mapper.Map<ImageDescriptionDTO, ImageDescription>(image);
            var user = repository.Get(userId);
            if (user == null)
            {
                throw new ArgumentException("Нет такого пользователя в базе данных");
            }
            user.UserImages.Add(img);
            repository.Save();
        }

        public void ChangeNickName(string userId, string newNickName)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Id пользователя не может быть пустым или null");
            }
            if (String.IsNullOrEmpty(newNickName))
            {
                throw new ArgumentNullException("Имя пользователя не может быть пустым или null");
            }
            var user = repository.Get(userId);
            if (user == null)
            {
                throw new ArgumentException("Нет такого пользователя в базе данных");
            }
            user.NickName = newNickName;
            repository.Save();
        }

        #endregion

        #region Helpers

        private Image Resize(Image image, float width)
        {
            float sourceWidth = image.Width;
            float sourceHeight = image.Height;
            float destinationHeight = (float)(sourceHeight * width / sourceWidth);
            float destinationWidth = width;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            Bitmap bitmap = new Bitmap((int)destinationWidth, (int)destinationHeight, PixelFormat.Format32bppPArgb);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            Graphics graphicsPhoto = Graphics.FromImage(bitmap);
            graphicsPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            graphicsPhoto.DrawImage(image,
                new Rectangle(destX, destY, (int)destinationWidth, (int)destinationHeight),
                new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                GraphicsUnit.Pixel);

            graphicsPhoto.Dispose();

            return bitmap;
        }
        #endregion

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
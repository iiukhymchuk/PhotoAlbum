using InternetPhotoAlbum.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using InternetPhotoAlbum.BLL.Interfaces;
using InternetPhotoAlbum.BLL.DTO;
using System.Web.Routing;
using AutoMapper;
using System.IO;

namespace InternetPhotoAlbum.WEB.Controllers
{
    public class BaseController : Controller
    {

        #region Initialization
        protected SharedLayoutViewModel baseModel = new SharedLayoutViewModel();
        protected IImageService imageService;
        protected IUserService userService;

        public BaseController(IImageService imageService, IUserService userService)
        {
            this.imageService = imageService;
            this.userService = userService;
        }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            baseModel.UserSpaceId = GetCurrentUserSpaceId(requestContext);
            baseModel.UserNickName = GetCurrentUserNickName(requestContext);
            baseModel.CurrentController = GetCurrentController(requestContext);
            baseModel.CurrentPageSpaceId = Convert.ToString(RouteData.Values["id"]);
        }
        #endregion

        #region Helpers
        private string GetCurrentUserSpaceId(RequestContext requestContext)
        {
            var currentUser = GetCurrentUser(requestContext);
            if (currentUser == null) return null;
            return currentUser.UserSpaceId.ToString();
        }

        private string GetCurrentUserNickName(RequestContext requestContext)
        {
            var currentUser = GetCurrentUser(requestContext);
            if (currentUser == null) return null;
            return currentUser.NickName.ToString();
        }

        private ApplicationUserDTO GetCurrentUser(RequestContext requestContext)
        {
            if (!requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }
            var userId = requestContext.HttpContext.User.Identity.GetUserId();
            return userService.GetUserById(userId);
        }

        private string GetCurrentController(RequestContext requestContext)
        {
            return requestContext.RouteData.Values["controller"].ToString();
        }

        // Свойства, которые задаются в класах-наследниках
        // Эти свойства нужны при сохранении картинок на сервер
        // методом защищенным методом TrySaveImage()
        protected virtual string FileName { get; set; }
        protected virtual string Name { get; set; }
        protected virtual string Description { get; set; }
        protected virtual string SaveOriginalPath { get; set; }
        protected virtual string SaveResizedPath { get; set; }
        protected virtual bool IsAvatar { get; set; }

        /// <summary>
        /// Метод для обновления данных в модели из данных базовой модели.
        /// 
        /// </summary>
        /// <param name="model">
        /// Данные которой хотим обновить
        /// </param>
        protected virtual void JoinWithBaseModel(SharedLayoutViewModel model)
        {
            model.CurrentController = baseModel.CurrentController;
            model.UserNickName = baseModel.UserNickName;
            model.UserSpaceId = baseModel.UserSpaceId;
            model.CurrentPageSpaceId = baseModel.CurrentPageSpaceId;
        }

        protected virtual ImageViewModel MapImageResult(ImageDescriptionDTO image)
        {
            Mapper.CreateMap<ImageDescriptionDTO, ImageViewModel>();
            return Mapper.Map<ImageDescriptionDTO, ImageViewModel>(image);
        }

        protected virtual List<ImageViewModel> MapImageResults(IEnumerable<ImageDescriptionDTO> imagesList)
        {
            if (imagesList == null) return new List<ImageViewModel>();
            Mapper.CreateMap<ImageDescriptionDTO, ImageViewModel>();
            return Mapper.Map<IEnumerable<ImageDescriptionDTO>, List<ImageViewModel>>(imagesList).ToList();
        }

        protected virtual List<Tuple<ImageViewModel, ImageViewModel>> MapImagesWithAvatarsResults(IEnumerable<Tuple<ImageDescriptionDTO, ImageDescriptionDTO>> imagesWithAvatarsList)
        {
            if (imagesWithAvatarsList == null) return new List<Tuple<ImageViewModel, ImageViewModel>>();
            var result = new List<Tuple<ImageViewModel, ImageViewModel>>();
            foreach (var tuple in imagesWithAvatarsList)
            {
                var item1 = MapImageResult(tuple.Item1);
                var item2 = MapImageResult(tuple.Item2);
                result.Add(Tuple.Create(item1, item2));
            }
            return result;
        }

        protected virtual bool TrySaveImage(Stream stream)
        {
            ImageDescriptionDTO image = new ImageDescriptionDTO()
            {
                FileName = FileName,
                Name = Name,
                Description = Description,
                IsAvatar = IsAvatar,
                OwnerId = User.Identity.GetUserId()
            };
            bool result = userService.SaveImage(ref image, stream, SaveOriginalPath, SaveResizedPath);
            if (result == false)
            {
                return false;
            }
            // В ином случае мы добавляем записи в базу данных
            if (image.IsAvatar)
            {
                imageService.DeleteOldAvatar(image.OwnerId);
            }
            userService.AddImageToUser(image, image.OwnerId);
            return true;
        }

        protected virtual ImageViewModel GetCurrentUserAvatar()
        {
            var userId = User.Identity.GetUserId();
            Mapper.CreateMap<ImageDescriptionDTO, ImageViewModel>();
            return Mapper.Map<ImageDescriptionDTO, ImageViewModel>(imageService.GetUserAvatar(userId));
        }

        protected virtual void DeleteImageBase(string imageStoreName)
        {
            string imageName = "";
            imageName = imageStoreName;
            string imageOriginalPathAndName = Request.MapPath("~/Content/Images/Originals/" + imageName);
            string imageResizedPathAndName = Request.MapPath("~/Content/Images/Resized/" + imageName);

            if (System.IO.File.Exists(imageOriginalPathAndName) && System.IO.File.Exists(imageResizedPathAndName))
            {
                imageService.DeleteImage(imageStoreName, imageOriginalPathAndName, imageResizedPathAndName);
            }
        }

        protected virtual FileResult DownloadImageBase(string imageStoreName)
        {
            string imageName = "";
            imageName = imageStoreName;
            string imageOriginalPathAndName = Request.MapPath("~/Content/Images/Originals/" + imageName);

            if (System.IO.File.Exists(imageOriginalPathAndName))
            {
                string fileName = imageService.GetOriginalFileName(imageName);
                return File(imageOriginalPathAndName, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return null;
        }

        #endregion

        #region IDisposable
        bool disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (imageService != null)
                {
                    imageService.Dispose();
                    imageService = null;
                }
                if (userService != null)
                {
                    userService.Dispose();
                    userService = null;
                }
            }
            base.Dispose(disposing);
            disposed = true;
        }
        #endregion
    }
}
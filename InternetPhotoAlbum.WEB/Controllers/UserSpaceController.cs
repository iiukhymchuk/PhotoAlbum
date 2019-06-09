using AutoMapper;
using InternetPhotoAlbum.BLL.Interfaces;
using InternetPhotoAlbum.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using InternetPhotoAlbum.WEB.Models;

namespace InternetPhotoAlbum.WEB.Controllers
{
    public class UserSpaceController : BaseController
    {
        #region Initialization
        // Свойства для сохранения картинок
        protected override string FileName { get; set; }
        protected override string Name { get; set; }
        protected override string Description { get; set; }
        private string saveOriginalPath = "";
        protected override string SaveOriginalPath
        {
            get { return Server.MapPath("~/Content/Images/Originals/"); }
            set { saveOriginalPath = value; }
        }
        private string saveResizedlPath = "";
        protected override string SaveResizedPath
        {
            get { return Server.MapPath("~/Content/Images/Resized/"); }
            set { saveResizedlPath = value; }
        }
        private bool isAvatar = false;
        protected override bool IsAvatar
        {
            get { return false; }
            set { isAvatar = value; }
        }
        public UserSpaceController(IImageService imageService, IUserService userService) :
            base(imageService, userService)
        {
        }

        #endregion

        #region Index
        // GET: UserSpace/Index/id
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index_Get(UserSpaceIndexViewModel model)
        {
            ViewBag.Title = "User Space";
            JoinWithBaseModel(model);
            SetupUserSpaceIndexViewModel(model);

            // Перенаправляем на свою страницу, если не указан {id} прямо в uri
            if (RouteData.Values["id"] == null && model.UserSpaceId != null)
            {
                RouteData.Values["id"] = model.UserSpaceId;
                return RedirectToAction("Index", new { id = model.UserSpaceId });
            }

            // Если неправильный {id} в uri (не число) или нет такой страницы пользовалеля (что тоже неправильный uri)
            // Перенаправить на NotFoundPage
            if (model.Avatar == null || !IsPageExist(Convert.ToString(RouteData.Values["id"])))
            {
                return Redirect("~/Error/NotFoundPage");
            }
            return View(model);
        }


        // POST: UserSpace/Index/id
        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        // Исключаем параметр роута id из проверки
        public ActionResult Index_Post([Bind(Include = "Image")] UserSpaceIndexViewModel model)
        {
            ViewBag.Title = "User Space";
            JoinWithBaseModel(model);
            SetupUserSpaceIndexViewModel(model);

            // Если неправильный id в uri (не число) или нет такой страницы пользовалеля (что тоже неправильный uri)
            // Перенаправить на NotFoundPage
            if (!IsPageExist(Convert.ToString(model.UserSpaceId)))
            {
                return Redirect("~/Error/NotFoundPage");
            }

            // Такого быть не должно, добавлять картинки можно только на своей странице
            // Перенаправить на свою страницу для добавления картинки
            if (model.CurrentPageSpaceId != model.UserSpaceId)
            {
                RouteData.Values["id"] = model.UserSpaceId;
                return RedirectToAction("Index");
            }

            // Если модель не валидируется, перенаправить назад
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Это мы проверяли при валидации, но проверим еще раз (если валидация изменится в будущем)
            if (model.Image.File != null && model.Image.File.ContentLength > 0)
            {
                FileName = model.Image.File.FileName;
                Name = model.Image.Name;
                Description = model.Image.Description;
                bool goodResult;
                using (Stream stream = model.Image.File.InputStream)
                {
                    goodResult = TrySaveImage(stream);
                }
                if (!goodResult)
                {
                    ModelState.AddModelError("Image.Image.File", "Что-то пошло не так с загрузкой картинки");
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Search
        [HttpGet]
        [ActionName("Search")]
        public ActionResult Search_Get(UserSpaceIndexViewModel model)
        {
            JoinWithBaseModel(model);
            return PartialView("_Search", model);
        }

        [HttpPost]
        [ActionName("Search")]
        [ValidateAntiForgeryToken]
        public ActionResult Search(UserSpaceIndexViewModel model)
        {
            if (!String.IsNullOrWhiteSpace(model.SearchString))
            {
                JoinWithBaseModel(model);
                var userId = userService.GetUserByUserSpaceId(Int32.Parse(model.CurrentPageSpaceId)).Id;
                var searchlist = imageService.Search(model.SearchString, userId);
                List<ImageViewModel> images = MapImageResults(searchlist);
                model.ImagesOfThePage = images;
                model.Avatar = GetUserAvatar(model.CurrentPageSpaceId);
                return PartialView("_Images", model);
            }
            return PartialView("_Images", model);
        }
        #endregion

        #region DeleteImage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteImage(string imageStoreName)
        {
            DeleteImageBase(imageStoreName);
            return RedirectToAction("Index");
        }

        #endregion

        #region DownloadImage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadImage(string imageStoreName)
        {
            return DownloadImageBase(imageStoreName);
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
            base.Dispose(disposing);
            disposed = true;
        }
        #endregion

        #region Helpers
        private bool IsPageExist(string spaceId)
        {
            int id = 0;
            bool goodResult = int.TryParse(spaceId, out id);
            if (goodResult)
            {
                return userService.IsUserWithSpaceId(id);
            }
            return false;
        }
        private void SetupUserSpaceIndexViewModel(UserSpaceIndexViewModel model)
        {
            model.ImagesOfThePage = GetUserImages(model.CurrentPageSpaceId);
            model.Avatar = GetUserAvatar(model.CurrentPageSpaceId);
        }

        private ImageViewModel GetUserAvatar(string spaceId)
        {
            string userId = userService.GetUserIdBySpaceId(spaceId);
            if (String.IsNullOrEmpty(userId))
            {
                return null;
            }
            else
            {
                return MapImageResult(imageService.GetUserAvatar(userId));
            }
        }

        private List<ImageViewModel> GetUserImages(string pageSpaceId)
        {
            // Вернуть пустой список, если id не существует
            if (String.IsNullOrEmpty(pageSpaceId)) return new List<ImageViewModel>();
            var resultList = userService.GetUserImagesBySpaceId(pageSpaceId);
            return MapImageResults(resultList);
        }

        #endregion
    }
}
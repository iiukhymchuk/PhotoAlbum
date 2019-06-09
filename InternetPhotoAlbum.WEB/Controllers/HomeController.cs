using AutoMapper;
using InternetPhotoAlbum.BLL.Interfaces;
using InternetPhotoAlbum.DAL.EF;
using InternetPhotoAlbum.DAL.Entities;
using InternetPhotoAlbum.WEB.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetPhotoAlbum.WEB.Controllers
{
    public class HomeController : BaseController
    {

        #region Initialization
        public HomeController(IImageService imageService, IUserService userService) :
            base(imageService, userService)
        {
            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                roleManager.Create(new IdentityRole("Admin"));

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                //var user = userManager.FindByEmail("myEmail@company.com");
                //userManager.AddToRole(user.Id, "Admin");
                context.SaveChanges();
            }
        }

        #endregion

        #region Index
        // GET: Home/Index/
        [HttpGet]
        [ActionName("Index")]
        public ActionResult Index_Get(HomeIndexViewModel model)
        {
            ViewBag.Title = "Index";
            JoinWithBaseModel(model);
            model.ImagesWithAvatars = MapImagesWithAvatarsResults(imageService.GetAllImagesWithAvatars()).ToList();
            return View(model);
        }

        // POST: Home/Index/
        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Index_Post(HomeIndexViewModel model)
        {
            ViewBag.Title = "Index";
            return RedirectToAction("Index", model);
        }

        #endregion

        #region Search
        // GET: Home/Search/
        [HttpGet]
        [ActionName("Search")]
        public ActionResult Search_Get(HomeIndexViewModel model)
        {
            ViewBag.Title = "Index";
            JoinWithBaseModel(model);
            return PartialView("_Search");
        }

        // POST: Home/Index/
        [HttpPost]
        [ActionName("Search")]
        [ValidateAntiForgeryToken]
        public ActionResult Search_Post(HomeIndexViewModel model)
        {
            ViewBag.Title = "Index";
            if (!String.IsNullOrWhiteSpace(model.SearchString))
            {
                JoinWithBaseModel(model);
                //var userId = userService.GetUserByUserSpaceId(Int32.Parse(model.CurrentPageSpaceId)).Id;
                var searchlist = imageService.SearchWithAvatars(model.SearchString);
                var imagesWithAvatars = MapImagesWithAvatarsResults(searchlist);
                model.ImagesWithAvatars = imagesWithAvatars;
                return PartialView("_Images", model);
            }
            return PartialView("Error", model);
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
    }
}
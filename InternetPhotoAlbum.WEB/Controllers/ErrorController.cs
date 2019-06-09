using InternetPhotoAlbum.BLL.Interfaces;
using InternetPhotoAlbum.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace InternetPhotoAlbum.WEB.Controllers
{
    public class ErrorController : BaseController
    {

        public ErrorController(IImageService imageService, IUserService userService) :
            base(imageService, userService)
        {
        }

        // GET: Error
        public ActionResult NotFoundPage()
        {
            var model = new SharedLayoutViewModel();
            JoinWithBaseModel(model);
            Response.StatusCode = (int) HttpStatusCode.NotFound;
            return View(model);
        }

        public ActionResult UploadTooLarge()
        {
            var model = new SharedLayoutViewModel();
            JoinWithBaseModel(model);
            Response.StatusCode = (int) HttpStatusCode.NotFound;
            return View(model);
        }
    }
}
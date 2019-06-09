using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using InternetPhotoAlbum.WEB.Models;
using InternetPhotoAlbum.BLL.Services;
using InternetPhotoAlbum.DAL.Repositories;
using System.IO;
using InternetPhotoAlbum.BLL.DTO;
using AutoMapper;

namespace InternetPhotoAlbum.WEB.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        #region Initializing

        // Свойства для сохранения аватаров
        protected override string FileName { get; set; }
        protected override string Name { get; set; }
        protected override string Description { get; set; }
        private string saveOriginalPath = "";
        protected override string SaveOriginalPath
        {
            get { return Server.MapPath("~/Content/Images/Avatars/Originals/"); }
            set { saveOriginalPath = value; }
        }
        private string saveResizedlPath = "";
        protected override string SaveResizedPath
        {
            get { return Server.MapPath("~/Content/Images/Avatars/Resized/"); }
            set { saveResizedlPath = value; }
        }
        private bool isAvatar = false;
        protected override bool IsAvatar
        {
            get { return true; }
            set { isAvatar = value; }
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
            : base(new ImageService(new ImageRepository()), new UserService(new UserRepository()))
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : this()
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        #region Index
        //
        // GET: /Manage/Index
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                Avatar = GetCurrentUserAvatar()
            };
            JoinWithBaseModel(model);
            return View(model);
        }

        #endregion

        #region SetAvatar
        //
        // GET: /Manage/SetAvatar
        [HttpGet]
        [ActionName("SetAvatar")]
        public ActionResult SetAvatar_Get()
        {
            SetAvatarViewModel model = new SetAvatarViewModel();
            JoinWithBaseModel(model);
            return View(model);
        }

        //
        // POST: /Manage/SetAvatar
        [HttpPost]
        [ActionName("SetAvatar")]
        [ValidateAntiForgeryToken]
        public ActionResult SetAvatar_Post([Bind(Include = "File")]SetAvatarViewModel model)
        {
            JoinWithBaseModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Это мы проверяли при валидации, но проверим еще раз (если валидация изменится в будущем)
            if (model.File != null && model.File.ContentLength > 0)
            {
                FileName = model.File.FileName;
                Name =  model.UserSpaceId;
                Description = model.UserNickName;
                bool goodResult;
                using (Stream stream = model.File.InputStream)
                {
                    goodResult = TrySaveImage(stream);
                }
                // Если картинка не сохранена как следует, выдаем ошибку валидации 
                if (!goodResult)
                {
                    ModelState.AddModelError("Image.File", "Что-то пошло не так с загрузкой картинки");
                    return View(model);
                }
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region ChangePassword

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();
            JoinWithBaseModel(model);
            return View(model);
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            JoinWithBaseModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        #endregion

        #region ChangeNickName

        //
        // GET: /Manage/ChangeNickName
        public ActionResult ChangeNickName()
        {
            var model = new ChangeNickNameViewModel();
            JoinWithBaseModel(model);
            return View(model);
        }

        //
        // POST: /Manage/ChangeNickName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeNickName(ChangeNickNameViewModel model)
        {
            JoinWithBaseModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            bool goodResult = await UserManager.CheckPasswordAsync(user, model.OldPassword);
            if (!goodResult)
            {
                // Добавить ошибку
                return View(model);
            }
            userService.ChangeNickName(User.Identity.GetUserId(), model.NewNickName);
            return RedirectToAction("Index", new { Message = "Имя пользователя изменено" });
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}
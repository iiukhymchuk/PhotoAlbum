using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using InternetPhotoAlbum.WEB.Models;
using InternetPhotoAlbum.DAL.Entities;
using InternetPhotoAlbum.BLL.Services;
using InternetPhotoAlbum.DAL.Repositories;
using System.Data.Entity;
using InternetPhotoAlbum.DAL.EF;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;

namespace InternetPhotoAlbum.WEB.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {

        #region Initializing
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
            : base(new ImageService(new ImageRepository()), new UserService(new UserRepository()))
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager ) :
            this() 
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

        #region Actions
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            LoginViewModel model = new LoginViewModel();
            JoinWithBaseModel(model);
            // Если юзер уже зарегистрирован, то перенаправить на свою страницу
            if (model.UserNickName != null)
            {
                return RedirectToAction("Index", "UserSpace", new { id = model.UserSpaceId });
            }
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            JoinWithBaseModel(model);
            // Если юзер уже зарегистрирован, то перенаправить на свою страницу
            if (model.UserNickName != null)
            {
                return RedirectToAction("Index", "UserSpace", new { id = model.UserSpaceId });
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Нужно подтверждение почты, прежде чем иметь возможность залогиниться
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Photo Album Confirmation");
                    ViewBag.errorMessage = "You must have a confirmed email to log in.";
                    return View("Error", model);
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            JoinWithBaseModel(model);
            // Если юзер уже зарегистрирован, то перенаправить на свою страницу
            if (model.UserNickName != null)
            {
                return RedirectToAction("Index", "UserSpace", new { id = model.UserSpaceId });
            }
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            JoinWithBaseModel(model);
            // Если юзер уже зарегистрирован, то перенаправить на свою страницу
            if (model.UserNickName != null)
            {
                return RedirectToAction("Index", "UserSpace", new { id = model.UserSpaceId });
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, NickName = model.NickName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Пока пользователь не подтвержден мы его не логиним
                    // await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Photo Album Confirmation");

                    // Для дебагинга
                    // TempData["ViewBagLink"] = callbackUrl;

                    ViewBag.Message = "Проверте почту и подтвердите свой аккаунт. "
                         + "Ваш аккаунт должен быть подтвержден перед тем как войти.";

                    return View("Info", model);
                    // return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            var model = new SharedLayoutViewModel();
            JoinWithBaseModel(model);
            // Зарегистрировать пользователя автоматически
            var user = await UserManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userId == null || code == null || user == null)
            {
                return View("Error", model);
            }

            // Проверить ссылку и пользователя на корректность
            var cofirmationResult = await UserManager.ConfirmEmailAsync(userId, code);
            if (cofirmationResult.Succeeded)
            {
                // Залогиниваем пользователя
                var login = await LoginUserAndRedirect(user);
                return login;
            }
            return View("Error", model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel();
            JoinWithBaseModel(model);
            return View(model);
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            JoinWithBaseModel(model);
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Phot Album - Восстановление пароля", "Пожалуйста, перейдите по <a href=\"" + callbackUrl + "\">ссылке</a> для восстановления пароля");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            var model = new SharedLayoutViewModel();
            JoinWithBaseModel(model);
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            var model = new ResetPasswordViewModel();
            JoinWithBaseModel(model);
            var user = UserManager.FindById(userId);
            if (userId == null || code == null || user == null)
            {
                // Перенаправить на ошибку при неправильном адрессе
                return View("Error", model);
            }
            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string userId, string code, ResetPasswordViewModel model)
        {
            JoinWithBaseModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(userId);
            if (userId == null || code == null || user == null)
            {
                // Перенаправить на ошибку при неправильном адрессе
                return View("Error", model);
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                var login = await LoginUserAndRedirect(user);
                return login;
            }
            AddErrors(result);
            return View(model);
        }

        #endregion

        #region Roles

        // Вдохновлен самодельной админкой 
        // https://andersnordby.wordpress.com/2014/11/28/asp-net-mvc-4-5-owin-simple-roles-management/
        // Перейти на IdentityManager в будущем

        [Authorize(Roles = "Admin")]
        public ActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleCreate(string roleName)
        {
            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                roleManager.Create(new IdentityRole(roleName));
                context.SaveChanges();
            }

            ViewBag.ResultMessage = String.Format("Роль {0} успешно создана", roleName);
            return RedirectToAction("RoleIndex", "Account");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RoleIndex()
        {
            List<string> roles;
            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                roles = (from r in roleManager.Roles select r.Name).ToList();
            }

            return View(roles.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RoleDelete(string roleName)
        {
            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var role = roleManager.FindByName(roleName);

                roleManager.Delete(role);
                context.SaveChanges();
            }

            ViewBag.ResultMessage = String.Format("Роль пользователя {0} удалена успешно", roleName);
            return RedirectToAction("RoleIndex", "Account");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RoleAddToUser()
        {
            List<string> roles;
            List<string> users;
            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                users = (from u in userManager.Users select u.UserName).ToList();
                roles = (from r in roleManager.Roles select r.Name).ToList();
            }

            ViewBag.Roles = new SelectList(roles);
            ViewBag.Users = new SelectList(users);
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string roleName, string userName)
        {
            List<string> roles;
            List<string> users;
            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                users = (from u in userManager.Users select u.UserName).ToList();

                var user = userManager.FindByName(userName);
                if (user == null)
                    throw new Exception(String.Format("Нет такого пользователя {0}", userName));

                var role = roleManager.FindByName(roleName);
                if (role == null)
                    throw new Exception(String.Format("Нет такой роли {0}", roleName));

                if (userManager.IsInRole(user.Id, role.Name))
                {
                    ViewBag.ResultMessage = String.Format("У пользователя {0} уже есть эта роль {1}", userName, roleName);
                }
                else
                {
                    userManager.AddToRole(user.Id, role.Name);
                    context.SaveChanges();

                    ViewBag.ResultMessage = String.Format("Пользователю {0} успешно присвоена роль {1}", userName, roleName);
                }

                roles = (from r in roleManager.Roles select r.Name).ToList();
            }

            ViewBag.Roles = new SelectList(roles);
            ViewBag.Users = new SelectList(users);
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                List<string> userRoles;
                List<string> roles;
                List<string> users;
                using (var context = new ApplicationDbContext())
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    roles = (from r in roleManager.Roles select r.Name).ToList();

                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    users = (from u in userManager.Users select u.UserName).ToList();

                    var user = userManager.FindByName(userName);
                    if (user == null)
                        throw new Exception(String.Format("Пользователь {0} не найден", userName));

                    var userRoleIds = (from r in user.Roles select r.RoleId);
                    userRoles = (from id in userRoleIds
                                 let r = roleManager.FindById(id)
                                 select r.Name).ToList();
                }

                ViewBag.Roles = new SelectList(roles);
                ViewBag.Users = new SelectList(users);
                ViewBag.RolesForThisUser = userRoles;
            }

            return View("RoleAddToUser");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string userName, string roleName)
        {
            List<string> userRoles;
            List<string> roles;
            List<string> users;
            using (var context = new ApplicationDbContext())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                roles = (from r in roleManager.Roles select r.Name).ToList();

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                users = (from u in userManager.Users select u.UserName).ToList();

                var user = userManager.FindByName(userName);
                if (user == null)
                    throw new Exception(String.Format("Пользователь {0} не найден", userName));

                if (userManager.IsInRole(user.Id, roleName))
                {
                    userManager.RemoveFromRole(user.Id, roleName);
                    context.SaveChanges();

                    ViewBag.ResultMessage = String.Format("Роль {0} пользователя {1} успешно удалена", roleName, userName);
                }
                else
                {
                    ViewBag.ResultMessage = String.Format("Пользователь {0} не имеет данной роли {1}", userName, roleName);
                }

                var userRoleIds = (from r in user.Roles select r.RoleId);
                userRoles = (from id in userRoleIds
                             let r = roleManager.FindById(id)
                             select r.Name).ToList();
            }

            ViewBag.RolesForThisUser = userRoles;
            ViewBag.Roles = new SelectList(roles);
            ViewBag.Users = new SelectList(users);
            return View("RoleAddToUser");
        }

        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        /// <summary>
        /// Залогинить юзера и перенаправить его на страницу этого юзера
        /// </summary>
        /// <param name="user">
        /// 
        /// </param>
        /// <returns></returns>
        private async Task<ActionResult> LoginUserAndRedirect(ApplicationUser user)
        {
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToAction("Index", "UserSpace", new { id = user.UserSpaceId });
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Подтвердите, пожалуйста, Ваш аккаунт перейдя по <a href=\"" + callbackUrl + "\">ссылке</a>");

            return callbackUrl;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}
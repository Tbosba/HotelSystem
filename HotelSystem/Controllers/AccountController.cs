﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HotelSystem.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using HotelSystem.Models.Users;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;

namespace HotelSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //var user = await UserManager.FindByEmailAsync(model.Email);

            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            //if (roleManager.RoleExists("Customer"))
            //{
            //    if (user != null)
            //    {
            //        if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            //        {
            //            ModelState.AddModelError("", "You must have a confirmed email to log on.");
            //            return View();
            //        }
            //    }
            //}
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
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
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, ConfirmedEmail = false };
                var result = await UserManager.CreateAsync(user, model.Password);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                if (result.Succeeded)
                {
                    Customer customer = new Customer();

                   // user.Name = "Customer";

                    customer.CustomerID = DateTime.Now.Year.ToString() + new Random().Next(1, 9999).ToString();
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;
                    customer.ContactNo = model.ContactNo;
                    customer.Gender = model.Gender;
                    customer.Address = model.Address;
                    customer.Email = model.Email;
                    customer.Id = user.Id;
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    for (int i = 1; i == 1; i++)
                    {
                        MyBill myBill = new MyBill();

                        myBill.Email = customer.Email;
                        myBill.Name = customer.FirstName;
                        myBill.Surname = customer.LastName;
                        myBill.RegNo = customer.CustomerID;
                        myBill.Address = customer.Address;
                        myBill.billID = new Random().Next(1, 999).ToString();
                        myBill.Status = "No funds are due at the moment!";
                        myBill.BookingTotal = 0;
                        myBill.Discount = 0;
                        myBill.RoomServiceTotal = 0;
                        myBill.SpaTotal = 0;
                        myBill.LaundryTotal = 0;
                        myBill.Tax = 0;
                        myBill.TotalAmount = 0;

                        db.Bill.Add(myBill);
                        db.SaveChanges();
                    }

                    if (!roleManager.RoleExists("Customer"))
                    {
                        roleManager.Create(new IdentityRole("Customer"));
                    }
                    await UserManager.AddToRoleAsync(user.Id, "Customer");
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Confirm/ConfirmAccount.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{ConfirmationLink}", callbackUrl);
                    body = body.Replace("{UserName}", model.Email);
                    bool IsSendEmail = SendEmail.EmailSend(model.Email, "Confirm your account", body, true);

                    if (IsSendEmail)
                        return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AssistanceReg()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssistanceReg(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, ConfirmedEmail = false };
                var result = await UserManager.CreateAsync(user, model.Password);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                if (result.Succeeded)
                {
                    Assist customer = new Assist();

                   // user.FirstName = "Assistance";

                    customer.AssistanceID = DateTime.Now.Year.ToString() + new Random().Next(1, 9999).ToString();
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;
                    customer.ContactNo = model.ContactNo;
                    customer.Gender = model.Gender;
                    customer.Address = model.Address;
                    customer.Email = model.Email;
                    customer.Id = user.Id;
                    db.Assists.Add(customer);
                    db.SaveChanges();

                    if (!roleManager.RoleExists("Assistance"))
                    {
                        roleManager.Create(new IdentityRole("Assistance"));
                    }
                    await UserManager.AddToRoleAsync(user.Id, "Assistance");
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Confirm/ConfirmAccount.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{ConfirmationLink}", callbackUrl);
                    body = body.Replace("{UserName}", model.Email);
                    bool IsSendEmail = SendEmail.EmailSend(model.Email, "Confirm your account", body, true);

                    if (IsSendEmail)
                        return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[AllowAnonymous]
        //public ActionResult SpaWorkers()
        //{
        //    return View();
        //}
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SpaWorkers(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, ConfirmedEmail = false };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        //        if (result.Succeeded)
        //        {
        //            SpaWorkers spaworker = new SpaWorkers();

        //           // user.FirstName = "SpaWorkers";

        //            spaworker.StaffID = DateTime.Now.Year.ToString() + new Random().Next(1, 9999).ToString() + DateTime.Now.Day.ToString();
        //            spaworker.FirstName = model.FirstName;
        //            spaworker.LastName = model.LastName;
        //            spaworker.ContactNo = model.ContactNo;
        //            spaworker.Gender = model.Gender;
        //            spaworker.Email = model.Email;
        //            spaworker.Id = user.Id;
        //            db.SpaWorkers.Add(spaworker);
        //            db.SaveChanges();


        //            if (!roleManager.RoleExists("SpaWorkers"))
        //            {
        //                roleManager.Create(new IdentityRole("SpaWorkers"));
        //            }
        //            await UserManager.AddToRoleAsync(user.Id, "SpaWorkers");

        //            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //            string body = string.Empty;
        //            using (StreamReader reader = new StreamReader(Server.MapPath("~/Confirm/ConfirmAccount.html")))
        //            {
        //                body = reader.ReadToEnd();
        //            }
        //            body = body.Replace("{ConfirmationLink}", callbackUrl);
        //            body = body.Replace("{UserName}", model.Email);
        //            bool IsSendEmail = SendEmail.EmailSend(model.Email, "Confirm your account", body, true);

        //            if (IsSendEmail)
        //                return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //[AllowAnonymous]
        //public ActionResult LaundryMaid()
        //{
        //    return View();
        //}
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> LaundryMaid(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, ConfirmedEmail = false };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        //        if (result.Succeeded)
        //        {
        //            LaundryMaid laundryMaid = new LaundryMaid();

        //           // user.FirstName = "LaundryMaid";

        //            laundryMaid.StaffID = DateTime.Now.Year.ToString() + new Random().Next(1, 9999).ToString() + DateTime.Now.Day.ToString();
        //            laundryMaid.FirstName = model.FirstName;
        //            laundryMaid.LastName = model.LastName;
        //            laundryMaid.ContactNo = model.ContactNo;
        //            laundryMaid.Gender = model.Gender;
        //            laundryMaid.Email = model.Email;
        //            laundryMaid.Id = user.Id;
        //            db.LaundryMaid.Add(laundryMaid);
        //            db.SaveChanges();


        //            if (!roleManager.RoleExists("LaundryMaid"))
        //            {
        //                roleManager.Create(new IdentityRole("LaundryMaid"));
        //            }
        //            await UserManager.AddToRoleAsync(user.Id, "LaundryMaid");
        //            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //            string body = string.Empty;
        //            using (StreamReader reader = new StreamReader(Server.MapPath("~/Confirm/ConfirmAccount.html")))
        //            {
        //                body = reader.ReadToEnd();
        //            }
        //            body = body.Replace("{ConfirmationLink}", callbackUrl);
        //            body = body.Replace("{UserName}", model.Email);
        //            bool IsSendEmail = SendEmail.EmailSend(model.Email, "Confirm your account", body, true);

        //            if (IsSendEmail)
        //                return RedirectToAction("Login", "Account");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
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
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

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
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using HotelSystem.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HotelSystem
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
            CreateRolersandUsers();
        }
        private void CreateRolersandUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            //-----------------------------------------------------------------------------------------------------------------------------
            //Admin
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));

                var user = new ApplicationUser();
                user.Name = "Admin";
                user.UserName = "Admin@hotel.com";
                user.Email = "Admin@hotel.com";
                string pwd = "@Password01";

                var newuser = userManager.Create(user, pwd);
                if (newuser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }

            }

            //Receptionist
            if (!roleManager.RoleExists("Receptionist"))
            {
                roleManager.Create(new IdentityRole("Receptionist"));

                var user = new ApplicationUser();
                user.Name = "Receptionist";
                user.UserName = "Receptionist01@hotel.com";
                user.Email = "Receptionist01@hotel.com";
                string pwd = "@Password01";

                var newuser = userManager.Create(user, pwd);
                if (newuser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Receptionist");
                }

            }
            //--------------------------------------------------------------------------------------------------------------------------------
            //Laundry 
            //if (!roleManager.RoleExists("LaundryMaid"))
            //{
            //    roleManager.Create(new IdentityRole("LaundryMaid"));

            //    var user = new ApplicationUser();
            //    user.Name = "LaundryMaid";
            //    user.LastName = "Maid";
            //    user.ContactNo = 0638979623;
            //    user.UserName = "Kapenta@laundrymaid.com";
            //    user.Email = "Kapenta@laundrymaid.com";
            //    string pwd = "@Password01";

            //    var newuser = userManager.Create(user, pwd);
            //    if (newuser.Succeeded)
            //    {
            //        userManager.AddToRole(user.Id, "LaundryMaid");
            //    }

            //}
            //--------------------------------------------------------------------------------------------------------------------------------
            //Spa Service 
            //if (!roleManager.RoleExists("SpaWorkers"))
            //{
            //    roleManager.Create(new IdentityRole("SpaWorkers"));

            //    var user = new ApplicationUser();
            //    user.Name = "SpaWorkers";
            //    user.LastName = "Service";
            //    user.ContactNo = 0638979623;
            //    user.UserName = "Kapenta@spaworker.com";
            //    user.Email = "Kapenta@spaworker.com";
            //    string pwd = "@Password01";

            //    var newuser = userManager.Create(user, pwd);
            //    if (newuser.Succeeded)
            //    {
            //        userManager.AddToRole(user.Id, "SpaWorkers");
            //    }
            //}
            //--------------------------------------------------------------------------------------------------------------------------------
            //Delivery Guy 
            //if (!roleManager.RoleExists("DeliveryGuy"))
            //{
            //    roleManager.Create(new IdentityRole("DeliveryGuy"));

            //    var user = new ApplicationUser();
            //    user.FirstName = "Delivery";
            //    user.LastName = "Guy";
            //    user.ContactNo = 0638979623;
            //    user.UserName = "Kapenta@deliveryguy.com";
            //    user.Email = "Kapenta@deliveryguy.com";
            //    string pwd = "@Password01";

            //    var newuser = userManager.Create(user, pwd);
            //    if (newuser.Succeeded)
            //    {
            //        userManager.AddToRole(user.Id, "DeliveryGuy");
            //    }
            //}
        }
    }
}
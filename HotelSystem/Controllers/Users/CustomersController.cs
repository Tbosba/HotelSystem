using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelSystem.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using HotelSystem.Models;
using Microsoft.AspNet.Identity.Owin;

namespace HotelSystem.Controllers
{
    public class GuestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public GuestsController()
           : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }
        public GuestsController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        // GET: Guests
        public ActionResult GuestList(int? page)
        {
            Session["id"] = "";
            return View(db.Customers.ToList());
        }
        public ActionResult SingleGuest()
        {
            //var userId = User.Identity.Name;
            //var MemberMail = db.Users.ToList().Find(x => x.BlockId == HttpContext.User.Identity.Name).BlockId;

            //var guest = db.Guests;

            //return View(guest.Where(i => i.Email == MemberMail).ToList());
            var use = db.Users.ToList().Find(p => p.Email == User.Identity.Name);
            var InUse = db.Customers.Where(p => p.Email == use.Email);
            return View(InUse.ToList());    
        }

        // GET: Guests/GuestDetails/5
        public async Task<ActionResult> GuestDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Guests/NewGuest
        [AllowAnonymous]
        public ActionResult NewGuest()
        {
            return View();
        }

        // POST: Guests/NewGuest
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewGuest([Bind(Include = "GuestID,FirstName,LastName,Email,BirthDate,Gender,Title,Picture,ContactNo,Address,IdNumber,Passport,isSaCitizen,isActive,Password,ConfirmPassword,Points")] Customer customers, HttpPostedFileBase img_upload)
        {
            var countGuests = db.Customers.Count();
            customers.CustomerID = DateTime.Now.Year.ToString() + countGuests.ToString() + new Random().Next(1, 999).ToString();
            
            if (ModelState.IsValid)
            {
                //var user = guest.GetUser();
                //Guest dm = new Guest();

                //var result = await UserManager.CreateAsync(user, guest.Password);

                //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                //if (result.Succeeded)
                //{
                //    if (!roleManager.RoleExists("Guest"))
                //    {
                //        roleManager.Create(new IdentityRole("Guest"));
                //    }
                //    await UserManager.AddToRoleAsync(user.Id, "Guest");

                    db.Customers.Add(customers);
                    await db.SaveChangesAsync();

                    return RedirectToAction("GuestList");
                //}
            }
            return View(customers);
        }
        // GET: Guests/EditGuest/5
        public async Task<ActionResult> EditGuest(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer guest = await db.Customers.FindAsync(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // POST: Guests/EditGuest/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditGuest([Bind(Include = "GuestID,FirstName,LastName,Email,ContactNo,Address,Password,ConfirmPassword")] Customer guest)
        {

            if (ModelState.IsValid)
            {
                db.Entry(guest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("GuestList");
            }
            return View(guest);
        }

        // GET: Guests/DeleteGuest/5
        public async Task<ActionResult> DeleteGuest(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer guest = await db.Customers.FindAsync(id);
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        // POST: Guests/DeleteGuest/5
        [HttpPost, ActionName("DeleteGuest")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Customer guest = await db.Customers.FindAsync(id);
            db.Customers.Remove(guest);
            await db.SaveChangesAsync();
            return RedirectToAction("GuestList");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

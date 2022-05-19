using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelSystem.Models;
using Microsoft.AspNet.Identity;

namespace HotelSystem.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //public ActionResult RatingIndex(int? filter)
        //{
        //    if (filter > 0)
        //    {
        //        return View(db.RatingClass.Where(m => m.Rating == filter).ToList());
        //    }
        //    return View(db.RatingClass.ToList());
        //}
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Success() => View();
        public ActionResult Rating() => View();

        //[HttpPost]
        //public ActionResult Rating(int rating, string Comment)
        //{
        //    var ratingClass = new RatingClass();
        //    ratingClass.Id = Guid.NewGuid();
        //    ratingClass.Comment = Comment;
        //    ratingClass.Rating = rating;
        //    ratingClass.Date = DateTime.Today;
        //    db.RatingClass.Add(ratingClass);
        //    db.SaveChanges();
        //    return View("Success");
        //}
        public ActionResult Response(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatClass airports = db.ChatClass.Find(id);
            if (airports == null)
            {
                return HttpNotFound();
            }
            return View(airports);
        }

        // POST: Airports/Delete/5
        [HttpPost, ActionName("Response")]
        [ValidateAntiForgeryToken]
        public ActionResult ResponseConfirmed(Guid id, string message)
        {
            var idd = User.Identity.GetUserId();
            var FindUser = db.Users.Where(m => m.Id == idd);
            string NameFound = "";
            foreach (var item in FindUser)
            {
                NameFound = item.FirstName;
            }
            ChatClass airports = db.ChatClass.Find(id);
            ChatClass newChat = new ChatClass
            {
                Id = Guid.NewGuid(),
                ChatContent = message,
                Time = DateTime.Now,
                UserId = airports.UserId,
                FirstName = airports.FirstName,
                LastName = airports.LastName,
                ChatStatus = "Response",
                Employee = NameFound
            };
            db.ChatClass.Add(newChat);
            airports.ChatStatus = "Read";
            db.Entry(airports).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RespondChat");
        }
        public ActionResult ClearChat()
        {
            var id = User.Identity.GetUserId();
            var DeleteAll = db.ChatClass.Where(m => m.UserId == id);
            int DeleteAllCounter = db.ChatClass.Where(m => m.UserId == id).Count();
            for (int i = 1; i <= DeleteAllCounter; i++)
            {
                foreach (var item in DeleteAll)
                {
                    ChatClass deleteChat = db.ChatClass.Find(item.Id);
                    db.ChatClass.Remove(deleteChat);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Chat");
        }
        [Authorize]
        public ActionResult Chat()
        {
            var id = User.Identity.GetUserId();
            return View(db.ChatClass.Where(m => m.UserId == id).OrderBy(m => m.Time).ToList());
        }
        [Authorize]
        [HttpPost]
        public ActionResult Chat(string message)
        {
            var id = User.Identity.GetUserId();
            var FindUser = db.Users.Where(m => m.Id == id);
            string NameFound = "";
            foreach (var item in FindUser)
            {
                NameFound = item.FirstName;
            }
            if (!string.IsNullOrEmpty(message))
            {
                ChatClass chat = new ChatClass
                {
                    Id = Guid.NewGuid(),
                    ChatContent = message,
                    Time = DateTime.Now,
                    UserId = id,
                    FirstName = NameFound,
                    ChatStatus = "Not read"

                };
                db.ChatClass.Add(chat);
                db.SaveChanges();
            }
            return View(db.ChatClass.Where(m => m.UserId == id).OrderBy(m => m.Time).ToList());
        }
        public ActionResult RespondChat()
        {
            var id = User.Identity.GetUserId();
            return View(db.ChatClass.Where(m => m.ChatStatus.Contains("Not")).OrderBy(m => m.Time).ToList());
        }
        public ActionResult Index()
        {
            List<Hotels> hotels = db.Hotels.Where(h => h.HotelName != null).ToList()
                .Select(h => new Hotels()
                {
                    HotelId = h.HotelId,
                    HotelDescription = h.HotelDescription,
                    Address = h.Address,
                    HotelName = h.HotelName,
                    HotelPic = h.HotelPic,
                    TotalNumberOfRooms = h.TotalNumberOfRooms,
                }).ToList();
            ViewBag.HotelList = hotels;
            ViewBag.Towns = db.Hotels.ToList();
            ViewBag.Rooms = db.RoomTypes.ToList();
            return View();
        }
        public ActionResult ThankYou()
        {
            return View();
        }
        public ActionResult Routen()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            ViewBag.Age = DateTime.Now.Year - 1999;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult LocationIndex()
        {
            List<Hotels> hotels = db.Hotels.Where(h => h.HotelName != null).ToList()
                .Select(h => new Hotels()
                {
                    HotelId = h.HotelId,
                    HotelDescription = h.HotelDescription,
                    Address = h.Address,
                    HotelName = h.HotelName,
                    HotelPic = h.HotelPic,
                    TotalNumberOfRooms = h.TotalNumberOfRooms,
                }).ToList();
            ViewBag.HotelList = hotels;
            ViewBag.Towns = db.Hotels.ToList();
            ViewBag.Rooms = db.RoomTypes.ToList();
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelSystem.Models;
using Microsoft.AspNet.Identity;

namespace HotelSystem.Controllers
{
    public class HotelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Hotels
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();
            return View(db.Hotels.ToList().Where(x => x.ManagerEmail == userName));
        }
        public ActionResult HotelView()
        {
            return View(db.Hotels.ToList());
        }
        public ActionResult HotelView1(string search)
        {
            return View(db.Hotels.ToList().Where(x=>x.Address.Contains(search)));
        }
        // GET: Hotels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotels hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // GET: Hotels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HotelId,ManagerEmail,HotelName,street_number,route,locality,administrative_area_level_1,country,postal_code,Latitude,Longitude,NoOfFloors,TotalNumberOfRooms,HotelDescription,HotelPic,Address")] Hotels hotel, HttpPostedFileBase img_upload)
        {

            byte[] photo = null;
            photo = new byte[img_upload.ContentLength];
            img_upload.InputStream.Read(photo, 0, img_upload.ContentLength);
            hotel.HotelPic = photo;

            var userName = User.Identity.GetUserName();
            if (ModelState.IsValid)
            {
                //hotel.Address = ($"{hotel.street_number}, {hotel.route}, {hotel.locality}, {hotel.administrative_area_level_1}, {hotel.country}");
                hotel.ManagerEmail = userName;
                db.Hotels.Add(hotel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel);
        }

        // GET: Hotels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotels hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HotelId,ManagerEmail,HotelName,street_number,route,locality,administrative_area_level_1,country,postal_code,Latitude,Longitude,NoOfFloors,TotalNumberOfRooms,HotelDescription,HotelPic,Address")] Hotels hotel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotels hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hotels hotel = db.Hotels.Find(id);
            db.Hotels.Remove(hotel);
            db.SaveChanges();
            return RedirectToAction("Index");
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

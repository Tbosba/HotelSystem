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
    public class RoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rooms
        public ActionResult Index()
        {
            var rooms = db.Room.Include(r => r.hotels).Include(r => r.roomTypes);
            return View(rooms.ToList());
        }
        public ActionResult RoomBooking()
        {
            var rooms = db.Room.Include(r => r.hotels).Include(r => r.roomTypes);
            return View(rooms.ToList());
        }
        public ActionResult RoomBooking1(int? id)
        {
            var rooms = db.Room.Include(r => r.hotels).Include(r => r.roomTypes);
            return View(rooms.ToList().Where(x=>x.HotelId == id));
        }
        public ActionResult SearchRoom(int? hotelid, int? typeId, int? child, int? adult)
        {
            var rooms = db.Room.Where(m => m.HotelId == hotelid && m.roomtypeId == typeId && m.Children == child && m.Adult == adult && m.roomTypes.RoomAvailable > 0);

            return View(rooms.ToList().Where(x => x.HotelId == hotelid && x.roomtypeId == typeId && x.Adult <= adult && x.Children <= child));
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rooms rooms = db.Room.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            return View(rooms);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            var userName = User.Identity.GetUserName();
            ViewBag.HotelId = new SelectList(db.Hotels.Where(m=>m.ManagerEmail == userName), "HotelId", "HotelName");
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomId,HotelId,roomtypeId,RoomCapacity,Adult,Children,roomDescription,RoomPicture,RoomPrice,Status")] Rooms rooms, HttpPostedFileBase img_upload)
        {

            byte[] photo = null;
            photo = new byte[img_upload.ContentLength];
            img_upload.InputStream.Read(photo, 0, img_upload.ContentLength);
            rooms.RoomPicture = photo;

            if (ModelState.IsValid)
            {
                Hotels hotel = new Hotels();
                hotel = db.Hotels.Where(r => r.HotelId == rooms.HotelId).FirstOrDefault();
                RoomType type = new RoomType();
                type = db.RoomTypes.Where(r => r.RoomtypeId == rooms.roomtypeId).FirstOrDefault();
                hotel.TotalNumberOfRooms -= type.NumberOfRooms;
                if (hotel.TotalNumberOfRooms == 0)
                {
                    TempData["AlertMessage"] = " " + hotel.HotelName + " have reached it maximum room capacity";
                }
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();

                rooms.RoomCapacity = rooms.Adult + rooms.Children;
                db.Room.Add(rooms);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HotelId = new SelectList(db.Hotels, "HotelId", "HotelName", rooms.HotelId);
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type", rooms.roomtypeId);
            return View(rooms);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rooms rooms = db.Room.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            ViewBag.HotelId = new SelectList(db.Hotels, "HotelId", "HotelName", rooms.HotelId);
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type", rooms.roomtypeId);
            return View(rooms);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomId,HotelId,roomtypeId,RoomCapacity,Adult,Children,roomDescription,RoomPicture,RoomPrice,Status")] Rooms rooms)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rooms).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HotelId = new SelectList(db.Hotels, "HotelId", "HotelName", rooms.HotelId);
            ViewBag.roomtypeId = new SelectList(db.RoomTypes, "RoomtypeId", "Type", rooms.roomtypeId);
            return View(rooms);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rooms rooms = db.Room.Find(id);
            if (rooms == null)
            {
                return HttpNotFound();
            }
            return View(rooms);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rooms rooms = db.Room.Find(id);
            db.Room.Remove(rooms);
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

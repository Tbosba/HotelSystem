using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelSystem.Models;
using HotelSystem.Models.Users;

namespace HotelSystem.Controllers.Users
{
    public class AssistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Assists
        public ActionResult Index()
        {
            var assists = db.Assists;
            return View(assists.ToList());
        }

        // GET: Assists/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assist assist = db.Assists.Find(id);
            if (assist == null)
            {
                return HttpNotFound();
            }
            return View(assist);
        }

        // GET: Assists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Assists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AssistanceID,FirstName,LastName,Email,Gender,ContactNo,Address,Password,ConfirmPassword,Id")] Assist assist)
        {
            if (ModelState.IsValid)
            {
                db.Assists.Add(assist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assist);
        }

        // GET: Assists/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assist assist = db.Assists.Find(id);
            if (assist == null)
            {
                return HttpNotFound();
            }
            return View(assist);
        }

        // POST: Assists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssistanceID,FirstName,LastName,Email,Gender,ContactNo,Address,Password,ConfirmPassword,Id")] Assist assist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assist);
        }

        // GET: Assists/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assist assist = db.Assists.Find(id);
            if (assist == null)
            {
                return HttpNotFound();
            }
            return View(assist);
        }

        // POST: Assists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Assist assist = db.Assists.Find(id);
            db.Assists.Remove(assist);
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

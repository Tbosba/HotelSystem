using HotelSystem.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HotelSystem.Controllers
{
    
    public class AdministrationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Administration/
        public ActionResult Index()
        {
            return View(db.Administrations.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Value")] AdministrationModel lab )
        {
            if (ModelState.IsValid)
            {
                db.Administrations.Add(lab);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lab);
        }

        // GET: /Administration/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdministrationModel Administrationmodel = db.Administrations.Find(id);
            if (Administrationmodel == null)
            {
                return View("Error");
            }
            return View(Administrationmodel);
        }

        // POST: /Administration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Value")] AdministrationModel Administrationmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Administrationmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Administrationmodel);
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelSystem.Controllers
{
     // GET: Admin
    public class AdminsController : Controller
    {
        public ActionResult dashboard()
        {
            return View();
        }
    }
}
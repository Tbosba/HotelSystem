using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HotelSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PayFast;
using PayFast.AspNet;
using Twilio.TwiML.Voice;
using HotelSystem.Web.Domain;

namespace HotelSystem.Controllers
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public BookingsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        // GET: RoomBookings
        [Authorize]
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();
            ViewData["RoomBookings"] = db.RoomBookings.Include(r => r.Room).Where(x => x.GuestEmail == userName).ToList();
            return View();
        }

        public ActionResult CustomerIndex()
        {
            return View(db.RoomBookings.ToList());
        }

        public ActionResult CancelBooking(int? id)
        {
            Session["room"] = id;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }
        [HttpPost, ActionName("CancelBooking")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelIndexs(Booking booking)
        {
            var room = Session["room"].ToString();
            booking.RoomId = int.Parse(room);

            var userName = User.Identity.GetUserName();

            booking = db.RoomBookings.Find(int.Parse(room));
            History history = db.History.Find(booking.RoomBookingId);

            history = db.History.Where(m => m.HistoryId == booking.RoomBookingId && m.GuestEmail == userName).FirstOrDefault();
            history.Status = "Cancelled";
            db.Entry(history).State = EntityState.Modified;
            db.SaveChanges();

            db.RoomBookings.Remove(booking);
            db.SaveChanges();
            TempData["AlertMessage"] = "Booking successfully cancelled!";
            return RedirectToAction("Index");
        }
        public ActionResult TermsAndCondition()
        {
            return View();
        }

        public ActionResult HistoryIndex()
        {
            var userName = User.Identity.GetUserName();
            return View(db.History.ToList().Where(x => x.GuestEmail == userName));
        }
        public ActionResult GetMyBill()
        {
            var user = User.Identity.GetUserName();
            var customer = db.Customers.Where(g => g.Email == user).FirstOrDefault();
            var bill = db.Bill.Where(b => b.Email == customer.Email);
            return View(bill);
        }
        public ActionResult ViewBillHistory()
        {
            var user = User.Identity.GetUserName();
            var guest = db.Customers.Where(g => g.Email == user).FirstOrDefault();
            var bill = db.BillHistory.Where(b => b.Email == guest.Email);
            return View(bill);
        }
        public async Task<ActionResult> SendEmailToCheckIn(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error","Home");
            }
            Booking bookings = db.RoomBookings.Find(id);
            var email = User.Identity.Name;
            ApplicationUser user = db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (bookings != null)
            {
                var checkIn = Url.Action("ChckIn", "Bookings", new { RefN = bookings.RefNum }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + checkIn + "\">here</a>");
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Confirm/CheckIn.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{CheckingInLink}", checkIn);
                body = body.Replace("{UserName}", bookings.GuestEmail);
                body = body.Replace("{Name}", bookings.Name);
                body = body.Replace("{Surname}", bookings.Surname);
                body = body.Replace("{Mobile}", bookings.Mobile);
                body = body.Replace("{RefNum}", bookings.RefNum);
                body = body.Replace("{CheckInDate}", bookings.CheckInDate.Date.ToLongDateString());
                body = body.Replace("{CheckOutDate}", bookings.CheckOutDate.Date.ToLongDateString());
                body = body.Replace("{Total}", bookings.Total.ToString("R 0.00"));
                body = body.Replace("{RoomPrice}", bookings.RoomPrice.ToString("R 0.00"));
                body = body.Replace("{RoomType}", bookings.RoomType);
                body = body.Replace("{NumberOfPeople}", bookings.NumberOfPeople.ToString());
                body = body.Replace("{BookingDate}", bookings.BookingDate.Date.ToShortDateString());
                bool IsSendEmail = SendEmail.EmailSend(bookings.GuestEmail, "Confirm your reservation check in", body, true);

                if (IsSendEmail)
                    TempData["AlertMessage"] = bookings.GuestEmail + " check your email to check in.";
                return RedirectToAction("Index");
            }
            TempData["AlertMessage"] = bookings.GuestEmail + " sorry, couldn't check in.";
            return View(bookings);
        }
        public async Task<ActionResult> SendEmailToCheckOut(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            Booking bookings = db.RoomBookings.Find(id);
            var email = User.Identity.Name;
            ApplicationUser user = db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (bookings != null)
            {
                var checkIn = Url.Action("CheckOut", "Bookings", new { RefN = bookings.RefNum }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + checkIn + "\">here</a>");
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Confirm/CheckOut.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{CheckingOutLink}", checkIn);
                body = body.Replace("{UserName}", bookings.GuestEmail);
                body = body.Replace("{Name}", bookings.Name);
                body = body.Replace("{Surname}", bookings.Surname);
                body = body.Replace("{Mobile}", bookings.Mobile);
                body = body.Replace("{RefNum}", bookings.RefNum);
                body = body.Replace("{CheckInDate}", bookings.CheckInDate.Date.ToLongDateString());
                body = body.Replace("{CheckOutDate}", bookings.CheckOutDate.Date.ToLongDateString());
                body = body.Replace("{Total}", bookings.Total.ToString("R 0.00"));
                body = body.Replace("{RoomPrice}", bookings.RoomPrice.ToString("R 0.00"));
                body = body.Replace("{RoomType}", bookings.RoomType);
                body = body.Replace("{NumberOfPeople}", bookings.NumberOfPeople.ToString());
                body = body.Replace("{BookingDate}", bookings.BookingDate.Date.ToShortDateString());
                bool IsSendEmail = SendEmail.EmailSend(bookings.GuestEmail, "Confirm your reservation check in", body, true);

                if (IsSendEmail)
                    TempData["AlertMessage"] = bookings.GuestEmail + " check your email to check out.";
                return RedirectToAction("Index");
            }
            TempData["AlertMessage"] = bookings.GuestEmail + " sorry, couldn't check out.";
            return View(bookings);
        }
        public ActionResult ChckIn(string RefN)
        {
            Booking bookingRoom = db.RoomBookings.Where(p => p.RefNum.Contains(RefN)).FirstOrDefault();
            var customer = db.Customers.ToList().Where(p => p.Email == User.Identity.GetUserName()).Select(p => p.Email).FirstOrDefault();
            if (bookingRoom.RefNum != RefN)
            {
                TempData["AlertMessage"] = "Invalid Reference Number";
                return RedirectToAction("Index");
            }
            else
            if (bookingRoom.Status == "Checked Out")
            {
                TempData["AlertMessage"] = "Cannot check in a person who has already been checked out";
                return RedirectToAction("Index");
            }
            else
            if (bookingRoom.Status == "Checked In") 
            {
                TempData["AlertMessage"] = "Cannot check in " + customer + " twice";
                return RedirectToAction("Index");
            }
            else
            if (bookingRoom.RefNum == RefN)
            {
                bookingRoom.Status = "Checked In";
                db.Entry(bookingRoom).State = EntityState.Modified;
                db.SaveChanges();
                TempData["AlertMessage"] = customer + " Has been Successfully checked in";
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
            //return View();
        }
        public ActionResult CheckOut(string RefN)
        {
            Booking bookingRoom = db.RoomBookings.Where(p => p.RefNum.Contains(RefN)).FirstOrDefault();
            var customer = db.Customers.ToList().Where(p => p.Email == User.Identity.GetUserName()).Select(p => p.Email).FirstOrDefault();
            if (bookingRoom.RefNum != RefN)
            {
                TempData["AlertMessage"] = "Invalid Reference Number";
                return RedirectToAction("Index");
            }
            else
           if (bookingRoom.Status == "Not yet Checked In!!")
            {
                TempData["AlertMessage"] = "Cannot check out a person who has not been checked in";
                return RedirectToAction("ChckIn");
            }
            else if (bookingRoom.Status == "Checked Out")
            {
                TempData["AlertMessage"] = "Cannot check out " + customer + " twice";
                return RedirectToAction("ChckIn");
            }
            else if (bookingRoom.RefNum == RefN)
            {
                bookingRoom.Status = "Checked Out";
                db.Entry(bookingRoom).State = EntityState.Modified;
                db.SaveChanges();

                var q = db.Room.Where(p => p.RoomId == bookingRoom.RoomId).Select(p => p.roomtypeId).FirstOrDefault();
                var rty = db.RoomTypes.Where(p => p.RoomtypeId == q).FirstOrDefault();
                rty.RoomAvailable++;
                db.Entry(rty).State = EntityState.Modified;
                db.SaveChanges();

                TempData["AlertMessage"] = customer + " Successfully checked Out";
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult MyBookings()
        {
            var userName = User.Identity.GetUserName();
            var roomBookings = db.RoomBookings.Include(r => r.Room);
            return View(roomBookings.ToList().Where(x => x.GuestEmail == userName));
        }
        public ActionResult BookingDetails(int id)
        {
            Booking booking = db.RoomBookings.Find(id);
            return View(booking);
        }
        public ActionResult Invoice(int id)
        {
            var booking = db.RoomBookings.Find(id);
            return View(booking);
        }
        // GET: RoomBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }
        public ActionResult ConfirmBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }
        public ActionResult Confirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }
        [Authorize]
        // GET: RoomBookings/Create
        public ActionResult Create(int? id)
        {
            Session["roombookings"] = id;
            ViewBag.Id = id;
            ViewBag.RoomId = new SelectList(db.Room, "RoomId", "roomDescription");

            var user = User.Identity.GetUserName();

            TempData["userID"] = user;

            ViewBag.Later = BusinessLogic.LastDate(id);
            ViewBag.DoubleCheck = db.Room.Include(r => r.roomTypes).Count();

            return View();
        }

        // POST: RoomBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomBookingId,RoomType,RoomId,GuestEmail,Name,Surname,Address,Passport,Mobile,RoomPrice,BasicCost,Total,Tax,DepositAmt,CheckInDate,CheckOutDate,DepositPaid,BookingDate,NumberOfDays,NumberOfPeople")] Booking roomBooking)
        {
            var userName = User.Identity.GetUserName();

            var customer = db.Customers.Where(g => g.Email == userName).FirstOrDefault();

            var room = Session["roombookings"].ToString();
            roomBooking.RoomId = int.Parse(room);

            if (ModelState.IsValid)
            {
                if (BusinessLogic.dateLessOutChecker(roomBooking) == true)
                {
                    TempData["AlertMessage"] = " Booked";
                    return View(roomBooking);
                }
                else
                {
                    roomBooking.GuestEmail = customer.Email;
                    roomBooking.Name = customer.FirstName;
                    roomBooking.Surname = customer.LastName;
                    roomBooking.Mobile = customer.ContactNo;
                    roomBooking.Address = customer.Address;
                    roomBooking.BookingDate = DateTime.Now.Date;
                    roomBooking.ManagerEmail = BusinessLogic.GetHotelManagerEmail(roomBooking.RoomId);
                    roomBooking.RoomPrice = BusinessLogic.GetRoomPrice(roomBooking.RoomId);
                    roomBooking.NumberOfPeople = BusinessLogic.Occupants(roomBooking.RoomId);
                    roomBooking.RoomType = BusinessLogic.GetRoomType(roomBooking.RoomId);
                    roomBooking.NumberOfDays = BusinessLogic.GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate);
                    roomBooking.Status = "Not yet Checked In";
                    roomBooking.BasicCost = BusinessLogic.calcTotalRoomCost(roomBooking);
                    roomBooking.Total = BusinessLogic.calcTotalRoomCost(roomBooking) + BusinessLogic.CalcTax(roomBooking);
                    roomBooking.Tax = BusinessLogic.CalcTax(roomBooking);
                    roomBooking.DepositAmt = BusinessLogic.Deposit(roomBooking);
                    roomBooking.RefNum = BusinessLogic.GenerateRefNumber(roomBooking);
                    roomBooking.HotelAddress = BusinessLogic.GetHotelAddress(roomBooking.RoomId);

                    db.RoomBookings.Add(roomBooking);

                    Rooms rooms = new Rooms();
                    rooms = db.Room.Where(r => r.RoomId == roomBooking.RoomId).FirstOrDefault();
                    rooms.roomTypes.RoomAvailable -= 1;
                    db.Entry(rooms).State = EntityState.Modified;
                    db.SaveChanges();


                    History history = new History();

                    history.HistoryId = roomBooking.RoomBookingId;

                    history.GuestEmail = roomBooking.GuestEmail;
                    history.RefNum = roomBooking.RefNum;
                    history.RoomType = roomBooking.RoomType;
                    history.RoomId = roomBooking.RoomId;
                    history.Name = roomBooking.Name;
                    history.Surname = roomBooking.Surname;
                    history.Address = roomBooking.Address;
                    history.Passport = roomBooking.Passport;
                    history.Mobile = roomBooking.Mobile;
                    history.RoomPrice = roomBooking.RoomPrice;
                    history.BasicCost = roomBooking.BasicCost;
                    history.Total = roomBooking.Total;
                    history.Tax = roomBooking.Tax;
                    history.DepositAmt = roomBooking.DepositAmt;
                    history.DepositPaid = roomBooking.DepositPaid;
                    history.CheckIn = roomBooking.CheckIn;
                    history.CheckInDate = roomBooking.CheckInDate;
                    history.BookingDate = roomBooking.BookingDate;
                    history.CheckOut = roomBooking.CheckOut;
                    history.CheckOutDate = roomBooking.CheckOutDate;
                    history.ManagerEmail = roomBooking.ManagerEmail;
                    history.HotelAddress = roomBooking.HotelAddress;
                    history.Status = roomBooking.Status;
                    history.NumberOfPeople = roomBooking.NumberOfPeople;
                    history.NumberOfDays = roomBooking.NumberOfDays;

                    db.History.Add(history);
                    db.SaveChanges();

                    //Points point = new Points();
                    //point.Email = roomBooking.GuestEmail;
                    //if (roomBooking.GuestEmail != null)
                    //{
                    //    point.Point = Convert.ToInt32(roomBooking.NumberOfDays);
                    //}
                    //if (roomBooking.Room.roomTypes.Type.Contains("Double"))
                    //{
                    //    point.Point = Convert.ToInt32(roomBooking.NumberOfDays) * 3;
                    //}
                    //else if (roomBooking.Room.roomTypes.Type.Contains("Single"))
                    //{
                    //    point.Point = Convert.ToInt32(roomBooking.NumberOfDays) * 2;
                    //}
                    //point.Reference = roomBooking.RefNum;
                    //point.Status = "Pending";
                    //db.Points.Add(point);

                    MyBill bill = new MyBill();
                    bill = db.Bill.Where(x => x.Email == roomBooking.GuestEmail).FirstOrDefault();

                    bill.BookingTotal += roomBooking.Total - roomBooking.DepositAmt;
                    bill.TotalAmount += bill.BookingTotal;
                    bill.Status = "You have " + bill.TotalAmount.ToString("R 0.00") + " amount due";
                    db.Entry(bill).State = EntityState.Modified;
                    db.SaveChanges();

                    Session["bookID"] = roomBooking.RoomBookingId;
                    return RedirectToAction("ConfirmBooking", new { id = roomBooking.RoomBookingId });
                }
            }
            ViewBag.RoomId = new SelectList(db.Room, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }
        //public ActionResult ViewPoints(Points points, string email)
        //{
        //    if (!string.IsNullOrEmpty(email))
        //    {
        //        var find = db.Points.Count(m => m.Email == email && email != null);
        //        if (find > 0)
        //        {
        //            double Sum = db.Points.Where(m => m.Email == email && m.Point > 0).Sum(m => m.Point);

        //            var userName = User.Identity.Name;
        //            var cust = db.Points.Where(x => x.Email == email).FirstOrDefault();

        //            MailMessage mail = new MailMessage();
        //            mail.To.Add(email);
        //            mail.From = new MailAddress("donotreply.shadownet@gmail.com");
        //            mail.Subject = "Prometheus Hotels.";
        //            mail.Body = "Dear: " + points.Email +
        //               "<br/>" + "<br/>" + "You have #" + Sum + " available points." +
        //               "<br/>" + "Status: " + points.Status
        //               + "<br/>" + "<br/>" + "Thank you." + "<br/>" + "<br/>";
        //            mail.IsBodyHtml = true;
        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = "smtp.gmail.com";
        //            smtp.Port = 587;
        //            smtp.UseDefaultCredentials = false;
        //            smtp.Credentials = new System.Net.NetworkCredential("thubehlavo65@gmail.com", "@Thubelihle65");
        //            smtp.EnableSsl = true;
        //            try
        //            {
        //                smtp.Send(mail);
        //            }
        //            catch
        //            {
        //                ViewBag.Error = "";
        //            }
        //        }
        //        TempData["Success123"] = "An email has been sent to your inbox to see your available points.";
        //        return RedirectToAction("ViewPoints");
        //    }
        //    return View();
        //}
        public ActionResult ExtendStay(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            decimal amount;

            Booking myBooking = db.RoomBookings.Find(id);
            //int points = db.Points.Where(c => c.Email == myBooking.GuestEmail).Count();
            //if (points >= 100 && myBooking.Total > 400)
            //{
            //    amount = myBooking.Total - (points * 2);
            //}
            //else
            //{
                amount = myBooking.Total;
            //}
            TempData["CheckInDate"] = myBooking.CheckInDate.Date.ToShortDateString();
            TempData["CheckOutDate"] = myBooking.CheckOutDate.Date.ToShortDateString();
            TempData["GuestEmail"] = myBooking.GuestEmail;
            TempData["Status"] = myBooking.Status;
            TempData["RoomBookingId"] = myBooking.RoomBookingId;
            TempData["amount"] = myBooking.Total;
            TempData["NumberOfDays"] = myBooking.NumberOfDays;
            TempData["Name"] = myBooking.Name;
            TempData["Surname"] = myBooking.Surname;
            TempData["Mobile"] = myBooking.Mobile;
            TempData["RoomType"] = myBooking.RoomType;
            TempData["NumberOfPeople"] = myBooking.NumberOfPeople;
            TempData["RefNum"] = myBooking.RefNum;
            TempData["RoomPrice"] = myBooking.RoomPrice;

            if (myBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomId = new SelectList(db.Room, "RoomId", "roomDescription", myBooking.RoomId);
            return View(myBooking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExtendStay([Bind(Include = "RoomBookingId,RoomType,RoomId,GuestEmail,Name,Surname,Identity,Passport,Mobile,RoomPrice,CheckInDate,CheckOutDate,NumberOfDays,NumberOfPeople")] Booking roomBooking)
        {
            if (ModelState.IsValid)
            {
                roomBooking.NumberOfDays = BusinessLogic.GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate);
                roomBooking.Status = "Extended stay";
                roomBooking.BasicCost = BusinessLogic.calcTotalRoomCost(roomBooking);
                roomBooking.Total = BusinessLogic.calcTotalRoomCost(roomBooking) + BusinessLogic.CalcTax(roomBooking);
                db.Entry(roomBooking).State = EntityState.Modified;
                db.SaveChanges();

                MyBill bill = new MyBill();
                bill = db.Bill.Where(x => x.Email == roomBooking.GuestEmail).FirstOrDefault();

                bill.BookingTotal += roomBooking.Total;
                bill.TotalAmount += bill.BookingTotal;
                bill.Status = "You have R " + bill.TotalAmount.ToString("R 0.00") + " amount due";
                db.Entry(bill).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success123"] = "You have successfull extended your stay editional charges will occur.";
                return RedirectToAction("Index");
            }
            ViewBag.RoomId = new SelectList(db.Room, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }
        // GET: RoomBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomId = new SelectList(db.Room, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }

        // POST: RoomBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomBookingId,RoomType,RoomId,GuestEmail,Name,Surname,Identity,Passport,Mobile,RoomPrice,CheckInDate,CheckOutDate,NumberOfDays,NumberOfPeople")] Booking roomBooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoomId = new SelectList(db.Room, "RoomId", "roomDescription", roomBooking.RoomId);
            return View(roomBooking);
        }

        // GET: RoomBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking roomBooking = db.RoomBookings.Find(id);
            if (roomBooking == null)
            {
                return HttpNotFound();
            }
            return View(roomBooking);
        }

        // POST: RoomBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking roomBooking = db.RoomBookings.Find(id);
            db.RoomBookings.Remove(roomBooking);
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
        public BookingsController()
        {
            this.payFastSettings = new PayFastSettings();
            this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
            this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
            this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
            this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
            this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
            this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
            this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
        }
    //Payment
    #region Fields

    private readonly PayFastSettings payFastSettings;

        #endregion Fields

        #region Constructor

        //public ApprovedOwnersController()
        //{

        //}

        #endregion Constructor

        #region Methods



        public ActionResult Recurring()
        {
            var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            recurringRequest.merchant_id = this.payFastSettings.MerchantId;
            recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
            recurringRequest.return_url = this.payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
            recurringRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            recurringRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Some details about the recurring option";

            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "drnendwandwe@gmail.com";

            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.Now;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult OnceOff(int? id)
        {
            var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);
            //int ReservationID = int.Parse(Session["bookID"].ToString());
            Booking roomBooking = new Booking();
            roomBooking = db.RoomBookings.Find(id);
            var userName = User.Identity.GetUserName();
            var customer = db.Customers.Where(x => x.Email == userName).FirstOrDefault();

            roomBooking.DepositPaid = true;
            db.Entry(roomBooking).State = EntityState.Modified;
            db.SaveChanges();

            var attachments = new List<Attachment>();
            attachments.Add(new Attachment(new MemoryStream(GeneratePDF(roomBooking.RoomBookingId)), "Reservation Receipt", "application/pdf"));


            var mailTo = new List<MailAddress>();
            mailTo.Add(new MailAddress(User.Identity.GetUserName(), customer.LastName));
            var body = $"Hello {customer.FirstName + " " + customer.LastName}, please see attached receipt for the recent reservation you made. <br/>Make sure you bring along your receipt when you check in for your room.<br/>";
            HotelSystem.Models.EmailService emailService = new HotelSystem.Models.EmailService();
            emailService.SendEmail(new EmailContent()
            {
                mailTo = mailTo,
                mailCc = new List<MailAddress>(),
                mailSubject = "Application Statement | Ref No.:" + roomBooking.RoomBookingId,
                mailBody = body,
                mailFooter = "<br/> Many Thanks, <br/> <b>KasiFied</b>",
                mailPriority = MailPriority.High,
                mailAttachments = attachments

            });
            // Merchant Details
            onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
            onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details

            onceOffRequest.email_address = "sbtu01@payfast.co.za";
            //onceOffRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            onceOffRequest.m_payment_id = "";
            onceOffRequest.amount = Convert.ToDouble(roomBooking.DepositAmt);
            onceOffRequest.item_name = "Room Booking payment";
            onceOffRequest.item_description = "Some details about the once off payment";

            // BusinessLogic.UpdateRoomsAvailable(roomBooking.RoomId);
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
            return Redirect(redirectUrl);
        }

        public ActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

            // Merchant Details
            adHocRequest.merchant_id = this.payFastSettings.MerchantId;
            adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
            adHocRequest.return_url = this.payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
            adHocRequest.notify_url = this.payFastSettings.NotifyUrl;

            // Buyer Details
            adHocRequest.email_address = "sbtu01@payfast.co.za";

            // Transaction Details
            adHocRequest.m_payment_id = "";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

            var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

            System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

            // Currently seems that the data validation only works for successful payments
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
            }

            if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Error()
        {
            return View();
        }

        #endregion Methods
        public byte[] GeneratePDF(int BookingID)
        {
            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document(PageSize.A5, 0, 0, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            Booking roomBooking = new Booking();
            roomBooking = db.RoomBookings.Find(BookingID);
            var userName = User.Identity.GetUserName();
            var customer = db.Customers.Where(x => x.Email == userName).FirstOrDefault();

            iTextSharp.text.Font font_heading_3 = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED);
            iTextSharp.text.Font font_body = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.BaseColor.BLUE);

            // Create the heading paragraph with the headig font
            PdfPTable table1 = new PdfPTable(1);
            PdfPTable table2 = new PdfPTable(5);
            PdfPTable table3 = new PdfPTable(1);

            iTextSharp.text.pdf.draw.VerticalPositionMark seperator = new iTextSharp.text.pdf.draw.LineSeparator();
            seperator.Offset = -6f;
            // Remove table cell
            table1.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table3.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            table1.WidthPercentage = 80;
            table1.SetWidths(new float[] { 100 });
            table2.WidthPercentage = 80;
            table3.SetWidths(new float[] { 100 });
            table3.WidthPercentage = 80;

            PdfPCell cell = new PdfPCell(new Phrase(""));
            cell.Colspan = 3;
            table1.AddCell("\n");
            table1.AddCell(cell);
            table1.AddCell("\n\n");
            table1.AddCell(
                "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t" +
                "Paramount Hotel \n" +
                "Email :hotelparamount63@gmail.com" + "\n" +
                "\n" + "\n");
            table1.AddCell("--------------------Your Details-------------------");

            table1.AddCell("First Name : \t" + customer.FirstName);
            table1.AddCell("Last Name : \t" + customer.LastName);
            table1.AddCell("Email : \t" + customer.Email);
            table1.AddCell("Phone Number : \t" + customer.ContactNo);
            table1.AddCell("Address : \t" + customer.Address);

            table1.AddCell("\n------------------Booking details-------------------\n");

            table1.AddCell("Booking # : \t" + roomBooking.RoomBookingId);
            table1.AddCell("Room Type : \t" + roomBooking.RoomType);
            table1.AddCell("Room Price Per Night: \t" + roomBooking.RoomPrice.ToString("R 0.00"));
            table1.AddCell("Arrival date : \t" + roomBooking.CheckInDate);
            table1.AddCell("Departure date : \t" + roomBooking.CheckOutDate);
            table1.AddCell("Number Of days : \t" + roomBooking.NumberOfDays);
            table1.AddCell("Number Of People : \t" + roomBooking.NumberOfPeople);
            table1.AddCell("Total Room Cost: \t" + roomBooking.Total.ToString("R 0.00"));

            table1.AddCell("\n");

            table3.AddCell("---------------Looking forward to hear from you------------------");

            //////Intergrate information into 1 document
            //var qrCode = iTextSharp.text.Image.GetInstance(reservation.QrCodeImage);
            //qrCode.ScaleToFit(200, 200);
            table1.AddCell(cell);
            document.Add(table1);
            //document.Add(qrCode);
            document.Add(table3);
            document.Close();

            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            return bytes;
        }
    }
}

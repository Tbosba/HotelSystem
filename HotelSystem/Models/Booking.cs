using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelSystem.Models;

namespace HotelSystem.Models
{
    public class Booking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomBookingId { get; set; }
        [DisplayName("Room Type")]
        public string RoomType { get; set; }
        public int RoomId { get; set; }
        [DisplayName("Guest Email")]
        public string GuestEmail { get; set; }
        [Display(Name = "Guest Name")]
        public string Name { get; set; }
        [Display(Name = "Guest Surname")]
        public string Surname { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Passport Number")]
        public string Passport { get; set; }
        [Display(Name = "Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        public string Mobile { get; set; }
        [DisplayName("Room Price")/*, DataType(DataType.Currency)*/]
        public decimal RoomPrice { get; set; }
        [DisplayName("Basic Cost")/*, DataType(DataType.Currency)*/]
        public decimal BasicCost { get; set; }
        [DisplayName("Total")/*, DataType(DataType.Currency)*/]
        public decimal Total { get; set; }
        [DisplayName("Tax")/*, DataType(DataType.Currency)*/]
        public decimal Tax { get; set; }
        [DisplayName("Deposit")/*, DataType(DataType.Currency)*/]
        public decimal DepositAmt { get; set; }
        [DisplayName("Paid Deposit")]
        public bool DepositPaid { get; set; }
        [Display(Name = "Ref. Number")]
        public string RefNum { get; set; }
        [DisplayName("Check-In-Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckInDate { get; set; }
        [DisplayName("Check-Out-Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOutDate { get; set; }
        public bool CheckIn { get; set; }

        [DisplayName("Booking Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BookingDate { get; set; }

        public bool CheckOut { get; set; }
        [DisplayName("Number Of Days")]
        public int NumberOfDays { get; set; }
        [DisplayName("Number Of People")]
        public int NumberOfPeople { get; set; }
        public string Status { get; set; }
        public string HotelAddress { get; set; }
        public string ManagerEmail { get; set; }
        public virtual Rooms Room { get; set; }

        public virtual ICollection<Booking> RoomBookings { get; set; }

       // public virtual ICollection<MyBill> Bill { get; set; }
    }
}
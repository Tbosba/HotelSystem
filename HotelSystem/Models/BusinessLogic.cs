using HotelSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelSystem.Models
{
    public class BusinessLogic
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetRoomType(int roomId)
        {
            var roomBuilding = (from rb in db.Room
                                where rb.RoomId == roomId
                                select rb.roomTypes.Type).FirstOrDefault();
            return roomBuilding;
        }
        public static decimal GetRoomPrice(int roomId)
        {
            var roomPrice = (from rb in db.Room
                             where rb.RoomId == roomId
                             select rb.RoomPrice).FirstOrDefault();
            return roomPrice;
        }
        public static int Occupants(int roomId) 
        {
            var adult = (from rb in db.Room
                             where rb.RoomId == roomId
                             select rb.Adult).FirstOrDefault();
            var child = (from rb in db.Room
                         where rb.RoomId == roomId
                         select rb.Children).FirstOrDefault();
            return adult + child;
        }
        public static decimal GetRoomCapacity(int roomId)
        {
            var roomCapacity = (from rb in db.Room
                                where rb.RoomId == roomId
                                select rb.RoomCapacity).FirstOrDefault();
            return roomCapacity;
        }
        public static string GetHotelAddress(int roomId)
        {
            var hotelId = (from rb in db.Room
                           where rb.RoomId == roomId
                           select rb.HotelId).FirstOrDefault();

            var address = (from rb in db.Hotels
                           where rb.HotelId == hotelId
                           select rb.Address).FirstOrDefault();

            return address;
        }
        public static string GenerateRefNumber(Booking booking)
        {
            int count = db.RoomBookings.Count();
            var RefNum = booking.Name.Substring(0, 2).ToUpper() + booking.Surname.Substring(0, 1).ToUpper() + DateTime.Today.Year.ToString() + count.ToString();
            return RefNum;
        }
        public static string GetHotelManagerEmail(int roomId)
        {
            var hotelId = (from rb in db.Room
                           where rb.RoomId == roomId
                           select rb.HotelId).FirstOrDefault();

            var email = (from rb in db.Hotels
                         where rb.HotelId == hotelId
                         select rb.ManagerEmail).FirstOrDefault();

            return email;
        }
        public static Int32 GetNumberDays(DateTime Check_in, DateTime Check_Out)
        {
            return ((Check_Out.Date - Check_in.Date).Days);
        }
        public static decimal CalcTax(Booking roomBooking)
        {
            return (GetRoomPrice(roomBooking.RoomId) * GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate)) * 0.15m;
        }
        public static decimal Deposit(Booking roomBooking)
        {
            return (GetRoomPrice(roomBooking.RoomId) * GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate)) * 0.20m;
        }
        public static decimal calcTotalRoomCost(Booking roomBooking)
        {
            return GetRoomPrice(roomBooking.RoomId) * GetNumberDays(roomBooking.CheckInDate, roomBooking.CheckOutDate);            
        }
        public static bool dateLessOutChecker(Booking roomBooking)
        {
            bool check = false;
            if (roomBooking.CheckInDate >= roomBooking.CheckOutDate)
            {
                check = true;
            }
            return check;
        }
        public static bool dateLessChecker(Booking roomBooking)
        {
            bool check = false;
            if (roomBooking.CheckInDate < DateTime.Now)
            {
                check = true;
            }
            return check;
        }
        public static DateTime LastDate(int? roomId)
        {
            var outDate = (from r in db.RoomBookings
                           where r.RoomId == roomId
                           select r.CheckOutDate
                         ).FirstOrDefault();
            return outDate;
        }
        public static bool roomChecker(Booking roomBooking)
        {
            bool check = false;
            var outDate = (from r in db.RoomBookings
                           where r.RoomId == roomBooking.RoomId
                           select r.CheckOutDate
                         ).FirstOrDefault();
            if (roomBooking.CheckInDate >= outDate)
            {
                check = true;
            }
            return check;
        }
        public static void UpdateRoomsAvailable(int roomId)
        {
            var roomTypeId = (from rb in db.Room
                              where rb.RoomId == roomId
                              select rb.roomtypeId).FirstOrDefault();

            var roomsAvail = (from rb in db.RoomTypes
                              where rb.RoomtypeId == roomTypeId
                              select rb).FirstOrDefault();
            roomsAvail.RoomAvailable -= 1;
            db.Entry(roomsAvail).State = EntityState.Modified;
            db.SaveChanges();

        }
    }
}
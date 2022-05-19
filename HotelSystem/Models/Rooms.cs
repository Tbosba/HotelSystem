using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelSystem.Models
{
    public class Rooms
    {
        [Key]
        public int RoomId { get; set; }
        [DisplayName("Hotels")]
        public int HotelId { get; set; }
        [DisplayName("Room Type")]
        public int roomtypeId { get; set; }
        [DisplayName("Room Capacity")]
        public int RoomCapacity { get; set; }

        [DisplayName("Adult Capacity")]
        public int Adult { get; set; }

        [DisplayName("Children Capacity")]
        public int Children { get; set; }
        [DisplayName("Room Description")]
        public string roomDescription { get; set; }
        public byte[] RoomPicture { get; set; }
        [DisplayName("Room Price")/*, DataType(DataType.Currency)*/]
        public decimal RoomPrice { get; set; }
        [DisplayName("Room Status")]
        public string Status { get; set; }
        public virtual Hotels hotels { get; set; }
        public virtual RoomType roomTypes { get; set; }

       // public virtual List<Booking> Bookings { get; set; }
    }
}
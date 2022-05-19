using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HotelSystem.Models
{
    public class Hotels
    {
        [Key]
        public int HotelId { get; set; }
        [DisplayName("Manager Email")]
        public string ManagerEmail { get; set; }
        [DisplayName("Hotel Name")]
        public string HotelName { get; set; }

        [Display(Name = "Street Number")]
        public string street_number { get; set; }

        [Display(Name = "Street Name")]
        public string route { get; set; }
        [Display(Name = "City")]
        public string locality { get; set; }
        [Display(Name = "Province")]
        public string administrative_area_level_1 { get; set; }
        [Display(Name = "Country")]
        public string country { get; set; }
        [Display(Name = "Postal Code")]
        public string postal_code { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [Display(Name = "Number Of Floors")]
        public int NoOfFloors { get; set; }

        [Display(Name = "Number of rooms")]
        public int TotalNumberOfRooms { get; set; }
        [AllowHtml]
        [Display(Name = "Hotels Description")]
        public string HotelDescription { get; set; }
        [Display(Name = "Picture")]
        public byte[] HotelPic { get; set; }
        public string Address { get; set; }

    }
}
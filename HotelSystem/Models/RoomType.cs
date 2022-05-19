using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelSystem.Models
{
    public class RoomType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomtypeId { get; set; }
        [Required]
        [DisplayName("Room Type")]
        [MinLength(5)]
        [RegularExpression(pattern: @"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Numbers and special characters are not allowed.")]
        public string Type { get; set; }

        [DisplayName("Rooms Available")]
        [Range(0, 300)]
        public Nullable<int> RoomAvailable { get; set; }

        [DisplayName("Number Of Rooms")]
        public int NumberOfRooms { get; set; }
        public ICollection<Rooms> rooms { get; set; }
    }
}
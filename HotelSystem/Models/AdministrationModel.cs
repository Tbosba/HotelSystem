using System;
using System.ComponentModel.DataAnnotations;

namespace HotelSystem.Models
{
    public class AdministrationModel
    {
        [Required]
        public int ID { get; set; }
        [Required]

        public String  Name { get; set; }
        [Required]

        public string  Value { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelSystem.Models
{
    public class ChatClass
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public DateTime Time { get; set; }
        public string Employee { get; set; }
        public string ChatContent { get; set; }
        public string ChatStatus { get; set; }
    }
}
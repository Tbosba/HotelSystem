
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelSystem.Models
{
    public class MyBill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string billID { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Surname")]
        public string Surname { get; set; }

        [DisplayName("Reg. Number")]
        public string RegNo { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        [Display(Name = "Booking Total")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public decimal BookingTotal { get; set; } = 0.00m;

        [Display(Name = "Room Service Total")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public decimal RoomServiceTotal { get; set; } = 0.00m;

        [Display(Name = "Discount")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public decimal Discount { get; set; } = 0.00m;

        [Display(Name = "Tax Amount")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public decimal Tax { get; set; } = 0.00m;

        [Display(Name = "Total Amount")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public decimal TotalAmount { get; set; } = 0.00m;

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Spa Total")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public decimal SpaTotal { get; set; } = 0.00m;

        [Display(Name = "Laundry Total")]
        [DisplayFormat(DataFormatString = "{0: 0.00}")]
        public decimal LaundryTotal { get; set; } = 0.00m;
    }
}
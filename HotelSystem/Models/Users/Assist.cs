using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelSystem.Models.Users
{
    public class Assist
    {
        [Key]
        [Display(Name = "Assistance Number")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string AssistanceID { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Surname")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        //[Required(ErrorMessage = "Enter Gender")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Enter Phone Nubmer")]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Enter Valid Home Address")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
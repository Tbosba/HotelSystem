using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelSystem.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HotelSystem.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int ContactNo { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool ConfirmedEmail { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<AdministrationModel> Administrations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Assist> Assists { get; set; }
        public DbSet<ChatClass> ChatClass { get; set; }
        public DbSet<Hotels> Hotels { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Rooms> Room { get; set; }
        public DbSet<Booking> RoomBookings { get; set; }
        public DbSet<MyBill> Bill { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<BillHistory> BillHistory { get; set; }
    }
  
}
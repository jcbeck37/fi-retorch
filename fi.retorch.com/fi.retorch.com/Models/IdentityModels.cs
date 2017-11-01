using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using fi.retorch.com.Data;

namespace fi.retorch.com.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static ApplicationUser ConvertDBUser(User user)
        {
            ApplicationUser userObject = new ApplicationUser();

            if (user != null)
            {
                userObject.Id = user.Id.ToString();
                userObject.Email = user.Email;
                userObject.UserName = user.Email;
                userObject.PasswordHash = user.PasswordHash;
            }

            return userObject;
        }
    }

    public partial class ApplicationRole : IdentityRole
    {

    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {

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

        public DbSet<CategoryGroup> CategoryGroups { get; set; }

        public DbSet<Areas.Dashboard.Models.CategoryModel> CategoryModels { get; set; }

        public DbSet<Areas.Dashboard.Models.AccountTypeModel> AccountTypeModels { get; set; }

        public DbSet<Areas.Dashboard.Models.AccountModel> AccountModels { get; set; }

        public DbSet<Areas.Dashboard.Models.TransactionModel> TransactionModels { get; set; }
        
        public DbSet<Areas.Dashboard.Models.ReminderModel> ReminderModels { get; set; }
    }
}
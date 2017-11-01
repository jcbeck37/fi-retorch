using fi.retorch.com.Areas.Configuration.Code.Base;
using fi.retorch.com.Areas.Configuration.Models;
using fi.retorch.com.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Linq;
using fi.retorch.com.Code.Data.Roles;

namespace fi.retorch.com.Areas.Configuration.Controllers
{
    public class UsersController : BaseController
    {
        // GET: Configuration/Users
        public ActionResult Index()
        {
            UserListModel model = new UserListModel(authDb, db);

            return View(model);
        }

        public ActionResult Initialize()
        {
            var roleManager = new RoleManager<ApplicationRole>(new ApplicationRoleStore(authDb));
            var userManager = new UserManager<ApplicationUser>(new com.Code.Data.Users.UserStore());

            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin rool   
                var role = new ApplicationRole();
                role.Name = "Admin";
                roleManager.Create(role);

            }

            var dbUser = from u in authDb.Users
                            where u.Email == "jason@retorch.com"
                            select u;
            var user = ApplicationUser.ConvertDBUser(dbUser.First());

            if (!userManager.IsInRole(user.Id, "Admin")) {
                //Add default User to Role Admin
                var result1 = userManager.AddToRole(user.Id, "Admin");
            }

            return View();
        }
    }
}
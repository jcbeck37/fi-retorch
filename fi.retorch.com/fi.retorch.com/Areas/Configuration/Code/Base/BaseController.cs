using fi.retorch.com.Data;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Configuration.Code.Base
{
    [Authorize(Roles = "Admin")]
    [RequireHttps]
    public class BaseController : Controller
    {
        internal string userKey { get { return User.Identity.GetUserId(); } }
        internal AuthEntities authDb = new AuthEntities();
        internal Entities db = new Entities();
    }
}
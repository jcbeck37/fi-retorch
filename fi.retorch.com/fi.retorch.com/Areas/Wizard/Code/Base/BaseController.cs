using fi.retorch.com.Data;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Wizard.Code.Base
{
    [Authorize]
    [RequireHttps]
    public class BaseController : Controller
    {
        internal string userKey { get { return User.Identity.GetUserId(); } }
        internal Entities db = new Entities();
        internal DefaultEntities defaultDb = new DefaultEntities();

        //internal int PageSize = 25;
    }
}
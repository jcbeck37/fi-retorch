using fi.retorch.com.Data;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Legacy.Code.Base
{
    [Authorize]
    [RequireHttps]
    public class BaseController : Controller
    {
        internal string userKey { get { return User.Identity.GetUserId(); } }
        internal LegacyEntities legacyDb = new LegacyEntities();
        internal Entities db = new Entities();
    }
}
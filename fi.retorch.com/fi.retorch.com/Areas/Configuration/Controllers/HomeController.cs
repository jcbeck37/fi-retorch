using fi.retorch.com.Areas.Configuration.Code.Base;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Configuration.Controllers
{
    [Authorize] // need to fix ROLE checking and then delete this line
    [RequireHttps]
    public class HomeController : BaseController
    {
        // GET: Dashboard/Home
        public ActionResult Index()
        {
            return View();
        }

        #region AJAX functions
        #endregion
    }
}
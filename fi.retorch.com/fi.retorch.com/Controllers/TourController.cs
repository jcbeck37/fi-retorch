using System.Web.Mvc;

namespace fi.retorch.com.Controllers
{
    [RequireHttps]
    public class TourController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
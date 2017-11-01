using fi.retorch.com.Areas.Reports.Code.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Reports.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Reports/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}
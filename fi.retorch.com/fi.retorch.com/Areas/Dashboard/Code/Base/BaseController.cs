using fi.retorch.com.Data;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Dashboard.Code.Base
{
    [Authorize]
    [RequireHttps]
    public class BaseController : Controller
    {
        internal string userKey { get { return User.Identity.GetUserId(); } }
        internal Entities db = new Entities();

        internal int PageSize = 25;
    }
}
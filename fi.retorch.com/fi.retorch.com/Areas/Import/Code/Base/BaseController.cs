using fi.retorch.com.Data;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Import.Code.Base
{
    [Authorize]
    [RequireHttps]
    public class BaseController : Controller
    {
        internal string userKey { get { return User.Identity.GetUserId(); } }
        internal Entities db = new Entities();

        internal string FilePath
        {
            get { return ConfigurationManager.AppSettings["FileUploadPath"]; }
        }

        internal string FileUploadPath(string subfolder)
        {
            return Server.MapPath(Path.Combine(FilePath, subfolder));
        }

    }
}
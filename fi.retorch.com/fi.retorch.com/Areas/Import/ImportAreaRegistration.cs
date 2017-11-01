using System.Web.Mvc;

namespace fi.retorch.com.Areas.Import
{
    public class ImportAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Import";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Import_default",
                "Import/{controller}/{action}/{id}",
                new { area = "Import", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "fi.retorch.com.Areas.Import.Controllers" }
            );
        }
    }
}
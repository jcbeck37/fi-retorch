using System.Web.Mvc;

namespace fi.retorch.com.Areas.Legacy
{
    public class LegacyAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Legacy";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Legacy_default",
                "Legacy/{controller}/{action}/{id}",
                new { area = "Legacy", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "fi.retorch.com.Areas.Legacy.Controllers" }
            );
        }
    }
}
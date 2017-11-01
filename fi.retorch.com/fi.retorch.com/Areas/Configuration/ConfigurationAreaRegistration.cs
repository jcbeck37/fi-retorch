using System.Web.Mvc;

namespace fi.retorch.com.Areas.Configuration
{
    public class ConfigurationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Configuration";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Configuration_default",
                "Configuration/{controller}/{action}/{id}",
                new { area = "Configuration", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "fi.retorch.com.Areas.Configuration.Controllers" }
            );
        }
    }
}
using System.Web.Mvc;

namespace fi.retorch.com.Areas.Reports
{
    public class ReportsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Reports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Reports_default",
                "Reports/{controller}/{action}/{id}",
                new { area = "Reports", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "fi.retorch.com.Areas.Reports.Controllers" }
            );
        }
    }
}
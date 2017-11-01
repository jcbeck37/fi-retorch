using System.Web.Mvc;

namespace fi.retorch.com.Areas.Wizard
{
    public class WizardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Wizard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Wizard_default",
                "Wizard/{action}/{id}",
                new { area = "Wizard", controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "fi.retorch.com.Areas.Wizard.Controllers" }
            );
        }
    }
}
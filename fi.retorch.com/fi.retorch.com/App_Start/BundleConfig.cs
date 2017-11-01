using System.Web.Optimization;

namespace fi.retorch.com
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery/jquery.validate*",
                        "~/Scripts/jquery/jquery.validate.unobtrusive*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap/bootstrap.js",
                      "~/Scripts/bootstrap/bootstrap-datepicker.js",
                      "~/Scripts/respond/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css-bundle").Include(
                      "~/Content/css/bootstrap/bootstrap.css",
                      "~/Content/css/bootstrap/bootstrap-datepicker.css",
                      "~/Content/css/site.css",
                      "~/Content/css/fi.css"));
        }
    }
}

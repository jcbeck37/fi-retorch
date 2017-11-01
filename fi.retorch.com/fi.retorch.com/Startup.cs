using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(fi.retorch.com.Startup))]
namespace fi.retorch.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

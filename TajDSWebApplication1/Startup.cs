using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TajDSWebApplication1.Startup))]
namespace TajDSWebApplication1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

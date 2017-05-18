using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LS.WebApp.Startup))]

namespace LS.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

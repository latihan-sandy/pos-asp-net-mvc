using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(pos_asp_net_mvc.Startup))]
namespace pos_asp_net_mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

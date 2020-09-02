using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IdentityWithRedis.Startup))]
namespace IdentityWithRedis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}

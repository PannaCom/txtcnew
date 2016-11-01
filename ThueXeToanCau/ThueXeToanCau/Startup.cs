using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ThueXeToanCau.Startup))]
namespace ThueXeToanCau
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

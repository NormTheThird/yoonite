using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Yoonite.UI.Startup))]
namespace Yoonite.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

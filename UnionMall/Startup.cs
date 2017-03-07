using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UnionMall.Startup))]
namespace UnionMall
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

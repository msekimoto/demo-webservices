using System.IdentityModel.Tokens;
using Microsoft.Owin;
using Owin;
using Serilog;
using System.Configuration;

[assembly: OwinStartup(typeof(Investran.Api.Startup))]

namespace Investran.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();            
        }
    }
}

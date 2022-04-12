using SimpleInjector;
using SimpleInjector.Lifestyles;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;
using Investran.Api.Auth;
using System;
using System.Web;
using Investran.Integration;
using Investran.Api.Helper;

namespace Investran.Api
{
    public partial class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            // Simple Injector
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Singleton                              
            container.Register<Authentication>(Lifestyle.Singleton);
            container.Register<UdfHelper>(Lifestyle.Singleton);

            // Services
            container.Register<UdfService>(Lifestyle.Scoped);
            container.Register<SdsService>(Lifestyle.Scoped);
            container.Register<BatchService>(Lifestyle.Scoped);
            container.Register<LookupService>(Lifestyle.Scoped);
            container.Register<SecurityService>(Lifestyle.Scoped);
            container.Register<EntityService>(Lifestyle.Scoped);
            container.Register<AllocationService>(Lifestyle.Scoped);
            container.Register<GeneralLedgerService>(Lifestyle.Scoped);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            if (exception != null)
                Logs.Error(exception);

            Server.ClearError();
        }
    }
}

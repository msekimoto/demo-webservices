using Castle.Facilities.Logging;
using Sungard.Investran.Suite.Api;
using Sungard.Investran.Suite.Api.Installers;
using Sungard.Investran.Suite.Security.Principal;
using System;
using System.Configuration;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;

namespace Investran.Api.Auth
{
    public class Authentication
    {
        private const string AssemblyName = "Sungard.Investran.Suite.DataImport.WebServices.Contracts";

        private ApplicationScope ApplicationScope { get; set; }
        private InvestranSuitePrincipal Principal { get; set; }
        
        public string WebServicesUri { get; }
        public string ServicePrincipalName { get; }
        public string EndPointIdentity { get; }
        public string Server { get; }
        public string Database { get; }
        public string Username { get; set; }
        public string Password { get; set; }

        public WindowsIdentity Identity { get; set; }

        public Authentication()
        {
            WebServicesUri = ConfigurationManager.AppSettings["WebServicesUri"];
            ServicePrincipalName = ConfigurationManager.AppSettings["ServicePrincipalName"];
            EndPointIdentity = ConfigurationManager.AppSettings["EndPointIdentity"];
            Server = ConfigurationManager.AppSettings["Server"];
            Database = ConfigurationManager.AppSettings["Database"];
            Username = ConfigurationManager.AppSettings["Username"];
            Password = ConfigurationManager.AppSettings["Password"];

            Identity = WindowsIdentity.GetCurrent();
            ApplicationScope = GetApplicationScope();         
            
            SetUserCredentials();
        }

        private ApplicationScope GetApplicationScope()
        {
            if (InvestranApplication.Current != null)
                return InvestranApplication.Current.CreateApplicationScope(Server, Database, new ApplicationScopeOptions());

            InvestranApplicationOptions options = new InvestranApplicationOptions();
            options.LoggingOptions.LogUsing(LoggerImplementation.Console);
            options.Install(new ClientWebServicesInstaller(options.WebServicesOptions, Assembly.Load(AssemblyName), "DataImport"));
            options.WebServicesOptions.UsingWebServicesAt(WebServicesUri);
            options.WebServicesOptions.UsingEndpointIdentity(new DnsEndpointIdentity(EndPointIdentity));
            options.WebServicesOptions.UsingAuthenticationMethods(o =>
            {
                o.Add(AuthenticationMethod.UserNamePassword, null, new DnsEndpointIdentity(EndPointIdentity), null);
                o.Add(AuthenticationMethod.Windows, null, new SpnEndpointIdentity(ServicePrincipalName), null);
            });

            return InvestranApplication.Create(Assembly.GetExecutingAssembly().GetName().Name, InvestranApplicationType.WindowsApplication, options)
                                       .CreateApplicationScope(Server, Database, new ApplicationScopeOptions());
        }

        public void SetUserCredentials()
        {
            Principal = ApplicationScope.ValidateUser(Username, Password);
            Thread.CurrentPrincipal = Principal;
        }

    }
}

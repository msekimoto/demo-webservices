using Investran.Api.Attributes;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Investran.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Filters.Add(new ExceptionHandlerFilterAttribute());
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {
                    action = RouteParameter.Optional,
                    id = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { controller = "Browser", action = "Index", id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configure((httpConfig) =>
            {
                httpConfig.Filters.Add(new AuthorizeAttribute());
            });

        }
    }
}

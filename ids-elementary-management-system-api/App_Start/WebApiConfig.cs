using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ids_elementary_management_system_api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors(new EnableCorsAttribute("http://localhost:4200", headers: "*", methods: "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
            //    = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling
            //     = Newtonsoft.Json.PreserveReferencesHandling.Objects;


            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));


            //config.Routes.MapHttpRoute(
            //    name: "ActionApi",
            //    routeTemplate: "api/{controller}/{action}/{id}/{asd}",
            //    defaults: new { id = RouteParameter.Optional}
            //);
        }
    }
}

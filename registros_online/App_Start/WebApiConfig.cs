using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace webApiTest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //configurar cors
            config.EnableCors();
            // Configuración y servicios de Web API
            // Configure Web API para usar solo la autenticación de token de portador.

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

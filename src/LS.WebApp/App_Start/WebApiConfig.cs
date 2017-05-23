using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using LS.WebApp.Formatters;
using LS.WebApp.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Exceptionless;
namespace LS.WebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			// Web API configuration and services
			// Configure Web API to use only bearer token authentication.
			//            config.SuppressDefaultHostAuthentication();
			//            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
			Exceptionless.ExceptionlessClient.Default.RegisterWebApi(GlobalConfiguration.Configuration);

			// Web API routes
			config.MapHttpAttributeRoutes();


            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
			config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
			config.Formatters.Add(new ImageMediaTypeFormatter());
		}
    }
}

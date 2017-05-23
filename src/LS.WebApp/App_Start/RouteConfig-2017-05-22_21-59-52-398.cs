using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LS.WebApp
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute(
				name: "AngularSPA",
				url: "{*id}",
				defaults: new { controller = "AngularAccess", action = "Index", id = UrlParameter.Optional }
			);
			//routes.MapRoute(
			//                         name: "Default",
			//                         url: "{*anything}",
			//                         defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			//                     );
		}
	}
}
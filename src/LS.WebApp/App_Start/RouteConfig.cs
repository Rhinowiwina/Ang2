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
				name: "Login",
				url: "login",
				defaults: new { controller = "Authentication", action = "Login", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "ForgotPassword",
				url: "forgotpassword",
				defaults: new { controller = "Authentication", action = "ForgotPassword", id = UrlParameter.Optional });

			routes.MapRoute(
			   name: "ForgotPasswordConfirmation",
			   url: "forgotpasswordconfirmation",
			   defaults: new { controller = "Authentication", action = "ForgotPasswordConfirmation", id = UrlParameter.Optional });

			routes.MapRoute(
				name: "ResetPassword",
				url: "resetpassword",
				defaults: new { controller = "Authentication", action = "ResetPassword", id = UrlParameter.Optional });

			routes.MapRoute(
			   name: "ResetPasswordConfirmation",
			   url: "resetpasswordconfirmation",
			   defaults: new { controller = "Authentication", action = "ResetPasswordConfirmation", id = UrlParameter.Optional });

			routes.MapRoute(
				name: "Logout",
				url: "logout/{*id}",
				defaults: new { controller = "Authentication", action = "Logout", id = UrlParameter.Optional }
				);

			routes.MapRoute(
				name: "ForceLogout",
				url: "forceLogout",
				defaults: new { controller = "Authentication", action = "ForceLogout" }
				);

			routes.MapRoute(
				 name: "ApiLogArchive",
				 url: "SchedTasks/ApiLogArchive",
				 defaults: new { controller = "SchedTasks", action = "ApiLogArchive" }
				 );
			routes.MapRoute(
				name: "CommissionPayment",
				url: "SchedTasks/CommissionPayment",
				defaults: new { controller = "SchedTasks", action = "CommissionPayment" } //This action/function does not actually exist anymore.  We want to return an error if this route is hit.
				);

			routes.MapRoute(
				  name: "ApiLogEhdbCheckToEhdbResponseLog",
				  url: "SchedTasks/ApiLogEhdbCheckToEhdbResponseLog",
				  defaults: new { controller = "SchedTasks", action = "ApiLogEhdbCheckToEhdbResponseLog" }
				  );

			routes.MapRoute(
				 name: "SetUserInactiveDrugExpiration",
				 url: "SchedTasks/SetUserInactiveDrugExpiration",
				 defaults: new { controller = "SchedTasks", action = "SetUserInactiveDrugExpiration" }
				 );

			routes.MapRoute(
				name: "MoveDataFiles",
				url: "SchedTasks/MoveDataFiles",
				defaults: new { controller = "SchedTasks", action = "MoveDataFiles" }
				);
			routes.MapRoute(
			 name: "ClearOrders",
			 url: "SchedTasks/ClearOrders",
			 defaults: new { controller = "SchedTasks", action = "ClearOrders" }
			 );
			routes.MapRoute(
				name: "ImportDataFiles",
				url: "SchedTasks/ImportDataFiles",
				defaults: new { controller = "SchedTasks", action = "ImportDataFiles" }
				);
			routes.MapRoute(
			   name: "DetailReports",
			   url: "SchedTasks/DetailReport",
			   defaults: new { controller = "SchedTasks", action = "DetailReport" }
			   );
			routes.MapRoute(
				name: "ProcessData",
				url: "SchedTasks/ProcessData",
				defaults: new { controller = "SchedTasks", action = "ProcessData" }
				);

			routes.MapRoute(
				name: "ImportDataFilesCheck",
				url: "SchedTasks/ImportDataFilesCheck",
				defaults: new { controller = "SchedTasks", action = "ImportDataFilesCheck" }
				);

			routes.MapRoute(
				name: "ProcessDataCheck",
				url: "SchedTasks/ProcessDataCheck",
				defaults: new { controller = "SchedTasks", action = "ProcessDataCheck" }
				);

			routes.MapRoute(
				name: "SendFailureEmail",
				url: "SchedTasks/SendFailureEmail",
				defaults: new { controller = "SchedTasks", action = "SendFailureEmail" }
				);

			routes.MapRoute(
				name: "TempFileCleanup",
				url: "SchedTasks/TempFileCleanup",
				defaults: new { controller = "SchedTasks", action = "TempFileCleanup" }
				);

			routes.MapRoute(
				name: "UpdateDatabase",
				url: "updatedatabase",
				defaults: new { controller = "UpdateDatabase", action = "UpdateDatabase" }
				);

			routes.MapRoute(
			   name: "ForceSessionLogout",
			   url: "forceSessionLogout",
			   defaults: new { controller = "Authentication", action = "ForceSessionLogout" }
			   );

			routes.MapRoute(
				name: "UserSignup",
				url: "UserSignup/",
				defaults: new { controller = "UserSignup", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "AngularSPA",
				url: "{*id}",
				defaults: new { controller = "AngularAccess", action = "Index", id = UrlParameter.Optional }
			);

			routes.IgnoreRoute("Content/{*pathInfo}");

		}
	}
}
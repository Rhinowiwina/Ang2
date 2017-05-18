using System;
using System.Diagnostics;
using Exceptionless.Plugins;
using Exceptionless.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using LS.Domain;
using System.Web;

namespace LS.WebApp.Utilities {
    public class ExceptionlessLoggedInUser : IEventPlugin {
        public void Run(EventPluginContext context) {
            try {
                ApplicationUser user = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                context.Event.Data.Add("Logged In User", user);
            } catch {
                context.Event.Data.Add("Logged In User", "Failed to get users session.");
            }
        }
    }
}
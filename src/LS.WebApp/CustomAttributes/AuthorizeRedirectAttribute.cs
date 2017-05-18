using System;
using System.Web.Mvc;

namespace LS.WebApp.CustomAttributes {
    public class AuthorizeRedirectAttribute : AuthorizeAttribute {
        private readonly string _redirectUrl = "";

        public AuthorizeRedirectAttribute(string redirectUrl)
            : base() {
            this._redirectUrl = redirectUrl;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
            if (!filterContext.HttpContext.Request.IsAuthenticated) {
                string authUrl = this._redirectUrl; //passed from attribute

                if (!String.IsNullOrEmpty(authUrl)) {
                    if (!filterContext.HttpContext.Response.HeadersWritten) {
                        //filterContext.HttpContext.Response.Redirect(authUrl);

                        //This remarked like could possibly fix the issues in the events view of the azure portal
                        //Server cannot set status after HTTP headers have been sent(http://salmontech.blogspot.com/2011/07/http-header-errors-with-mvc3-redirects.html)
                        filterContext.Result = new RedirectResult(authUrl);
                    }
                }
            }

            //else do normal process
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
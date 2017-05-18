using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using LS.Services;
using Microsoft.AspNet.Identity;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;

namespace LS.WebApp.CustomAttributes {
    public class SingleSessionAuthorizeAttribute : AuthorizeAttribute {
        private static readonly string SessionIdKey = "SessionId";

        public override void OnAuthorization(HttpActionContext actionContext) {
            var httpContext = HttpContext.Current;
            if (!httpContext.Request.IsAuthenticated) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Lost_Session");
            }

            if (!IsAuthorized(actionContext) && httpContext.Request.IsAuthenticated) {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        protected override bool IsAuthorized(HttpActionContext actionContext) {
            if (!base.IsAuthorized(actionContext)) {
                return false;
            }

            var userId = actionContext.RequestContext.Principal.Identity.GetUserId();
            if (userId == null) {
                return false;
            }

            var httpContext = HttpContext.Current;
            if (httpContext.Session == null) {
                return false;
            }
            if (httpContext.Session[SessionIdKey] == null) {
                httpContext.Session[SessionIdKey] = httpContext.Session.SessionID;
            }

            var sessionId = httpContext.Session[SessionIdKey].ToString();

            var loginService = new LoginInfoDataService();
            var checkIfLoginIsValid = loginService.CurrentLoginIsValid(userId, sessionId);
            if (checkIfLoginIsValid.IsFatalFailure()) {
                return false;
            }
            var loginIsValid = checkIfLoginIsValid.Data;

            return loginIsValid;
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using System.Collections.Specialized;
using LS.Domain;
using System.Net;
using System.Text;
using LS.Services;
using LS.WebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LS.Utilities;
using System.IO;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.MVC {
    public class AuthenticationController : Controller {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private IAuthenticationManager _authenticationManager;

        private const string _loginPath = "/login";
        private static readonly string SessionIdKey = "SessionId";

        public IAuthenticationManager AuthenticationManger {
            get {
                return _authenticationManager ?? (_authenticationManager = HttpContext.GetOwinContext().Authentication);
                }
            set { _authenticationManager = value; }
            }

        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
            private set {
                _userManager = value;
                }
            }
        public ApplicationSignInManager SignInManager {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
            private set {
                _signInManager = value;
                }
            }

        public ActionResult Login() {
            return View();
            }

        [HttpPost]
        //Todo: Verify errors are acceptable to display to user
        //Todo: Validate or improve error messages
        public async Task<ActionResult> Login(LoginViewModel model) {
            //var request = (HttpWebRequest)WebRequest.Create("https://wsonline.seisint.com/WsIdentity?ver_=1.83");

            //var postData = "<Envelope> <Header> <Security> <UsernameToken> <Username>BUDXML</Username> <Password>Bu17Dt42</Password> </UsernameToken> </Security> </Header> <Body> <FlexIDRequest> <User> <ReferenceCode>Budget Mobile</ReferenceCode> <BillingCode>Signup</BillingCode> <GLBPurpose>5</GLBPurpose> <DLPurpose>3</DLPurpose> </User> <Options> <IncludeAllRiskIndicators>1</IncludeAllRiskIndicators> <DOBMatch> <MatchType>FuzzyCCYYMMDD</MatchType> <MatchYearRadius>3</MatchYearRadius> </DOBMatch> </Options> <SearchBy> <Name> <First>PHAN</First> <Last>LY</Last> </Name> <Address> <StreetAddress1>1347 E Idahome St</StreetAddress1> <City>West Covina</City> <State>CA</State> <Zip5>91790</Zip5> </Address> <DOB> <Year>1955</Year> <Month>10</Month> <Day>20</Day> </DOB> <SSNLast4>6717</SSNLast4> <HomePhone>7140000000</HomePhone> </SearchBy> </FlexIDRequest> </Body> </Envelope>";

            //var data = Encoding.ASCII.GetBytes(postData);

            //request.Method = "POST";
            //request.Proxy = null;
            //request.ContentType = "text/xml";
            //request.ContentLength = data.Length;

            //request.Headers.Add("SOAPAction", "WsIdentity/FlexID?ver_=1.83");

            //using (var stream = request.GetRequestStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}

            //var response = (HttpWebResponse)request.GetResponse();

            //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //ModelState.AddModelError("", responseString);
            //return View(model);

            if (!ModelState.IsValid) {
                //return View(model);
                }

            var appUserService = new ApplicationUserDataService();
            var userResult = await appUserService.GetByUserNameAsync(model.UserName);
            if (!userResult.IsSuccessful) {
                ModelState.AddModelError("","An error occurred with the login username/password");
                return View(model);
                }
            var user = userResult.Data;
            if (!user.IsActive) {
                ModelState.AddModelError("","Your account is inactive. Contact your supervisor to activate your accoount.");
                return View(model);

                }

            Session[SessionIdKey] = Session.SessionID;
            var sessionId = Session[SessionIdKey].ToString();


            var loginInfoService = new LoginInfoDataService();
            var checkIfLoggedInElsewhereResult = loginInfoService.IsUserLoggedInElsewhere(user.Id,sessionId);
            if (!checkIfLoggedInElsewhereResult.IsSuccessful) {
                ModelState.AddModelError("LoggedInElsewhere",
                    "An error occurred while verifying that the user is not logged in elsewhere.");
                return View(ModelState);
                }
            var userIsLoggedInElsewhere = checkIfLoggedInElsewhereResult.Data;
            if (userIsLoggedInElsewhere) {
                var logoutUserElsewhereResult = loginInfoService.LogOutUserElsewhere(user.Id);
                if (!logoutUserElsewhereResult.IsSuccessful) {
                    ModelState.AddModelError("","An error occurred logged out of your previous session");
                    return View(model);
                    }
                }

            //var currentSessionExists = loginInfoService.GetFrom(user.Id, sessionId);
            //if (!currentSessionExists.IsSuccessful)
            //{
            //    ModelState.AddModelError("", "An error occurred retrieving a previous session");
            //    return View(model);
            //}
            //var existingSession = currentSessionExists.Data;
            //if (existingSession == null)
            //{
            var loginInfo = new LoginInfo {
                UserId = user.Id,
                UserIsLoggedIn = true,
                SessionId = sessionId,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                IsDeleted = false,
                };

            var addLoginInfoResult = loginInfoService.Add(loginInfo);
            if (!addLoginInfoResult.IsSuccessful) {
                ModelState.AddModelError("","An error occurred creating login info");
                return View(model);
                }
            //}
            //else
            //{
            //    existingSession.UserIsLoggedIn = true;
            //    var updateLoginInfoResult = loginInfoService.Update(existingSession);
            //}

            var result = await SignInManager.PasswordSignInAsync(model.UserName,model.Password,model.RememberMe,shouldLockout: false);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToAction("Index","AngularAccess");
                //                case SignInStatus.LockedOut:
                //                    return View("Lockout");
                //                case SignInStatus.RequiresVerification:
                //                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("","Invalid login attempt.");
                    return View(model);
                }
            }

        public ActionResult Logout(string UserID) {
            var loginInfoService = new LoginInfoDataService();
            loginInfoService.LogOutUserElsewhere(UserID);
            AuthenticationManger.SignOut();
            return Redirect(_loginPath);
            }

        public ActionResult ForceLogout() {
            AuthenticationManger.SignOut();
            return View("ForcedLogoutLogin");
            }
        public ActionResult ForceSessionLogout() {
            AuthenticationManger.SignOut();
            return View("ForceSessionLogoutLogin");
            }

        public ActionResult ForgotPassword() {
            return View();
            }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null) {
                    ModelState.AddModelError("","The user either does not exist or is not confirmed.");
                    return View();
                    }
                if (user.Role == null) {
                    var appUserService = new ApplicationUserDataService();
                    var getUserResult = await appUserService.GetAsync(user.Id);
                    if (!getUserResult.IsSuccessful) {
                        ModelState.AddModelError("",
                            "An error occurred while trying to verify you have permissions to perform this action. Please try again.");
                        return View();
                        }

                    user.Role = getUserResult.Data.Role;
                    }
                //Let all users reset password per Budget/Kevin
                //if (!user.Role.IsAdmin() && !user.Role.IsSuperAdmin())
                //{
                //    ModelState.AddModelError("",
                //        "Your account cannot perform this action. If you need your password reset, please contact your Manager or and Administrator.");
                //    return View();
                //}

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //No need to encode.  That is handled by the UrlHelper 
                //code = HttpUtility.UrlEncode(code);
                var baseUrl = Request.Url.Authority;             //RequestUri.Authority;
                var helper = new System.Web.Mvc.UrlHelper(HttpContext.Request.RequestContext);
                var callbackPath = helper.Action("ResetPassword","Authentication",new { c = user.Id,code = code });
                var callbackUrl = "https://" + baseUrl + callbackPath;
                // = Url.Action("ResetPassword", "Authentication", new { c = user.Id, code = code }, protocol: Request.Url.Scheme);
                var emailHelper = new EmailHelper();
              
               
                var emailResult = emailHelper.SendEmail("Reset Password",user.Email,"","A password reset request has been made for your account on https://arrow.spinlifeserv.com.  If you do not recognize this request, you can safely ignore it.<br/><br/> <a href=\"" + callbackUrl + "\">Reset Password Now</a><br><br><span style='font-size:9px'><em>Link not working? Paste the following link into your browser:</em><br>" + callbackUrl + "</span>",null);
                if (!emailResult.Result.IsSuccessful) {
                    ExceptionlessClient.Default.CreateLog("Create User Email")
                   .SetMessage(emailResult.Result.Error.UserHelp)
                   .AddObject(user)
                    .Submit();
                    return View(model);

                    }
                return RedirectToAction("ForgotPasswordConfirmation","Authentication");

                }
            // If we got this far, something failed, redisplay form
            return View(model);
            }
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation() {
            return View();
            }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code,string c) {
            if (code == null || c == null) {
                return View("Error");
                }
            return View();
            }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model) {
            if (ModelState.IsValid) {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null) {
                    ModelState.AddModelError("","No user found.");
                    return View();
                    }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id,model.Code,model.Password);
                if (result.Succeeded) {
                    return RedirectToAction("ResetPasswordConfirmation","Authentication");
                    } else {
                    AddErrors(result);
                    return View();
                    }
                }

            // If we got this far, something failed, redisplay form
            return View(model);
            }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation() {
            return View();
            }

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError("",error);
                }
            }
        }
    }

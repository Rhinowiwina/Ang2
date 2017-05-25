using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Common.Logging;
using LS.Domain;
using LS.Utilities;
using LS.WebApp.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using LS.ApiBindingModels;
using LS.Core;
using LS.Services;
namespace LS.WebApp.Controllers.api
{
	public abstract class BaseAPIController : ApiController
	{
		//protected ILog Logger { get; set; }

		private ApplicationRole _loggedInUserRole { get; set; }
		private string _loggedInUserCompanyId { get; set; }
		private string _loggedInUserLanguage { get; set; }
		private string _loggedInUserFullName { get; set; }
		private string _loggedInUserFirstName { get; set; }
		private string _loggedInUserLastName { get; set; }
		private string _loggedInUserExternalUserID { get; set; }
		private ApplicationUserManager _AppUserManager = null;

		protected ApplicationUserManager AppUserManager
		{
			get {
				return _AppUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
		}

		protected AuthAndUserManager _authAndUserManager;

		protected AuthAndUserManager AuthAndUserManager
		{
			get { return _authAndUserManager ?? (_authAndUserManager = new AuthAndUserManager(Request)); }
			private set { _authAndUserManager = value; }
		}

		protected string LoggedInUserUserName
		{
			get { return User.Identity.GetUserName(); }
		}

		protected ServerEnvironmentBindingModel ServerVars
		{
			get { return GetServerVars(); }
		}
		protected string LoggedInUserId
		{
			get { return User.Identity.GetUserId(); }
		}

		protected ApplicationUser LoggedInUser
		{
			get {
				return new ApplicationUser
				{
					Id = LoggedInUserId,
					UserName = LoggedInUserUserName,
					CompanyId = GetLoggedInUserCompanyId(),
					Language = GetLoggedInUserLanguage(),
					Role = GetLoggedInUserRole(),
					LastName = GetLoggedInUserLastName(),
					FirstName = GetLoggedInUserFirstName(),
					ExternalUserID = GetLoggedInUserExternalUserID()
				};
			}
		}

		protected BaseAPIController()
		{
			//Logger = LoggerFactory.GetLogger(GetType());
		}

		protected List<Claim> GetAllClaims()
		{
			var userIdentity = (ClaimsIdentity)User.Identity;
			return userIdentity.Claims.ToList();
		}

		protected ServerEnvironmentBindingModel GetServerVars()
		{
			var serv = new UtilityDataService();
			var result = serv.GetServerVars();
			if (!result.Result.IsSuccessful)
			{
				return null;
			}
			return result.Result.Data;
		}
		protected ApplicationRole GetLoggedInUserRole()
		{
			if (_loggedInUserRole != null)
			{
				return _loggedInUserRole;
			}
			var claims = GetAllClaims();
			var roles = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
			return _loggedInUserRole = AuthAndUserManager.GetRoleWithName(roles[0].Value);
		}
		protected string GetLoggedInUserExternalUserID()
		{
			if (_loggedInUserExternalUserID != null)
			{

				return _loggedInUserExternalUserID;
			}
			var claims = GetAllClaims();
			return _loggedInUserExternalUserID = claims.Single(c => c.Type == CustomClaimTypes.ExternalUserIDClaimType).Value;
		}
		protected string GetLoggedInUserCompanyId()
		{
			if (_loggedInUserCompanyId != null)
			{

				return _loggedInUserCompanyId;
			}
			var claims = GetAllClaims();
			return _loggedInUserCompanyId = claims.Single(c => c.Type == CustomClaimTypes.CompanyIdClaimType).Value;
		}

		protected string GetLoggedInUserLanguage()
		{
			if (_loggedInUserLanguage != null)
			{

				return _loggedInUserLanguage;
			}
			var claims = GetAllClaims();
			return _loggedInUserLanguage = claims.Single(c => c.Type == CustomClaimTypes.LanguageClaimType).Value;
		}

		protected string GetLoggedInUserFullName()
		{
			if (_loggedInUserFullName != null)
			{
				return _loggedInUserFullName;
			}
			var claims = GetAllClaims();
			return _loggedInUserFullName = claims.Single(c => c.Type == CustomClaimTypes.FullNameClaimType).Value;
		}

		protected string GetLoggedInUserFirstName()
		{
			if (_loggedInUserFirstName != null)
			{

				return _loggedInUserFirstName;
			}
			var claims = GetAllClaims();
			return _loggedInUserFirstName = claims.Single(c => c.Type == CustomClaimTypes.FirstNameClaimType).Value;
		}

		protected string GetLoggedInUserLastName()
		{
			if (_loggedInUserLastName != null)
			{

				return _loggedInUserLastName;
			}
			var claims = GetAllClaims();
			return _loggedInUserLastName = claims.Single(c => c.Type == CustomClaimTypes.LastNameClaimType).Value;
		}

		protected static string GetModelStateErrorsAsString(ModelStateDictionary modelState,
			string errorSeparator = "\n")
		{
			var errorList = modelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).ToList();
			return String.Join(errorSeparator, errorList);
		}
	}
}
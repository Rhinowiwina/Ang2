using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace LS.Utilities
{
    public class ClaimsHelper
    {
        private string _loggedInUserCompanyId = null;
        private string _loggedInUserFullName = null;

        protected List<Claim> GetAllClaims()
        {
            var userIdentity = (ClaimsIdentity) Thread.CurrentPrincipal.Identity;
            return userIdentity.Claims.ToList();
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

        protected string GetLoggedInUserFullName()
        {
            if (_loggedInUserFullName != null)
            {
                return _loggedInUserFullName;
            }
            var claims = GetAllClaims();
            return _loggedInUserFullName = claims.Single(c => c.Type == CustomClaimTypes.FullNameClaimType).Value;
        }
    }
}

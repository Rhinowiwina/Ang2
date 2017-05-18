using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LS.Core.Interfaces;
using LS.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
namespace LS.Domain
{
    public class ApplicationUser : IdentityUser<string, IdentityUserLogin, ApplicationUserRole, IdentityUserClaim>, IEntity<string>
    {
        [NotMapped]
        private ApplicationRole _role { get; set; }
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
         
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        //This will not create in database because there is no set. This is the way it should be.
        public string FullName
        {
            get
            {
                if (FirstName.Length > 0 && LastName.Length > 0) {
                    return FirstName + " " + LastName;
                } else if (FirstName.Length > 0) {
                    return FirstName;
                } else {
                    return LastName;
                }
            }
        }

        public string PayPalEmail { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public string SalesTeamId { get; set; }
        public SalesTeam SalesTeam { get; set; }
        public string Language { get; set; }
        public string ExternalUserID { get; set; } //tracfone user id
        public bool IsExternalUserIDActive { get; set; }
        public bool PermissionsLifelineCA { get; set; }
        public bool PermissionsBypassTpiv { get; set; }
        public bool PermissionsAccountOrder { get; set; }
        public bool IsActive { get; set; }
        public bool AdditionalDataNeeded { get; set; }
        public bool IsDeleted { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public string CreatedByUserId { get; set; }
        public string ModifiedByUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        [NotMapped]
        public ApplicationRole Role {
            get { return _role ?? (_role = Roles.ToList()[0].Role); }
            set { _role = value; }
        }

        [Timestamp]
        public virtual byte[] RowVersion { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager, string authenticationType) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            userIdentity.AddClaim(new Claim(CustomClaimTypes.CompanyIdClaimType, CompanyId));
            userIdentity.AddClaim(new Claim(CustomClaimTypes.ExternalUserIDClaimType, ExternalUserID == null ? "" : ExternalUserID));
            userIdentity.AddClaim(new Claim(CustomClaimTypes.LanguageClaimType, Language));
            userIdentity.AddClaim(new Claim(CustomClaimTypes.FirstNameClaimType, FirstName));
            userIdentity.AddClaim(new Claim(CustomClaimTypes.LastNameClaimType, LastName));
            userIdentity.AddClaim(new Claim(CustomClaimTypes.FullNameClaimType, FullName));
            // Add custom user claims here
            return userIdentity;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using LS.Core;
using LS.Core.Interfaces;
using LS.Domain;

namespace LS.Services
{
    public abstract class BaseSalesGroupDataService<TEntity, TPK> : BaseDataService<TEntity, TPK>
        where TEntity : class, IEntity<TPK>
    {
        protected ApplicationRole AllowedRole { get; set; }

        protected BaseSalesGroupDataService(int requiredRoleRank)
        {
            var appRoleService = new ApplicationRoleDataService();
            var allRoles = appRoleService.GetAll().Data;
            AllowedRole = allRoles.FirstOrDefault(r => r.Rank == requiredRoleRank);
        }

        protected override string[] GetDefaultIncludes()
        {
            return new[] { "Managers.Roles.Role" };
        }

        protected ServiceProcessingResult<List<ApplicationUser>> GetValidManagersInCompanyFromListOfIds(List<string> ids, string companyId)
        {
            var result = new ServiceProcessingResult<List<ApplicationUser>>();

            if (ids == null || ids.Count <= 0)
            {
                result.Data = new List<ApplicationUser>();
                result.IsSuccessful = true;
                return result;
            }

            var appUserDataService = new ApplicationUserDataService();
            var getUsersResult = appUserDataService.GetUsersInCompanyFromIDList(ids, companyId);
            if (!getUsersResult.IsSuccessful || getUsersResult.Data == null)
            {
                getUsersResult.Data = new List<ApplicationUser>();
                return getUsersResult;
            }
            var users = getUsersResult.Data;

            var verifiedManagers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                var userRoleId = user.Roles.Select(r => r.RoleId).FirstOrDefault();
                if (userRoleId == AllowedRole.Id)
                {
                    verifiedManagers.Add(user);
                }
            }

            result.IsSuccessful = true;
            result.Data = verifiedManagers;
            return result;
        }
    }
}

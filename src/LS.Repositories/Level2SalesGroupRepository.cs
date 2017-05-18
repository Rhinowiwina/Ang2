using System;
using System.Data.Entity;
using System.Linq;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Repositories
{
    public class Level2SalesGroupRepository : BaseRepository<Level2SalesGroup, string>
    {
        public Level2SalesGroupRepository() : base(new AmbientDbContextLocator())
        {
        }

        public override DataAccessResult<Level2SalesGroup> Update(Level2SalesGroup entityToUpdate, EntityState newChildrenState = EntityState.Unchanged)
        {
            var result = new DataAccessResult<Level2SalesGroup>();

            try
            {
                var salesGroupEntity =
                    MainEntity.Include("Managers").SingleOrDefault(sg => sg.Id == entityToUpdate.Id);

                var managerIds = entityToUpdate.Managers.Select(m => m.Id).ToList();
                var managers =
                    DbContext.Set<ApplicationUser>().Include("Roles.Role").Where(u => managerIds.Contains(u.Id)).ToList();
                
                salesGroupEntity.Managers = managers;
                salesGroupEntity.Name = entityToUpdate.Name;
                salesGroupEntity.ParentSalesGroupId = entityToUpdate.ParentSalesGroupId;
                salesGroupEntity.DateModified = DateTime.UtcNow;
                salesGroupEntity.ModifiedByUserId = entityToUpdate.ModifiedByUserId;
                salesGroupEntity.IsDeleted = entityToUpdate.IsDeleted;
                result.IsSuccessful = true;
                result.Data = salesGroupEntity;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                    .SetMessage("Level 2 Sales Group Repository Update failed.")
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                //result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Level 2 Sales Group Repository Update failed.", ex);
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using LinqKit;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Repositories
{
    public class Level3SalesGroupRepository : BaseRepository<Level3SalesGroup, string>
    {
        public Level3SalesGroupRepository() : base(new AmbientDbContextLocator())
        {
        }

        public override DataAccessResult<Level3SalesGroup> Update(Level3SalesGroup entityToUpdate, EntityState newChildrenState = EntityState.Unchanged)
        {
            var result = new DataAccessResult<Level3SalesGroup>();

            try
            {
                var salesGroupEntity =
                    MainEntity.Include("Managers").SingleOrDefault(sg => sg.Id == entityToUpdate.Id);

                var managerIds = entityToUpdate.Managers.Select(m => m.Id).ToList();
                var managers =
                    DbContext.Set<ApplicationUser>()
                        .Include("Roles.Role")
                        .Where(u => managerIds.Contains(u.Id))
                        .ToList();

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
                   .SetMessage(ErrorValues.GENERIC_FATAL_BACKEND_ERROR)
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Level 3 Sales Group Repository Update failed. ", ex);
            }

            return result;
        }

        public DataAccessResult<Level3SalesGroup> GetLevel3SalesGroupWhereManagerInTree(string salesGroupId, string userId,
    string companyId)
        {
            var result = new DataAccessResult<Level3SalesGroup>();

            try
            {
                //TODO: Check the peformance of this query and consider creating a raw SQL query if necessary
                var idPredicate = PredicateBuilder.True<Level3SalesGroup>();
                idPredicate = idPredicate.And(sg => sg.Id == salesGroupId);
                idPredicate = idPredicate.And(sg => sg.CompanyId == companyId);

                var groupManagersPredicate = PredicateBuilder.False<Level3SalesGroup>();
                groupManagersPredicate = groupManagersPredicate.Or(sg => sg.Managers.Any(u => u.Id == userId));
                groupManagersPredicate =
                    groupManagersPredicate.Or(sg => sg.ParentSalesGroup.Managers.Any(u => u.Id == userId));
                groupManagersPredicate = groupManagersPredicate.Or(
                            sg => sg.ParentSalesGroup.ParentSalesGroup.Managers.Any(u => u.Id == userId)
                        );

                var compoundPredicate = PredicateBuilder.True<Level3SalesGroup>();
                compoundPredicate = compoundPredicate.And(idPredicate.Expand());
                compoundPredicate = compoundPredicate.And(groupManagersPredicate.Expand());
                var salesGroupWithManagerInTree = MainEntity.AsExpandable().SingleOrDefault(compoundPredicate);

                result.IsSuccessful = true;
                result.Data = salesGroupWithManagerInTree;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                   .SetMessage("Failed to get Level3SalesGroup where User in tree")
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Failed to get Level3SalesGroup where User in tree", ex);
            }

            return result;
        }
    }
}

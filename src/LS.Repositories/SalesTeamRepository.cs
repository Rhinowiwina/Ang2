using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using LS.Core;
using LS.Domain;
using LS.Repositories.EFConfigs;
using Numero3.EntityFramework.Implementation;
using Numero3.EntityFramework.Interfaces;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Repositories
{
    public class SalesTeamRepository : BaseRepository<SalesTeam, string>
    {
        public SalesTeamRepository() : base(new AmbientDbContextLocator())
        {
        }

        public override DataAccessResult<SalesTeam> Update(SalesTeam updatedSalesTeam, EntityState newChildrenState = EntityState.Unchanged)
        {
            var result = new DataAccessResult<SalesTeam>();

            try
            {
                var existingSalesTeam = GetEntity(updatedSalesTeam.Id);

                existingSalesTeam.Level3SalesGroupId = updatedSalesTeam.Level3SalesGroupId;
                existingSalesTeam.Name = updatedSalesTeam.Name;
                existingSalesTeam.SigType = updatedSalesTeam.SigType;
                existingSalesTeam.Address1 = updatedSalesTeam.Address1;
                existingSalesTeam.Address2 = updatedSalesTeam.Address2;
                existingSalesTeam.City = updatedSalesTeam.City;
                existingSalesTeam.State = updatedSalesTeam.State;
                existingSalesTeam.Zip = updatedSalesTeam.Zip;
                existingSalesTeam.Phone = updatedSalesTeam.Phone;
                existingSalesTeam.TaxId = updatedSalesTeam.TaxId;
                existingSalesTeam.PayPalEmail = updatedSalesTeam.PayPalEmail;
                existingSalesTeam.CycleCountTypeDevice = updatedSalesTeam.CycleCountTypeDevice;
                existingSalesTeam.CycleCountTypeSim = updatedSalesTeam.CycleCountTypeSim;
                existingSalesTeam.IsActive = updatedSalesTeam.IsActive;
                existingSalesTeam.DateModified = DateTime.UtcNow;
                existingSalesTeam.IsDeleted = updatedSalesTeam.IsDeleted;
                existingSalesTeam.ModifiedByUserId = updatedSalesTeam.ModifiedByUserId;
                result.IsSuccessful = true;
                result.Data = existingSalesTeam;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                .SetMessage(String.Format("An error occurred while updating SalesTeam with Id: {0}",
                updatedSalesTeam.Id))
                .MarkAsCritical()
                .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //var logMessage = String.Format("An error occurred while updating SalesTeam with Id: {0}",
                //    updatedSalesTeam.Id);
                //Logger.Error(logMessage, ex);
            }

            return result;
        }

        public DataAccessResult<SalesTeam> UpdateSalesTeamCommissions(SalesTeam updatedSalesTeam)
        {
            var result = new DataAccessResult<SalesTeam>();

            try
            {
                var existingSalesTeam = GetEntity(updatedSalesTeam.Id);


                result.IsSuccessful = true;
                result.Data = existingSalesTeam;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                .SetMessage(String.Format("An error occurred while updating commissions for SalesTeam with Id: {0}",updatedSalesTeam.Id))
                .MarkAsCritical()
                .Submit();

                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //var logMessage = String.Format(
                //    "An error occurred while updating commissions for SalesTeam with Id: {0}", updatedSalesTeam.Id);
                //Logger.Error(logMessage, ex);
            }

            return result;
        }

        public override DataAccessResult Delete(string salesTeamId)
        {
            var result = new DataAccessResult();

            try
            {
                var entityToMarkAsDeleted = GetEntity(salesTeamId);

                entityToMarkAsDeleted.IsActive = false;
                entityToMarkAsDeleted.IsDeleted = true;
                entityToMarkAsDeleted.DateModified = DateTime.UtcNow;

                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                var logMessage = String.Format("An error occurred while marking a SalesTeam with Id: {0} as deleted",
                    salesTeamId);
                ex.ToExceptionless()
                .SetMessage(logMessage)
                .MarkAsCritical()
                .Submit();
               
            }

            return result;
        }

        public async Task<DataAccessResult<SalesTeam>> GetSalesTeamUnderManagerAsync(string salesTeamId, string managerUserId, string companyId)
        {
            var result = new DataAccessResult<SalesTeam>();

            try
            {
                var idPredicate = PredicateBuilder.True<SalesTeam>();
                idPredicate = idPredicate.And(st => st.Id == salesTeamId);
                idPredicate = idPredicate.And(st => st.CompanyId == companyId);

                var managersPredicate = PredicateBuilder.False<SalesTeam>();
                managersPredicate =
                    managersPredicate.Or(st => st.Level3SalesGroup.Managers.Any(u => u.Id == managerUserId));
                managersPredicate =
                    managersPredicate.Or(
                        st => st.Level3SalesGroup.ParentSalesGroup.Managers.Any(u => u.Id == managerUserId));
                managersPredicate =
                    managersPredicate.Or(
                        st => st.Level3SalesGroup.ParentSalesGroup.ParentSalesGroup.Managers.Any(u => u.Id == managerUserId));

                var compoundPredicate = PredicateBuilder.True<SalesTeam>();
                compoundPredicate = compoundPredicate.And(idPredicate.Expand());
                compoundPredicate = compoundPredicate.And(managersPredicate.Expand());

                var salesTeamUnderManager = await MainEntity.AsExpandable().SingleOrDefaultAsync(compoundPredicate);

                result.IsSuccessful = true;
                result.Data = salesTeamUnderManager;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                 .SetMessage("Failed to get SalesTeam under Manager")
                 .MarkAsCritical()
                 .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                
            }

            return result;
        }
    }
}

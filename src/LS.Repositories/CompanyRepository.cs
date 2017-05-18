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
    public class CompanyRepository : BaseRepository<Company, string>
    {
        public CompanyRepository() : base(new AmbientDbContextLocator())
        {
        }
         public override DataAccessResult<Company> Update(Company entityToUpdate, EntityState newChildrenState = EntityState.Unchanged)
        {
            var result = new DataAccessResult<Company>();

            try
            {
                var customerGroupEntity =
                   MainEntity.SingleOrDefault(sg => sg.Id == entityToUpdate.Id);
                
                customerGroupEntity.CompanyLogoUrl = entityToUpdate.CompanyLogoUrl;
				customerGroupEntity.OrderStart = entityToUpdate.OrderStart;
				customerGroupEntity.OrderEnd = entityToUpdate.OrderEnd;
				customerGroupEntity.ShowHandsetOrders = entityToUpdate.ShowHandsetOrders;
				customerGroupEntity.ShowReporting = entityToUpdate.ShowReporting;
				customerGroupEntity.CompanySupportUrl = entityToUpdate.CompanySupportUrl;
                customerGroupEntity.Name = entityToUpdate.Name;
                customerGroupEntity.PrimaryColorHex = entityToUpdate.PrimaryColorHex;
                customerGroupEntity.SecondaryColorHex = entityToUpdate.SecondaryColorHex;
                customerGroupEntity.MaxCommission = entityToUpdate.MaxCommission;
                customerGroupEntity.MinToChangeTeam = entityToUpdate.MinToChangeTeam;
                customerGroupEntity.DateModified = DateTime.UtcNow;
                result.IsSuccessful = true;
                result.Data = customerGroupEntity;
            }
            catch (InvalidOperationException ex)
            {
                ex.ToExceptionless()
                   .SetMessage(ErrorValues.COULD_NOT_FIND_CompaySystem_TO_UPDATE_ERROR)
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.COULD_NOT_FIND_CompaySystem_TO_UPDATE_ERROR;
            }

            return result;
        }
        public DataAccessResult Delete(ApplicationUser systemToMarkAsDeleted)
        {
            var result = new DataAccessResult();

            try
            {
                var existingSystem = GetEntity(systemToMarkAsDeleted.Id);
                existingSystem.IsDeleted = true;
                existingSystem.DateModified = DateTime.UtcNow;

                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                   .SetMessage("Failed to mark user as deleted.")
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Failed to mark user as deleted.", ex);
            }

            return result;
        }

    } }

using System;
using System.Data.Entity;
using System.Linq;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;
namespace LS.Repositories {
    public class SystemStatusRepository : BaseRepository<SystemStatus, string> {
        public SystemStatusRepository() : base(new AmbientDbContextLocator()) {
        }

        public DataAccessResult<SystemStatus> UpdateSystemStatus(SystemStatus upDatedSystem) {
            var result = new DataAccessResult<SystemStatus>();

            try {
                var existingSystem = GetEntity(upDatedSystem.Id);
                existingSystem.Id = upDatedSystem.Id;
                existingSystem.System = upDatedSystem.System;
                existingSystem.Status = upDatedSystem.Status;
                existingSystem.Details = upDatedSystem.Details;
                existingSystem.Eta = upDatedSystem.Eta;
                existingSystem.ExternalSystem = upDatedSystem.ExternalSystem;
                existingSystem.SiteStatusColName = upDatedSystem.SiteStatusColName;
                existingSystem.SortOrder = upDatedSystem.SortOrder;
                existingSystem.Display = upDatedSystem.Display;
                result.IsSuccessful = true;
                result.Data = existingSystem;
            } catch (InvalidOperationException) {
                result.IsSuccessful = false;
                result.Error = ErrorValues.COULD_NOT_FIND_USER_TO_UPDATE_ERROR;
            }

            return result;
        }
        public DataAccessResult Delete(ApplicationUser systemToMarkAsDeleted) {
            var result = new DataAccessResult();

            try {
                var existingSystem = GetEntity(systemToMarkAsDeleted.Id);
                existingSystem.IsDeleted = true;
                result.IsSuccessful = true;
            } catch (Exception ex) {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                Logger.Error("Failed to mark user as deleted.", ex);
            }

            return result;
        }

    }
}
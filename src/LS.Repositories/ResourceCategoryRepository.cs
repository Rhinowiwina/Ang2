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
    public class ResourceCategoryRepository : BaseRepository<ResourceCategory, string>
    {
        public ResourceCategoryRepository() : base(new AmbientDbContextLocator())
        {
        }

        public DataAccessResult Delete(ResourceCategory CategoryToMarkAsDeleted)
        {
            var result = new DataAccessResult();

            try
            {
                var existingSystem = GetEntity(CategoryToMarkAsDeleted.Id);
                existingSystem.IsDeleted = true;
                existingSystem.DateModified = DateTime.UtcNow;

                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless()
                                 .SetMessage("Failed to mark record as deleted")
                                 .MarkAsCritical()
                                 .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
          /*      Logger.Error("Failed to mark user as deleted.", ex)*/;
            }

            return result;
        }

    }
}
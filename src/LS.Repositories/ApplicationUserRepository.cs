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
    public class ApplicationUserRepository : BaseRepository<ApplicationUser, string>
    {
        public ApplicationUserRepository() : base(new AmbientDbContextLocator())
        {
        }

        public override DataAccessResult<ApplicationUser> Update(ApplicationUser entityToUpdate, EntityState newChildrenState = EntityState.Unchanged)
        {
            //This is only to be used by LS.WebApp.Controllers.api, it is updated by an external source. Does not need to check user tree.

            var result = new DataAccessResult<ApplicationUser>();

            try
            {
                var applicationUserEntity = MainEntity.SingleOrDefault(a => a.Id == entityToUpdate.Id);
                applicationUserEntity.DateModified = DateTime.UtcNow;
                applicationUserEntity.ModifiedByUserId = entityToUpdate.ModifiedByUserId;
                
                result.IsSuccessful = true;
                result.Data = applicationUserEntity;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
              
                ex.ToExceptionless()
                  .SetMessage("Application User Repository Update failed. ")
                  .MarkAsCritical()
                  .Submit();

                }

            return result;
        }


        public override DataAccessResult<ApplicationUser> Get(string byId)
        {
            var result = new DataAccessResult<ApplicationUser>();

            try
            {
                var userWithRole = GetEntity(byId);

                result.IsSuccessful = true;
                result.Data = userWithRole;
            }
            catch (Exception ex)
            {
               
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                var logMessage = String.Format("Failed to get user with Id: {0}", byId);
             
                ex.ToExceptionless()
                    .SetMessage(String.Format("Failed to get user with Id: {0}",byId))
                    .AddObject(byId)
                    .AddTags("Repository")
                    .MarkAsCritical()
                    .Submit();
                }

            return result;
        }


        public DataAccessResult<ApplicationUser> UpdateUserOwnedByLoggedInUser(ApplicationUser updatedUser)
        {
            var result = new DataAccessResult<ApplicationUser>();

            try
            {
                var existingUser = GetEntity(updatedUser.Id);

                if (!existingUser.RowVersion.SequenceEqual(updatedUser.RowVersion))
                {
                    result.IsSuccessful = false;
                    result.Error = ErrorValues.UPDATE_USER_WITH_INVALID_ROW_VERSION_ERROR;
                    return result;
                }

                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Email = updatedUser.Email;
                existingUser.ExternalUserID = updatedUser.ExternalUserID;
                existingUser.PayPalEmail = updatedUser.PayPalEmail;
                existingUser.UserName = updatedUser.UserName;
                existingUser.PermissionsBypassTpiv = updatedUser.PermissionsBypassTpiv;
                existingUser.AdditionalDataNeeded = updatedUser.AdditionalDataNeeded;
                existingUser.PermissionsAccountOrder = updatedUser.PermissionsAccountOrder;
                existingUser.DateModified = DateTime.UtcNow;
                existingUser.ModifiedByUserId = updatedUser.Id;
                result.IsSuccessful = true;
                result.Data = existingUser;
            }
            catch (InvalidOperationException ex1)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.COULD_NOT_FIND_USER_TO_UPDATE_ERROR;
                ex1.ToExceptionless()
                    .SetMessage(ErrorValues.COULD_NOT_FIND_USER_TO_UPDATE_ERROR)
                    .AddObject(updatedUser.Id)
                    .AddTags("Repository")
                    .MarkAsCritical()
                    .Submit();
                }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
               
                ex.ToExceptionless()
                   .SetMessage(ErrorValues.GENERIC_FATAL_BACKEND_ERROR)
                   .AddObject(updatedUser.Id)
                   .AddTags("Repository")
                   .MarkAsCritical()
                   .Submit();
                }


            return result;
        }

        public DataAccessResult<ApplicationUser> UpdateUserNotOwnedByLoggedInUser(ApplicationUser updatedUser)
        {
            var result = new DataAccessResult<ApplicationUser>();

            try
            {
                var existingUser = GetEntity(updatedUser.Id);

                if (!existingUser.RowVersion.SequenceEqual(updatedUser.RowVersion))
                {
                    result.IsSuccessful = false;
                    result.Error = ErrorValues.UPDATE_USER_WITH_INVALID_ROW_VERSION_ERROR;
                    return result;
                }

                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.ExternalUserID = updatedUser.ExternalUserID;
                existingUser.IsExternalUserIDActive = updatedUser.IsExternalUserIDActive;
                existingUser.Email = updatedUser.Email;
                existingUser.PayPalEmail = updatedUser.PayPalEmail;
                existingUser.SalesTeamId = updatedUser.SalesTeamId;
                existingUser.IsActive = updatedUser.IsActive;
                existingUser.AdditionalDataNeeded = updatedUser.AdditionalDataNeeded;
                existingUser.PermissionsBypassTpiv = updatedUser.PermissionsBypassTpiv;
                existingUser.PermissionsAccountOrder = updatedUser.PermissionsAccountOrder;
                existingUser.PermissionsLifelineCA = updatedUser.PermissionsLifelineCA;
                existingUser.UserName = updatedUser.UserName;
                existingUser.DateModified = DateTime.UtcNow;
                existingUser.ModifiedByUserId = updatedUser.ModifiedByUserId;
                result.IsSuccessful = true;
                result.Data = existingUser;
            }
            catch (InvalidOperationException ex1)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.COULD_NOT_FIND_USER_TO_UPDATE_ERROR;
                ex1.ToExceptionless()
                 .SetMessage(result.Error)
                 .AddObject(updatedUser)
                 .AddTags("Repository")
                 .MarkAsCritical()
                 .Submit();
                }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
               
                ex.ToExceptionless()
                  .SetMessage(ErrorValues.GENERIC_FATAL_BACKEND_ERROR)
                  .AddObject(updatedUser)
                  .AddTags("Repository")
                  .MarkAsCritical()
                  .Submit();
                }

            return result;
        }

        public DataAccessResult MarkUserAsDeleted(ApplicationUser userToMarkAsDeleted)
        {
            var result = new DataAccessResult();

            try
            {
                var existingUser = GetEntity(userToMarkAsDeleted.Id);
                existingUser.IsDeleted = true;
                existingUser.DateModified = DateTime.UtcNow;
                existingUser.ModifiedByUserId = userToMarkAsDeleted.ModifiedByUserId;
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
           
                ex.ToExceptionless()
                  .SetMessage(result.Error)
                  .AddObject(userToMarkAsDeleted)
                  .AddTags("Repository")
                  .MarkAsCritical()
                  .Submit();
                }

            return result;
        }

        public override DataAccessResult Delete(string id)
        {
            throw new NotImplementedException("Cannot delete user from database.  Please use MarkUserAsDeleted to delete a user.");
        }

        protected override ApplicationUser GetEntity(string id)
        {
            var entity =
                MainEntity.Include("Roles.Role")
                    .Single(u => u.Id == id);

            return entity.IsDeleted ? null : entity;
        }
     }
}

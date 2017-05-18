using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqKit;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Repositories
{
    public class ApplicationRoleRepository : BaseRepository<ApplicationRole, string>
    {
        public ApplicationRoleRepository() : base(new AmbientDbContextLocator())
        {
        }

        public override DataAccessResult<List<ApplicationRole>> GetAll(params string[] includes)
        {
            var result = new DataAccessResult<List<ApplicationRole>>();

            try
            {
                var allRoles = MainEntity.IncludeMany(includes).ToList();

                result.IsSuccessful = true;
                result.Data = allRoles;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
               
                ex.ToExceptionless()
                      .SetMessage("ApplicationRoleRepository GetAll failed.")
                      .MarkAsCritical()
                      .Submit();
                }

            return result;
        }

        public override async Task<DataAccessResult<List<ApplicationRole>>> GetAllAsync(params string[] includes)
        {
            var result = new DataAccessResult<List<ApplicationRole>>();

            try
            {
                var allRoles = await MainEntity.IncludeMany(includes).ToListAsync();

                result.IsSuccessful = true;
                result.Data = allRoles;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                ex.ToExceptionless()
                     .SetMessage("ApplicationRoleRepository GetAllAsync failed.")
                     .MarkAsCritical()
                     .Submit();
               
            }

            return result;
        }

        public override DataAccessResult<ApplicationRole> Get(string byId)
        {
            var result = new DataAccessResult<ApplicationRole>();

            try
            {
                var role = MainEntity.Find(byId);

                result.Data = role;
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                ex.ToExceptionless()
                  .SetMessage("ApplicationRoleRepository Get failed for Id: " + byId)
                  .MarkAsCritical()
                  .Submit();
               
            }

            return result;
        }

        public override DataAccessResult<ApplicationRole> GetWhere(Expression<Func<ApplicationRole, bool>> predicate, params string[] includes)
        {
            var result = new DataAccessResult<ApplicationRole>();

            try
            {
                var subset = MainEntity.IncludeMany(includes).AsExpandable().Where(predicate).SingleOrDefault();

                result.Data = subset;
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                ex.ToExceptionless()
                  .SetMessage("ApplicationRoleRespository GetWhere failed.")
                  .MarkAsCritical()
                  .Submit();
               
            }

            return result;
        }
    }
}

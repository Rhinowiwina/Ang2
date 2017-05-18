using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Core;
using LS.Repositories;
using LS.Domain;
using LS.Core.Interfaces;
using LS.Services.Factories;

namespace LS.Services
{
    public class ResourceCategoryDataService : BaseDataService<ResourceCategory, string>
    {
        public override BaseRepository<ResourceCategory, string> GetDefaultRepository()
        {
            return new ResourceCategoryRepository();
        }
        public async Task<ServiceProcessingResult<List<ResourceCategory>>> GetAllCategoriessAsync(string companyid)
        {
            var processingResult = new ServiceProcessingResult<List<ResourceCategory>> { IsSuccessful = true };

            var includes = new[] {"Resources"};
            var getAllCategoriesResult = await GetAllWhereAsync(u => u.CompanyId==companyid,includes);

            if (!getAllCategoriesResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USERS_ERROR;
                return processingResult;
            }

            processingResult.Data = getAllCategoriesResult.Data;
            return processingResult;
        }
    }
}

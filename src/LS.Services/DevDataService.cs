using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;

namespace LS.Services
{
    public class DevDataService : BaseDataService<DevData, string>
    {
        public override BaseRepository<DevData, string> GetDefaultRepository()
        {
            return new DevDataRepository();
        }

        public async Task<ServiceProcessingResult<DevData>> GetGoodOrder(string state)
        {
            var processingResult = new ServiceProcessingResult<DevData>();

            var getResult = await GetAllWhereAsync(d => d.OrigState==state && d.IsDeleted == false);

            if (!getResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                return processingResult;
            }
            var firstAvailable = getResult.Data.OrderByDescending(d => d.Id).FirstOrDefault();

            if(firstAvailable != null && !firstAvailable.IsDeleted){
                //temporary
                firstAvailable.IsDeleted = true;
                base.Update(firstAvailable);
                processingResult.IsSuccessful = true;
                processingResult.Data = firstAvailable;
                }
            else
            {
                processingResult.IsSuccessful = false;
                processingResult.Data = firstAvailable;
            }
            return processingResult;
        }

        public string GetEnvironment()
        {
            var getResult = ConfigurationSettings.AppSettings["Environment"].ToString();
            return getResult;
        }

    }
}

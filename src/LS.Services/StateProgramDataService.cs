using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services
{
    public class StateProgramDataService : BaseDataService<StateProgram, string>
    {
        public override BaseRepository<StateProgram, string> GetDefaultRepository()
        {
            return new StateProgramRepository();
        }

        public async Task<ServiceProcessingResult<List<StateProgram>>> GetStateProgramsByListOfIds(List<string> stateProgramIds)
        {
            var processingResult = new ServiceProcessingResult<List<StateProgram>>();

            var stateProgramResult = await GetAllWhereAsync(sp => stateProgramIds.Contains(sp.Id));
            if (!stateProgramResult.IsSuccessful || stateProgramResult.Data == null) {
                if (stateProgramResult.IsFatalFailure()) {
                    //Logger.Fatal("A fatal error occurred while retrieving State Programs.");
                    ExceptionlessClient.Default.CreateLog(typeof(StateProgramDataService).FullName,"A fatal error occurred while retrieving State Programs.","Error").AddTags("Data Service Error").Submit();
                    }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_STATE_PROGRAMS_ERROR;

                if (processingResult.IsFatalFailure()) {
                    //Logger.Fatal("A fatal error occurred while retrieving State Programs.");
                    ExceptionlessClient.Default.CreateLog(typeof(StateProgramDataService).FullName,"A fatal error occurred while retrieving State Programs.","Error").AddTags("Data Service Error").Submit();
                    }
                
                return processingResult;
            }


            processingResult = stateProgramResult;
            return processingResult;
        }
    }
}

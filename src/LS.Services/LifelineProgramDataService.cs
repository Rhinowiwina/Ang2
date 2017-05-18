using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services
{
    public class LifelineProgramDataService : BaseDataService<LifelineProgram, string>
    {
        public override BaseRepository<LifelineProgram, string> GetDefaultRepository()
        {
            return new LifelineProgramRepository();
        }

        public async Task<ServiceProcessingResult<List<LifelineProgram>>> GetLifelineProgramsByStateCodeAsync(string stateCode)
        {
            var processingResult = new ServiceProcessingResult<List<LifelineProgram>>();
            var lifelineProgramsResult = await GetAllWhereAsync(lp => lp.StateCode == stateCode && (lp.DateStart <= DateTime.Now && lp.DateEnd >= DateTime.Now));
            if (!lifelineProgramsResult.IsSuccessful || lifelineProgramsResult.Data == null) {
                if (lifelineProgramsResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving Lifeline Programs for StateCode: {0}", stateCode);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(LifelineProgramDataService).FullName,String.Format("A fatal error occurred while retrieving Lifeline Programs for StateCode: {0}",stateCode),"Error").AddTags("Data Service Error").Submit();
                    }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_LIFELINE_PROGRAMS_ERROR;
                return processingResult;
            }

            processingResult.IsSuccessful = true;

            processingResult.Data = lifelineProgramsResult.Data.OrderBy(lp => lp.ProgramName).ToList();
            return processingResult;
        }
    }
}

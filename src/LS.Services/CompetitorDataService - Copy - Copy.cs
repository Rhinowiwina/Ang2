using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
//using LS.Domain;
//using LS.Repositories;
using System;
//using Exceptionless;
//using Exceptionless.Models;
namespace LS.Services
{
    public class CompetitorDataService : BaseDataService<Competitor, string>
    {
        public override BaseRepository<Competitor, string> GetDefaultRepository()
        {
            return new CompetitorRepository();
        }

        public async Task<ServiceProcessingResult<List<Competitor>>> GetCompetitorsByCompanyIdAndStateCode(string companyId, string stateCode){
            var processingResult = new ServiceProcessingResult<List<Competitor>>();

            var competitorResult = await GetAllWhereAsync(c => c.CompanyId == companyId && c.StateCode == stateCode);
            if (!competitorResult.IsSuccessful || competitorResult.Data == null) {
                if (competitorResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving Competitors for CompanyId: {0} and StateCode: {1}",companyId, stateCode);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(CompetitorDataService).FullName,String.Format("A fatal error occurred while retrieving Competitors for CompanyId: {0} and StateCode: {1}",companyId,stateCode),"Error").Submit();
                    }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_COMPETITORS_ERROR;
                return processingResult;
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = competitorResult.Data.OrderBy(c => c.Name).ToList();

            return processingResult;
        }
    }
}

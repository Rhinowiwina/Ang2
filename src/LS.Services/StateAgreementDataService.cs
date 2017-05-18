using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services
{
    public class StateAgreementDataService : BaseDataService<StateAgreement, string>
    {
        private static readonly string SubstituteCompetitorsString = "%%COMPETITORS%%";

        public override BaseRepository<StateAgreement, string> GetDefaultRepository()
        {
            return new StateAgreementRepository();
        }

        public async Task<ServiceProcessingResult<List<StateAgreement>>> GetStateAgreementsByStateCodeAsync(string companyId, string stateCode)
        {
            var processingResult = new ServiceProcessingResult<List<StateAgreement>>();

            // COMPETITORS
            var competitorService = new CompetitorDataService();
            var competitorsResult = await competitorService.GetCompetitorsByCompanyIdAndStateCode(companyId, stateCode);
            if (!competitorsResult.IsSuccessful || competitorsResult.Data == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = competitorsResult.Error;
                return processingResult;
            }

            var stateAgreementsResult = await GetAllWhereAsync(sa => sa.StateCode == stateCode);
            if (!stateAgreementsResult.IsSuccessful || stateAgreementsResult.Data == null) {
                if (stateAgreementsResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving State Agreements for StateCode: {0}", stateCode);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(StateAgreementDataService).FullName,String.Format("A fatal error occurred while retrieving State Agreements for StateCode: {0}",stateCode),"Error").AddTags("Data Service Error").Submit();
                    }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_STATE_AGREEMENTS_ERROR;
                return processingResult;
            }

            if (stateAgreementsResult.Data.Any(a => a.Agreement.Contains(SubstituteCompetitorsString))) {
                var agreementsToSubstituteString = stateAgreementsResult.Data.Where(a => a.Agreement.Contains(SubstituteCompetitorsString)).ToList();
                var competitors = competitorsResult.Data.Select(c => c.Name).ToList();
                var competitorsString = string.Join(", ", competitors);

                foreach (var agreement in agreementsToSubstituteString) {
                    agreement.Agreement = agreement.Agreement.Replace(SubstituteCompetitorsString, competitorsString);
                }
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = stateAgreementsResult.Data.OrderBy(sa => sa.Agreement).ToList();
            return processingResult;
        }
    }
}

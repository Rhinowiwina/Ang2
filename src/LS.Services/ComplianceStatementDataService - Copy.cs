using System.Collections.Generic;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services
{
    public class ComplianceStatementDataService : BaseDataService<ComplianceStatement, string>
    {
        public override BaseRepository<ComplianceStatement, string> GetDefaultRepository()
        {
            return new ComplianceStatementRepository();
        }

        public async Task<ServiceProcessingResult<ComplianceStatement>> GetComplianceStatementByStateCodeAsync(string stateCode)
        {
            var processingResult = new ServiceProcessingResult<ComplianceStatement>();

            var complianceStatementResult = await GetWhereAsync(cs => cs.StateCode == stateCode);
            if (!complianceStatementResult.IsSuccessful || complianceStatementResult.Data == null) {
                if (complianceStatementResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving ComplianceStatements for StateCode: {0}", stateCode);
                    //Logger.Fatal(logMessage);

                    ExceptionlessClient.Default.CreateLog(typeof(ComplianceStatementDataService).FullName,String.Format("A fatal error occurred while retrieving ComplianceStatements for StateCode: {0}",stateCode),"Error").Submit();
                    }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_COMPLIANCE_STATEMENTS_ERROR;
                return processingResult;
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = complianceStatementResult.Data;

            return processingResult;
        }
    }
}

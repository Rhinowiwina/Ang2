using System;
using LS.Core;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;
using LS.Repositories;

namespace LS.Services.ExternalApiIntegration
{
    public class CaliPhoneNumberDataService : BaseDataService<CaliPhoneNumber, string>
    {
        public override BaseRepository<CaliPhoneNumber, string> GetDefaultRepository()
        {
            return new CaliPhoneNumberRepository();
        }

        public ServiceProcessingResult<string> GetNextValidCaliPhoneNumber(string companyId)
        {
            var processingResult = new ServiceProcessingResult<string>();

            using (var contextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    var repository = (CaliPhoneNumberRepository)GetRepository();
                    var result = repository.GetNextValidCaliPhoneNumber(companyId);
                    if (result.IsSuccessful)
                    {
                        processingResult = result.ToServiceProcessingResult(null);
                    }
                    contextScope.SaveChanges();
                }
                catch (Exception exception)
                {
                    processingResult.IsSuccessful = false;
//                    processingResult.Error = ErrorValues.GENERIC_NLAD_PHONE_NUMBER_GENERATION_ERROR;
                }
            }

            return processingResult;
        }
    }
}

using System;
using LS.Core;
using LS.Domain.ExternalApiIntegration.Nlad;
using LS.Repositories;

namespace LS.Services.ExternalApiIntegration
{
    public class NladPhoneNumberDataService : BaseDataService<NladPhoneNumber, string>
    {
        public override BaseRepository<NladPhoneNumber, string> GetDefaultRepository()
        {
            return new NladPhoneNumberRepository();
        }

        public ServiceProcessingResult<string> GetNextValidNladPhoneNumber(string companyId)
        {
            var processingResult = new ServiceProcessingResult<string>();

            using (var contextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    var repository = (NladPhoneNumberRepository) GetRepository();
                    var result = repository.GetNextValidNladPhoneNumber(companyId);
                    if (result.IsSuccessful)
                    {
                        processingResult = result.ToServiceProcessingResult(null);
                    }
                    else
                    {
                        return result.ToServiceProcessingResult(result.Error);
                    }
                    contextScope.SaveChanges();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_NLAD_PHONE_NUMBER_GENERATION_ERROR;
                }
            }

            return processingResult;
        }

        public ServiceProcessingResult UpdatePhoneNumberToBeAvailable(long phoneNumber)
        {
            var processingResult = new ServiceProcessingResult();

            using (var contextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    var repository = (NladPhoneNumberRepository)GetRepository();

                    var getResult = repository.GetWhere(p => p.Number == phoneNumber);
                    if (!getResult.IsSuccessful)
                    {
                        processingResult.IsSuccessful = false;
                        //Todo: Error Value
                    }
                    var phoneNumberEntry = getResult.Data;

                    phoneNumberEntry.IsCurrentlyInUse = false;

                    var updateResult = repository.Update(phoneNumberEntry);
                    if (!updateResult.IsSuccessful)
                    {
                        processingResult.IsSuccessful = false;
                        //Todo: Error Value
                    }
                    contextScope.SaveChanges();
                }
                catch (Exception exception)
                {
                    processingResult.IsSuccessful = false;
                    //Todo: Error Value
                }
            }

            return processingResult;
        }
    }
}

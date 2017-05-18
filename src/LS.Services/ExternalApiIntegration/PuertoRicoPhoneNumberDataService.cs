using System;
using LS.Core;
using LS.Domain.ExternalApiIntegration.PuertoRico;
using LS.Repositories;

namespace LS.Services.ExternalApiIntegration
{
    public class PuertoRicoPhoneNumberDataService : BaseDataService<PuertoRicoPhoneNumber, string>
    {
        public override BaseRepository<PuertoRicoPhoneNumber, string> GetDefaultRepository()
        {
            return new PuertoRicoPhoneNumberRepository();
        }

        public ServiceProcessingResult<string> GetNextValidPuertoRicoPhoneNumber(string companyId)
        {
            var processingResult = new ServiceProcessingResult<string>();

            using (var contextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    var repository = (PuertoRicoPhoneNumberRepository) GetRepository();
                    var result = repository.GetNextValidPuertoRicoPhoneNumber(companyId);
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
                    processingResult.Error = ErrorValues.GENERIC_PuertoRico_PHONE_NUMBER_GENERATION_ERROR;
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
                    var repository = (PuertoRicoPhoneNumberRepository)GetRepository();

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

using System;
using System.Threading.Tasks;
using LS.Core;
using LS.ApiBindingModels;
using System.Configuration;
using Newtonsoft.Json;
using LS.Domain.ExternalApiIntegration;
using Exceptionless;
using RESTModule;

namespace LS.Solix
{
    public class SolixAPI
    {
        private RESTService RESTService { get; set; }

        public SolixAPI() {
            RESTService = new RESTService(new RESTAPIInit {BaseURL = ConfigurationManager.AppSettings["SolixAPIBaseURL"], ApiName = "Solix", Username = ConfigurationManager.AppSettings["SolixAPIUsername"], Password = ConfigurationManager.AppSettings["SolixAPIPassword"], AuthUrl = "authenticate" });
        }

        public async Task<ServiceProcessingResult<SolixAPICreateCustomerResponse>> CreateCustomer(SolixAPICreateCustomerRequest model, DateTime dob) {
            var result = new ServiceProcessingResult<SolixAPICreateCustomerResponse> { IsSuccessful = true, Data = new SolixAPICreateCustomerResponse() };

            result.Data.IsError = false;

            try {
                model.DOB = dob.ToString("MM/dd/yyyy");
                model.ResidentialZip = model.ResidentialZip.Substring(0, 5);
                model.ShippingZip = model.ShippingZip.Substring(0, 5);

                var createCustomerResult = await RESTService.MakeRESTCall("CreateCustomer", "createCustomer", "POST", JsonConvert.SerializeObject(model));
                result.IsSuccessful = createCustomerResult.IsSuccessful;
                result.Error = createCustomerResult.Error;
                result.Data.IsError = createCustomerResult.Data.IsError;
                result.Data.ErrorMessage = createCustomerResult.Data.ErrorMessage;

                if (result.IsSuccessful && !result.Data.IsError) {
                    var response = JsonConvert.DeserializeObject<SolixAPICreateCustomerResponse>(createCustomerResult.Data.APIResult);
                    result.Data.EnrollmentID = response.EnrollmentID;
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Error calling Solix CreateCustomer API")
                   .MarkAsCritical()
                   .Submit();

                result.IsSuccessful = false;
                result.Data.IsError = false;
                result.Error = new ProcessingError("Error calling SOLIX CreateCustomer API", "Error calling SOLIX CreateCustomer API", true, false);
            }

            return result;
        }

        public async Task<ServiceProcessingResult<SolixAPILexusNexusCheckResponse>> LexusNexusCheck(SolixAPILexusNexusCheckRequest model, DateTime dob) {
            var result = new ServiceProcessingResult<SolixAPILexusNexusCheckResponse> { IsSuccessful = true, Data = new SolixAPILexusNexusCheckResponse() };

            result.Data.IsError = false;

            try {
                model.Dob = dob.ToString("MM-dd-yyyy");

                var lexusNexusCheck = await RESTService.MakeRESTCall("LXNXCheck", "lxnxCheck", "POST", JsonConvert.SerializeObject(model));

                result.IsSuccessful = lexusNexusCheck.IsSuccessful;
                result.Error = lexusNexusCheck.Error;
                result.Data.IsError = lexusNexusCheck.Data.IsError;
                result.Data.ErrorMessage = lexusNexusCheck.Data.ErrorMessage;

                if (result.IsSuccessful && !result.Data.IsError) {
                    var response = JsonConvert.DeserializeObject<SolixAPILexusNexusCheckResponse>(lexusNexusCheck.Data.APIResult);
                    result.Data.Id = response.Id;
                    result.Data.Duplicate = response.Duplicate;
                    result.Data.IsError = false;
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Error calling Solix CreateCustomer API")
                   .MarkAsCritical()
                   .Submit();

                result.IsSuccessful = false;
                result.Data.IsError = false;
                result.Error = new ProcessingError("Error calling SOLIX CreateCustomer API", "Error calling SOLIX CreateCustomer API", true, false);
            }
            return result;
        }

        public async Task<ServiceProcessingResult<SolixAPITracFoneVerificationResponse>> TracFoneVerification(SolixAPITracFoneVerificationRequest model, DateTime dob) {
            var result = new ServiceProcessingResult<SolixAPITracFoneVerificationResponse> { IsSuccessful = true, Data = new SolixAPITracFoneVerificationResponse() };

            model.Dob = dob.ToString("MM-dd-yyyy");
            result.Data.IsError = false;

            try {
                model.Dob = dob.ToString("MM-dd-yyyy");

                var tracFoneVerification = await RESTService.MakeRESTCall("TFVerify", "tfVerify", "POST", JsonConvert.SerializeObject(model));

                result.IsSuccessful = tracFoneVerification.IsSuccessful;
                result.Error = tracFoneVerification.Error;
                result.Data.IsError = tracFoneVerification.Data.IsError;
                result.Data.ErrorMessage = tracFoneVerification.Data.ErrorMessage;

                if (result.IsSuccessful && !result.Data.IsError) {
                    var response = JsonConvert.DeserializeObject<SolixAPITracFoneVerificationResponse>(tracFoneVerification.Data.APIResult);

                    result.Data = response;
                    result.Data.IsError = false;
                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Error calling Solix CreateCustomer API")
                   .MarkAsCritical()
                   .Submit();

                result.IsSuccessful = false;
                result.Data.IsError = false;
                result.Error = new ProcessingError("Error calling SOLIX CreateCustomer API", "Error calling SOLIX CreateCustomer API", true, false);
            }
            return result;
        }
    }
}
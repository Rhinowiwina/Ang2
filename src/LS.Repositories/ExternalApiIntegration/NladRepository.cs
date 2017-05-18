using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;
using LS.Utilities;
using Newtonsoft.Json;

namespace LS.Repositories.ExternalApiIntegration {
    public class NladRepository : ILifelineApplicationRepository {
        private ILog Logger { get; set; }
        private static readonly string VerifyEndpoint = "verify?";
        private static readonly string EnrollEndpoint = "subscriber";
        private static readonly string ApiName = "NLAD";
        private static readonly string TransferEndpoint = "transfer";
        private static readonly string SubmitResolutionRequestEndpoint = "resolution";

        private static readonly string UserName = ConfigurationSettings.AppSettings["NladUsername"].ToString();
        private static readonly string Password = ConfigurationSettings.AppSettings["NladPassword"].ToString();

        private static readonly Cookie NladAuthorizationCookie = new Cookie("OBBasicAuth", "fromDialog");
        private const string AuthorizationType = "Basic";

        public NladRepository() {
            Logger = LoggerFactory.GetLogger(GetType());
        }

        public async Task<DataAccessResult<ICheckStatusResponse>> CheckCustomerStatusAsync(ICheckStatusRequestData request) {
            var result = new DataAccessResult<ICheckStatusResponse>();

            try {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApplicationConfig.NladApiBaseUrl + ApplicationConfig.NladApiVersionNumber + "/") }) {
                    //Todo: Move credentials to support multiple tenants
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode(UserName + ":" + Password));
                    cookieContainer.Add(new Uri(ApplicationConfig.NladApiBaseUrl), NladAuthorizationCookie);

                    var nladRequestData = (NladCheckStatusRequestData)request;
                    var jsonRequestData = JsonConvert.SerializeObject(nladRequestData);

                    var logEntry = CreateInitialCheckStatusApiLogEntry(jsonRequestData);

                    var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(VerifyEndpoint, postContent);
                    System.Threading.Thread.Sleep(15000);
                    if (response.Content == null) {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("An error occurred calling NLAD api", "An error occurred calling NLAD api (No Response). Please try again, if error persists contact support", true);
                        return result;
                    }

                    try {
                        var content = await response.Content.ReadAsStringAsync();
                        content = content.Trim();

                        FinishLogEntry(logEntry, content);
                        result.Data = new BaseCheckStatusResponse();
                        if (response.StatusCode == HttpStatusCode.Unauthorized) {
                            result.Data.EnrollmentType = EnrollmentType.Invalid;
                            result.Data.IsSuccessful = false;
                            result.Data.Errors = new List<string> { "Unauthorized NLAD Response. Please try again, if error persists contact support" };
                            result.IsSuccessful = true;
                            return result;
                        }

                        if (!(content.StartsWith("{") && content.EndsWith("}")) && !(content.StartsWith("[") && content.EndsWith("]"))) {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling NLAD api", "An error occurred calling NLAD api (Invalid Response). Please try again, if error persists contact support", true);
                            return result;
                        }

                        if (response.IsSuccessStatusCode) {
                            result.Data.EnrollmentType = EnrollmentType.New;
                            result.Data.IsSuccessful = true;
                            result.IsSuccessful = true;
                            return result;
                        }

                        var nladResponse = JsonConvert.DeserializeObject<NladApiErrorResponse>(content);

                        var errorList = CreateErrorList(nladResponse);
                        var nladRejectionsResult = ProcessNLADRejections(nladResponse);

                        if (DoesErrorListIndicateTransferType(nladRejectionsResult)) {
                            if (nladRejectionsResult.DuplicateSubscriber) {
                                errorList.Remove(NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicateSubscriber]);
                            }
                            result.Data.EnrollmentType = EnrollmentType.Transfer;
                            result.Data.IsSuccessful = true;
                            result.Data.Errors = errorList;
                            result.IsSuccessful = true;
                            return result;
                        }

                        result.Data.EnrollmentType = EnrollmentType.Invalid;
                        result.Data.IsSuccessful = false;
                        result.Data.Errors = errorList;
                        result.IsSuccessful = true;
                        return result;
                    } catch (Exception exception) {
                        Logger.Error("Failed to read NLAD Check Status Api response", exception);
                        return new DataAccessResult<ICheckStatusResponse> {
                            Error = new ProcessingError("An error occurred processing NLAD response", "An error occurred while processing the NLAD response (Unknown Error). Please try again, if error persists contact support", true),
                            IsSuccessful = false
                        };
                    }

                }
            }
            //Todo: Catch more specific exceptions with related errors
            catch (Exception exception) {
                Logger.Error(exception);
                return new DataAccessResult<ICheckStatusResponse> {
                    Error = new ProcessingError("An error occurred calling NLAD api", "An error occurred calling NLAD api (Unknown Error). Please try again, if error persists contact support", true, false),
                    IsSuccessful = false
                };
            }
        }

        //public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitApplicationAsync(ISubmitApplicationRequestData request) {
        //    var result = new DataAccessResult<ISubmitApplicationResponse> {
        //        Data = new NladApiResponse {
        //            AssignedPhoneNumber = request.AssignedTelephoneNumber
        //        }
        //    };
        //    try {
        //        var cookieContainer = new CookieContainer();
        //        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //        using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApplicationConfig.NladApiBaseUrl + ApplicationConfig.NladApiVersionNumber + "/") }) {
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode(UserName + ":" + Password));
        //            cookieContainer.Add(new Uri(ApplicationConfig.NladApiBaseUrl), NladAuthorizationCookie);

        //            var nladRequestData = (NladSubmitRequestData)request;
        //            var jsonRequestData = JsonConvert.SerializeObject(nladRequestData);

        //            var logEntry = CreateInitialSubmitApiLogEntry(jsonRequestData);

        //            var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
        //            var response = await httpClient.PostAsync(EnrollEndpoint, postContent);

        //            if (response.Content == null) {
        //                result.IsSuccessful = false;
        //                result.Error = new ProcessingError("An error occurred calling NLAD api", "Contact support", true);
        //                return result;
        //            }

        //            try {
        //                var content = await response.Content.ReadAsStringAsync();

        //                FinishLogEntry(logEntry, content);

        //                if (response.StatusCode == HttpStatusCode.Unauthorized) {
        //                    return new DataAccessResult<ISubmitApplicationResponse> {
        //                        Data = new NladApiResponse {
        //                            EnrollmentType = EnrollmentType.Invalid,
        //                            Errors = new List<string> { "NLAD Username and password were incorrect" },
        //                            IsSuccessful = false
        //                        },
        //                        IsSuccessful = true,
        //                    };
        //                }

        //                if (response.IsSuccessStatusCode) {
        //                    return new DataAccessResult<ISubmitApplicationResponse> {
        //                        Data = new NladApiResponse {
        //                            EnrollmentType = EnrollmentType.New,
        //                            IsSuccessful = true
        //                        },
        //                        IsSuccessful = true
        //                    };
        //                }

        //                var nladResponse = JsonConvert.DeserializeObject<NladApiErrorResponse>(content);

        //                var errorList = CreateErrorList(nladResponse);
        //                var nladRejectionsResult = ProcessNLADRejections(nladResponse);

        //                if (DoesErrorListIndicateTransferType(nladRejectionsResult)) {
        //                    return new DataAccessResult<ISubmitApplicationResponse> {
        //                        Data = new NladApiResponse {
        //                            EnrollmentType = EnrollmentType.Transfer,
        //                            IsSuccessful = true
        //                        }
        //                    };
        //                }

        //                return new DataAccessResult<ISubmitApplicationResponse> {
        //                    Data = new NladApiResponse {
        //                        EnrollmentType = EnrollmentType.Invalid,
        //                        IsSuccessful = false,
        //                        Errors = errorList,
        //                        ResolutionId = nladResponse.Header.ResolutionId
        //                    },
        //                    IsSuccessful = true
        //                };
        //            } catch (Exception exception) {
        //                FinishLogEntry(logEntry, null);
        //                Logger.Error("Failed to read NLAD Submit Api Response", exception);
        //                return new DataAccessResult<ISubmitApplicationResponse> {
        //                    Error = new ProcessingError("An error occurred processing NLAD response", "", true),
        //                    IsSuccessful = false
        //                };
        //            }

        //        }
        //    }
        //    //Todo: Catch more specific exceptions with related errors
        //    catch (Exception exception) {
        //        Logger.Error(exception);
        //        return new DataAccessResult<ISubmitApplicationResponse> {
        //            Error = new ProcessingError("An error occurred calling NLAD api", "Contact support", true, false),
        //            IsSuccessful = false
        //        };
        //    }
        //}

        //public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitTransferAsync(ISubmitApplicationRequestData request) {
        //    var result = new DataAccessResult<ISubmitApplicationResponse> {
        //        Data = new NladApiResponse {
        //            AssignedPhoneNumber = request.AssignedTelephoneNumber
        //        }
        //    };

        //    try {
        //        var cookieContainer = new CookieContainer();
        //        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //        using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApplicationConfig.NladApiBaseUrl + ApplicationConfig.NladApiVersionNumber + "/") }) {
        //            //Todo: Move credentials to support multiple tenants
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode(UserName + ":" + Password));
        //            cookieContainer.Add(new Uri(ApplicationConfig.NladApiBaseUrl), NladAuthorizationCookie);

        //            var nladRequestData = (NladSubmitRequestData)request;
        //            nladRequestData.TransactionType = "transfer";
        //            var jsonRequestData = JsonConvert.SerializeObject(nladRequestData);

        //            var logEntry = CreateInitialTransferApiLogEntry(jsonRequestData);

        //            var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
        //            var response = await httpClient.PostAsync(TransferEndpoint, postContent);

        //            if (response.Content == null) {
        //                result.IsSuccessful = false;
        //                result.Error = new ProcessingError("An error occurred calling NLAD api", "Contact support", true);
        //                return result;
        //            }

        //            try {
        //                var content = await response.Content.ReadAsStringAsync();

        //                FinishLogEntry(logEntry, content);

        //                if (response.StatusCode == HttpStatusCode.Unauthorized) {
        //                    return new DataAccessResult<ISubmitApplicationResponse> {
        //                        Data = new NladApiResponse {
        //                            EnrollmentType = EnrollmentType.Invalid,
        //                            Errors = new List<string> { "NLAD Username and password were incorrect" },
        //                            IsSuccessful = false
        //                        },
        //                        IsSuccessful = true,
        //                    };

        //                }

        //                if (response.IsSuccessStatusCode) {
        //                    return new DataAccessResult<ISubmitApplicationResponse> {
        //                        Data = new NladApiResponse {
        //                            EnrollmentType = EnrollmentType.Transfer,
        //                            IsSuccessful = true
        //                        },
        //                        IsSuccessful = true
        //                    };

        //                }

        //                var nladResponse = JsonConvert.DeserializeObject<NladApiErrorResponse>(content);

        //                var errorList = CreateErrorList(nladResponse);
        //                var nladRejectionsResult = ProcessNLADRejections(nladResponse);
        //                if (DoesErrorListIndicateTransferType(nladRejectionsResult)) {
        //                    return new DataAccessResult<ISubmitApplicationResponse> {
        //                        Data = new NladApiResponse {
        //                            EnrollmentType = EnrollmentType.Transfer,
        //                            IsSuccessful = true
        //                        }
        //                    };
        //                }

        //                return new DataAccessResult<ISubmitApplicationResponse> {
        //                    Data = new NladApiResponse {
        //                        EnrollmentType = EnrollmentType.Invalid,
        //                        IsSuccessful = false,
        //                        Errors = errorList,
        //                        ResolutionId = nladResponse.Header.ResolutionId
        //                    },
        //                    IsSuccessful = true
        //                };

        //            } catch (Exception exception) {
        //                FinishLogEntry(logEntry, null);
        //                Logger.Error("Failed to read NLAD Transfer Api response", exception);
        //                return new DataAccessResult<ISubmitApplicationResponse> {
        //                    Error = new ProcessingError("An error occurred processing NLAD response", "", true),
        //                    IsSuccessful = false
        //                };
        //            }

        //        }
        //    }
        //    //Todo: Catch more specific exceptions with related errors
        //    catch (Exception exception) {
        //        Logger.Error(exception);
        //        return new DataAccessResult<ISubmitApplicationResponse> {
        //            Error = new ProcessingError("An error occurred calling NLAD api", "Contact support", true, false),
        //            IsSuccessful = false
        //        };
        //    }
        //}

        //public async Task<DataAccessResult<ISubmitApplicationResponse>> SubmitResolutionRequest(ResolutionRequestData requestData) {
        //    var result = new DataAccessResult<ISubmitApplicationResponse> {
        //        Data = new NladApiResponse {
        //            AssignedPhoneNumber = requestData.AssignedPhoneNumber,
        //            TpivCode = requestData.ResolutionId,
        //            TpivBypassMessage = requestData.Description
        //        }
        //    };
        //    try {
        //        var cookieContainer = new CookieContainer();
        //        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        //        using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApplicationConfig.NladApiBaseUrl + ApplicationConfig.NladApiVersionNumber + "/") }) {
        //            //Todo: Move credentials to support multiple tenants
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode(UserName + ":" + Password));
        //            cookieContainer.Add(new Uri(ApplicationConfig.NladApiBaseUrl), NladAuthorizationCookie);

        //            var jsonRequestData = JsonConvert.SerializeObject(requestData);

        //            var logEntry = CreateInitialSubmitResolutionRequestApiLogEntry(jsonRequestData);

        //            var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
        //            var response = await httpClient.PostAsync(SubmitResolutionRequestEndpoint, postContent);

        //            if (response.Content == null) {
        //                result.IsSuccessful = false;
        //                result.Error = new ProcessingError("An error occurred calling NLAD api", "Contact support", true);
        //                return result;
        //            }

        //            try {
        //                var content = await response.Content.ReadAsStringAsync();

        //                FinishLogEntry(logEntry, content);

        //                if (response.StatusCode == HttpStatusCode.Unauthorized) {
        //                    result.Data.IsSuccessful = false;
        //                    result.Data.Errors = new List<string> { "NLAD Username and password were incorrect" };
        //                    result.IsSuccessful = true;
        //                    return result;
        //                }

        //                if (response.IsSuccessStatusCode) {
        //                    result.Data.IsSuccessful = true;
        //                    result.IsSuccessful = true;
        //                    return result;
        //                }

        //                var nladResponse = JsonConvert.DeserializeObject<NladApiErrorResponse>(content);

        //                var errorList = CreateErrorList(nladResponse);
        //                var nladRejectionsResult = ProcessNLADRejections(nladResponse);
        //                if (DoesErrorListIndicateTransferType(nladRejectionsResult)) {
        //                    if (errorList.Contains(NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicateSubscriber])) {
        //                        errorList.Remove(NladErrorCodes.ExpectedErrorCodes[NladErrorCodes.DuplicateSubscriber]);
        //                    }

        //                    result.Data.IsSuccessful = false;
        //                    result.Data.Errors = errorList;
        //                    result.IsSuccessful = true;
        //                    return result;
        //                }

        //                result.Data.IsSuccessful = false;
        //                result.Data.Errors = errorList;
        //                result.IsSuccessful = true;
        //                return result;
        //            } catch (Exception exception) {
        //                FinishLogEntry(logEntry, null);
        //                Logger.Error("Failed to read NLAD Transfer Api response", exception);
        //                return new DataAccessResult<ISubmitApplicationResponse> {
        //                    Error = new ProcessingError("An error occurred processing NLAD response", "", true),
        //                    IsSuccessful = false
        //                };
        //            }

        //        }
        //    }
        //    //Todo: Catch more specific exceptions with related errors
        //    catch (Exception exception) {
        //        Logger.Error(exception);
        //        return new DataAccessResult<ISubmitApplicationResponse> {
        //            Error = new ProcessingError("An error occurred calling NLAD api", "Contact support", true, false),
        //            IsSuccessful = false
        //        };
        //    }
        //}

        private List<string> CreateErrorList(NladApiErrorResponse nladResponse) {
            var translatedErrorList = new List<string>();
            foreach (var errorList in nladResponse.Body) {
                var foundTranslatedErrorCode = false;
                var previousLine = "";
                foreach (var singleErrorLine in errorList) {
                    if (singleErrorLine == NladErrorCodes.InvalidTransfer) {
                        translatedErrorList.Add(previousLine);
                        foundTranslatedErrorCode = true;
                    } else if (NladErrorCodes.ExpectedErrorCodes.ContainsKey(singleErrorLine)) {
                        translatedErrorList.Add(NladErrorCodes.ExpectedErrorCodes[singleErrorLine]);
                        foundTranslatedErrorCode = true;
                    }
                    previousLine = singleErrorLine;
                }
                if (!foundTranslatedErrorCode) {
                    var combinedError = errorList.Aggregate("", (current, errorLine) => current + (errorLine + " "));
                    Logger.Info("Unexpected error code returned from NLAD. " + combinedError);
                    translatedErrorList.Add(combinedError);
                }
            }
            return translatedErrorList;
        }

        private static NLADRejections ProcessNLADRejections(NladApiErrorResponse nladResponse) {
            var NLADRejections = new NLADRejections();
            foreach (var rejectionObject in nladResponse.Body) {
                var rejectionDetailLine = 0;
                foreach (var rejectionDetail in rejectionObject) {
                    rejectionDetailLine = rejectionDetailLine + 1;
                    //NLAD JSON is weird.  We are assuming the codes we are looking for will always be in the third line of the object
                    if (rejectionDetailLine == 3) {
                        if (rejectionDetail.Trim() == "AMS_FAILURE_ANALYSIS") { NLADRejections.InvalidAddress = true; } else if (rejectionDetail.Trim().Contains("TPIV_FAIL")) {
                            NLADRejections.FailedIdentity = true;
                            if (rejectionDetail.Trim() == "TPIV_FAIL_NAME_SSN4" || rejectionDetail.Trim() == "TPIV_FAIL_IDENTITY_NOT_FOUND") { NLADRejections.FailedIdentityNameSSN = true; }
                            if (rejectionDetail.Trim() == "TPIV_FAIL_DOB") { NLADRejections.FailedIdentityDOB = true; }
                        } else if (rejectionDetail.Trim() == "DUPLICATE_PRIMARY_ADDRESS") { NLADRejections.InvalidAddress = true; NLADRejections.DuplicateAddress = true; } else if (rejectionDetail.Trim() == "DUPLICATE_SUBSCRIBER") { NLADRejections.DuplicateSubscriber = true; } else if (rejectionDetail.Trim() == "CANNOT_TRANSFER_WITHIN_60_DAYS") { NLADRejections.CantTransfer = true; } else if (rejectionDetail.Trim() == "TPIV Webservice") { } //Do Nothing - just catching these so doesn't get labelled as Unknown
                                                                                                                                                                                                                                                                                                                                                                                                   else if (rejectionDetail.Trim() == "Subscriber") { } //Do Nothing - just catching these so doesn't get labelled as Unknown
                                                                                                                                                                                                                                                                                                                                                                                                   else if (rejectionDetail.Trim() == "Address") { } //Do Nothing - just catching these so doesn't get labelled as Unknown
                                                                                                                                                                                                                                                                                                                                                                                                   else { NLADRejections.UnknownRejection = true; }
                    }
                }
            }

            return NLADRejections;
        }

        private static bool DoesErrorListIndicateTransferType(NLADRejections nladRejections) {
            if (nladRejections.DuplicateSubscriber && !nladRejections.CantTransfer) {
                return true;
            } else {
                return false;
            }
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static ApiLogEntry CreateInitialCheckStatusApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.NladVerify,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialSubmitApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.Submit,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialTransferApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.NladTransfer,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialSubmitResolutionRequestApiLogEntry(string input) {
            return new ApiLogEntry {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.NladResolutionRequest,
                Input = input
            };
        }

        private static void FinishLogEntry(ApiLogEntry logEntry, string content) {
            logEntry.Response = content;
            logEntry.DateEnded = DateTime.UtcNow;

            var logEntryRepository = new ApiLogEntryRepository();
            logEntryRepository.Add(logEntry);
        }
    }
}
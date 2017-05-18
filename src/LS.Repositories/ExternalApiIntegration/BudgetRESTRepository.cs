using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.BudgetREST;
using LS.Domain.ExternalApiIntegration.Interfaces.BudgetREST;
using LS.Domain.ExternalApiIntegration;
using LS.Utilities;
using Newtonsoft.Json;


namespace LS.Repositories.ExternalApiIntegration
{
    public class BudgetRESTRepository
    {
        private ILog Logger { get; set; }
        private static readonly string OrderEndpoint = "index.cfm/order/";
        private static readonly string OrderCommitEndpoint = "index.cfm/ordercommit/";
        private static readonly string OrderDeviceEndpoint = "index.cfm/orderdevice/";
        private static readonly string SIMEndpoint = "index.cfm/sim/";
        private static readonly string ChangeOrderFulfillmentEndPoint = "index.cfm/changeorderfulfillment/";
        private static readonly string InventoryEndpoint = "index.cfm/inventory/";
        private static readonly string InventoryPriceEndpoint = "index.cfm/inventoryprice/";
        private static readonly string InventoryLocationEndpoint = "index.cfm/inventorylocation/";
        private static readonly string DeviceWarrantyEndpoint = "index.cfm/devicewarranty/";
        private static readonly string ApiName = "BudgetREST";


        private static readonly Cookie BudgetRestAuthorizationCookie = new Cookie("OBBasicAuth", "fromDialog");
        private const string AuthorizationType = "Basic";

        public BudgetRESTRepository()
        {
            Logger = LoggerFactory.GetLogger(GetType());
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public async Task<DataAccessResult<OrderSaveResponse>> OrderSaveAsync(string orderId)
        {
            var result = new DataAccessResult<OrderSaveResponse>();

            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl) })
                {
                    //Todo: Move credentials to support multiple tenants
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                    cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl), BudgetRestAuthorizationCookie);

                    var connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    var initialCatalog = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString).InitialCatalog;
                    var orderSaveRequestData = new IOrderSaveRequest
                    {
                        OrderID = orderId,
                        LSDB = initialCatalog
                    };
                    var jsonRequestData = JsonConvert.SerializeObject(orderSaveRequestData);

                    var logEntry = CreateInitialOrderSaveApiLogEntry(jsonRequestData);

                    var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(OrderEndpoint + orderId, postContent);

                    if (response.Content == null)
                    {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("An error occurred calling Budget REST API", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                        return result;
                    }

                    try
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();

                        FinishLogEntry(logEntry, apiResponse);

                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support", "Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support", true, false);

                            return result;
                        }

                        if (!response.IsSuccessStatusCode) { 
                            result.IsSuccessful = false;
                            return result;
                        }

                        var jsonContent = JsonConvert.DeserializeObject<OrderSaveResponse>(apiResponse);

                        if (jsonContent.IsError)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError(jsonContent.ErrorCode, jsonContent.ErrorMessage, true);
                        }
                        else
                        {
                            result.Data = new OrderSaveResponse();
                            result.Data.OrderID = jsonContent.OrderID;
                            result.IsSuccessful = true;
                        }
                        return result;
                    }
                    catch (Exception exception)
                    {
                        FinishLogEntry(logEntry, null);
                        Logger.Error("Failed to read BudgetREST API Order Save response", exception);
                        return new DataAccessResult<OrderSaveResponse>
                        {
                            Error = new ProcessingError("An error occurred processing BudgetREST API response", "An error occurred processing BudgetREST API response. Please try again, if error persists contact support", true, false),
                            IsSuccessful = false
                        };
                    }

                }
            }
            //Todo: Catch more specific exceptions with related errors
            catch (Exception exception)
            {
                Logger.Error(exception);
                return new DataAccessResult<OrderSaveResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "An error occurred calling BudgetREST api. Please try again, if error persists contact support", true,false),
                    IsSuccessful = false
                };
            }

        }

        public async Task<ServiceProcessingResult<ValidateSIMResponse>> ValidateSim(string iccid)
        {
            var result = new ServiceProcessingResult<ValidateSIMResponse>();

            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl) })
                {
                    //Todo: Move credentials to support multiple tenants
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                    cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl), BudgetRestAuthorizationCookie);

                    var validateSimRequestData = new IValidateSIMRequest
                    {
                        SimID = iccid
                    };
                    var jsonRequestData = JsonConvert.SerializeObject(validateSimRequestData);

                    var logEntry = CreateInitialValidateSimApiLogEntry(jsonRequestData);

                    var  response = await httpClient.GetAsync(SIMEndpoint + iccid);
                       
                    if (response.Content == null)
                    {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("Unknown BudgetRESTAPI Response error", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                        return result;
                    }
                    try
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();

                        FinishLogEntry(logEntry, apiResponse);

                        apiResponse = Regex.Replace(apiResponse, ":00", ":");
                        apiResponse = Regex.Replace(apiResponse, ":0", ":");
                        //FinishLogEntry(logEntry, APIResponse);


                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                            return result;
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("UnSuccessful BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                            return result;
                        }

                        var jsonContent = JsonConvert.DeserializeObject<ValidateSIMResponse>(apiResponse);


                        result.Data = new ValidateSIMResponse
                        {
                            ICCID = jsonContent.ICCID,
                            IMSI = jsonContent.IMSI,
                            IsError = jsonContent.IsError,
                            ErrorCode = jsonContent.ErrorCode,
                            ErrorMessage = jsonContent.ErrorMessage
                        };
                        result.IsSuccessful = true;
                        return result;
                    }
                    catch (Exception exception)
                    {
                        FinishLogEntry(logEntry, null);
                        Logger.Error("Failed to read BudgetREST API Order Save response", exception);
                        return new ServiceProcessingResult<ValidateSIMResponse>
                        {
                            Error = new ProcessingError("An error occurred processing BudgetREST API response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true),
                            IsSuccessful = false
                        };
                    }

                }
            }
            //Todo: Catch more specific exceptions with related errors
            catch (Exception exception)
            {
                Logger.Error(exception);
                return new ServiceProcessingResult<ValidateSIMResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true),
                    IsSuccessful = false
                };
            }
        }

        public async Task<ServiceProcessingResult<ChangeOrderFulfillmentResponse>> ChangeOrderFulfillment(string orderId, string fulfillmentType, string deviceId, string deviceModel)
        {
            var result = new ServiceProcessingResult<ChangeOrderFulfillmentResponse>();

            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl) })
                {
                    //Todo: Move credentials to support multiple tenants
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                    cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl), BudgetRestAuthorizationCookie);

                    var changeOrderFulfillmentRequestData = new IChangeOrderFulfillment
                    {
                        FulfillmentType = fulfillmentType,
                        DeviceID = deviceId,
                        DeviceModel = deviceModel
                    };
                    var jsonRequestData = JsonConvert.SerializeObject(changeOrderFulfillmentRequestData);
                    var logEntry = CreateInitialChangeOrderFulfillmentApiLogEntry(jsonRequestData);
                    var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(ChangeOrderFulfillmentEndPoint + orderId, postContent);

                    if (response.Content == null)
                    {
                        result.IsSuccessful = false;
                        result.Error = new ProcessingError("Unknown BudgetRESTAPI Response error", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                        return result;
                    }
                    try
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();

                        FinishLogEntry(logEntry, apiResponse);

                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                            return result;
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("UnSuccessful BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                            return result;
                        }

                        var jsonContent = JsonConvert.DeserializeObject<ChangeOrderFulfillmentResponse>(apiResponse);

                        if (jsonContent.IsError)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError(jsonContent.ErrorMessage, jsonContent.ErrorMessage, true);
                        }
                        result.Data = new ChangeOrderFulfillmentResponse
                        {
                            ErrorCode = jsonContent.ErrorCode,
                            ErrorMessage = jsonContent.ErrorMessage,
                            IsError = jsonContent.IsError
                        };

                        result.IsSuccessful = true;
                        return result;
                    }
                    catch (Exception exception)
                    {
                        FinishLogEntry(logEntry, null);
                        Logger.Error("Failed to read BudgetREST API Order Save response", exception);
                        return new ServiceProcessingResult<ChangeOrderFulfillmentResponse>
                        {
                            Error = new ProcessingError("An error occurred processing BudgetREST API response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true),
                            IsSuccessful = false
                        };
                    }

                }
            }
            //Todo: Catch more specific exceptions with related errors
            catch (Exception exception)
            {
                Logger.Error(exception);
                return new ServiceProcessingResult<ChangeOrderFulfillmentResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true),
                    IsSuccessful = false
                };
            }

        }


        public async Task<ServiceProcessingResult<InventoryResponse>> InventoryCheck(string deviceid, string agentid)
        {
            var result = new ServiceProcessingResult<InventoryResponse> { IsSuccessful = true };
            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (
                        var httpClient = new HttpClient(handler)
                        {
                            BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl)
                        })
                    {
                        //Todo: Move credentials to support multiple tenants
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                        cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl),
                            BudgetRestAuthorizationCookie);


                        var checkInventoryRequestData = new IInventoryCheckRequest {
                            deviceid = deviceid,
                            agentid = agentid
                        };
                        string identifier;
                        if (string.IsNullOrEmpty(agentid) || agentid == "undefined") {
                            identifier =  deviceid;
                        } else {
                            identifier =  deviceid + "?AgentID=" + agentid;
                        }

                        var jsonRequestData = JsonConvert.SerializeObject(checkInventoryRequestData);

                        var logEntry = CreateInitialInventoryCheckApiLogEntry(jsonRequestData);

                        var response = await httpClient.GetAsync(InventoryEndpoint + identifier);
                       

                        if (response.Content == null)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling Budget REST API","An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                            return result;
                        }

                        try
                        {
                            var apiResponse = await response.Content.ReadAsStringAsync();

                            FinishLogEntry(logEntry, apiResponse);

                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support", "Unauthorized BudgetRESTAPI Response.Please try again, if error persists contact support",false,false);
                                return result;
                            }

                            if (!response.IsSuccessStatusCode)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                                return result;
                            }
                            var jsonContent = JsonConvert.DeserializeObject<InventoryResponse>(apiResponse);

                            if (jsonContent.IsError)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError(jsonContent.ErrorMesage, jsonContent.ErrorMesage, true);
                            }


                            result.Data = new InventoryResponse
                            {
                                IsError = jsonContent.IsError,
                                ErrorMesage = jsonContent.ErrorMesage,
                                ErrorCode = jsonContent.ErrorCode,
                                InInventory = jsonContent.InInventory
                            };

                            result.IsSuccessful = true;
                            return result;
                        }
                        catch (Exception exception)
                        {
                            FinishLogEntry(logEntry, null);
                            Logger.Error("Failed to read BudgetREST API Inventory response", exception);
                            return new ServiceProcessingResult<InventoryResponse>
                            {
                                Error = new ProcessingError("An error occurred processing BudgetREST API response", "An error occurred processing BudgetREST API response. Please try again, if error persists contact support", true),
                                IsSuccessful = false
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ServiceProcessingResult<InventoryResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "An error occurred calling BudgetREST api. Please try again, if error persists contact support", true),
                    IsSuccessful = false
                };
            }
        }

        public async Task<ServiceProcessingResult<OrderCommitResponse>> OrderCommit(string orderid)
        {
            var result = new ServiceProcessingResult<OrderCommitResponse> { IsSuccessful = true };
            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (
                        var httpClient = new HttpClient(handler)
                        {
                            BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl)
                        })
                    {
                        //Todo: Move credentials to support multiple tenants
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                        cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl),
                            BudgetRestAuthorizationCookie);


                        var getOrderRequestData = new IOrderCommitRequest
                        {
                            OrderID = orderid
                        };

                       
                        var jsonRequestData = JsonConvert.SerializeObject(getOrderRequestData);
                        var logEntry = CreateInitialOrderCommitApiLogEntry(jsonRequestData);
                        var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
                        
                        var response = await httpClient.PostAsync(OrderCommitEndpoint + orderid, postContent);


                        if (response.Content == null)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling Budget REST API", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                            return result;
                        }

                        try
                        {
                            var apiResponse = await response.Content.ReadAsStringAsync();

                            FinishLogEntry(logEntry, apiResponse);

                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support", true);

                                return result;
                            }

                            if (!response.IsSuccessStatusCode)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                                return result;
                            }
                            var jsonContent = JsonConvert.DeserializeObject<OrderCommitResponse>(apiResponse);

                            if (jsonContent.IsError)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError(jsonContent.ErrorMessage, jsonContent.ErrorMessage, true);
                            }


                            result.Data = new OrderCommitResponse
                            {
                                IsError = jsonContent.IsError,
                                ErrorMessage = jsonContent.ErrorMessage,
                                ErrorCode = jsonContent.ErrorCode,
                                BudgetMobileID = jsonContent.BudgetMobileID
                            };
                            result.IsSuccessful = true;
                            return result;
                        }
                        catch (Exception exception)
                        {
                            FinishLogEntry(logEntry, null);
                            Logger.Error("Failed to read BudgetREST API get order response", exception);
                            return new ServiceProcessingResult<OrderCommitResponse>
                            {
                                Error = new ProcessingError("An error occurred processing BudgetREST API response", "", true),
                                IsSuccessful = false
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ServiceProcessingResult<OrderCommitResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "Contact support", true),
                    IsSuccessful = false
                };
            }
        }

        public async Task<ServiceProcessingResult<InventoryPriceResponse>> InventoryPrice(string deviceid)
        {
            var result = new ServiceProcessingResult<InventoryPriceResponse> { IsSuccessful = true };
            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (
                        var httpClient = new HttpClient(handler)
                        {
                            BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl)
                        })
                    {
                        //Todo: Move credentials to support multiple tenants
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                        cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl),
                            BudgetRestAuthorizationCookie);
                        
                        var logEntry = CreateInitialInventoryPriceApiLogEntry(deviceid);

                        var response = await httpClient.GetAsync(InventoryPriceEndpoint + deviceid);
                        if (response.Content == null)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling Budget REST API",
                                "Contact support", true);
                            return result;
                        }

                        try
                        {
                            var apiResponse = await response.Content.ReadAsStringAsync();

                            FinishLogEntry(logEntry, apiResponse);

                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.IsSuccessful = false;
                                result.Error.UserHelp =
                                    "Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support";
                                return result;
                            }

                            if (!response.IsSuccessStatusCode)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                                return result;
                            }
                            var jsonContent = JsonConvert.DeserializeObject<InventoryPriceResponse>(apiResponse);

                            if (jsonContent.IsError)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError(jsonContent.ErrorMesage, jsonContent.ErrorMesage, true);
                            }


                            result.Data = new InventoryPriceResponse
                            {
                                IsError = jsonContent.IsError,
                                ErrorMesage = jsonContent.ErrorMesage,
                                ErrorCode = jsonContent.ErrorCode,
                                Price = jsonContent.Price
                            };
                            result.IsSuccessful = true;
                            return result;
                        }
                        catch (Exception exception)
                        {
                            FinishLogEntry(logEntry, null);
                            Logger.Error("Failed to read BudgetREST API Inventory response", exception);
                            return new ServiceProcessingResult<InventoryPriceResponse>
                            {
                                Error = new ProcessingError("An error occurred processing BudgetREST API response", "", true),
                                IsSuccessful = false
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ServiceProcessingResult<InventoryPriceResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "Contact support", true),
                    IsSuccessful = false
                };
            }
        }

        public async Task<ServiceProcessingResult<DeviceWarrantyResponse>> DeviceWarranty(string deviceid)
        {
            var result = new ServiceProcessingResult<DeviceWarrantyResponse> { IsSuccessful = true };
            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (
                        var httpClient = new HttpClient(handler)
                        {
                            BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl)
                        })
                    {
                        //Todo: Move credentials to support multiple tenants
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                        cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl),
                            BudgetRestAuthorizationCookie);

                        var logEntry = CreateInitialDeviceWarrantyApiLogEntry(deviceid);

                        var response = await httpClient.GetAsync(DeviceWarrantyEndpoint + deviceid);
                        if (response.Content == null)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling Budget REST API",
                                "Contact support", true);
                            return result;
                        }

                        try
                        {
                            var apiResponse = await response.Content.ReadAsStringAsync();

                            FinishLogEntry(logEntry, apiResponse);

                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.IsSuccessful = false;
                                result.Error.UserHelp =
                                    "Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support";
                                return result;
                            }

                            if (!response.IsSuccessStatusCode)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                                return result;
                            }
                            var jsonContent = JsonConvert.DeserializeObject<DeviceWarrantyResponse>(apiResponse);

                            if (jsonContent.IsError)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError(jsonContent.ErrorMesage, jsonContent.ErrorMesage, true);
                            }


                            result.Data = new DeviceWarrantyResponse
                            {
                                IsError = jsonContent.IsError,
                                ErrorMesage = jsonContent.ErrorMesage,
                                ErrorCode = jsonContent.ErrorCode,
                                UnderWarranty = jsonContent.UnderWarranty
                            };
                            result.IsSuccessful = true;
                            return result;
                        }
                        catch (Exception exception)
                        {
                            FinishLogEntry(logEntry, null);
                            Logger.Error("Failed to read BudgetREST API Device Warranty response", exception);
                            return new ServiceProcessingResult<DeviceWarrantyResponse>
                            {
                                Error = new ProcessingError("An error occurred processing BudgetREST API response", "", true),
                                IsSuccessful = false
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ServiceProcessingResult<DeviceWarrantyResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "Contact support", true),
                    IsSuccessful = false
                };
            }
        }

        public async Task<ServiceProcessingResult<InventoryLocationResponse>> InventoryLocation(string deviceid)
        {
            var result = new ServiceProcessingResult<InventoryLocationResponse> { IsSuccessful = true };
            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (
                        var httpClient = new HttpClient(handler)
                        {
                            BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl)
                        })
                    {
                        //Todo: Move credentials to support multiple tenants
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                        cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl),
                            BudgetRestAuthorizationCookie);

                        var logEntry = CreateInitialInventoryLocationApiLogEntry(deviceid);

                        var response = await httpClient.GetAsync(InventoryLocationEndpoint + deviceid);
                        if (response.Content == null)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling Budget REST API",
                                "Contact support", true);
                            return result;
                        }

                        try
                        {
                            var apiResponse = await response.Content.ReadAsStringAsync();

                            FinishLogEntry(logEntry, apiResponse);

                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.IsSuccessful = false;
                                result.Error.UserHelp =
                                    "Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support";
                                return result;
                            }

                            if (!response.IsSuccessStatusCode)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                                return result;
                            }
                            var jsonContent = JsonConvert.DeserializeObject<InventoryLocationResponse>(apiResponse);

                            if (jsonContent.IsError)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError(jsonContent.ErrorMesage, jsonContent.ErrorMesage, true);
                            }


                            result.Data = new InventoryLocationResponse
                            {
                                IsError = jsonContent.IsError,
                                ErrorMesage = jsonContent.ErrorMesage,
                                ErrorCode = jsonContent.ErrorCode,
                                Location = jsonContent.Location
                            };
                            result.IsSuccessful = true;
                            return result;
                        }
                        catch (Exception exception)
                        {
                            FinishLogEntry(logEntry, null);
                            Logger.Error("Failed to read BudgetREST API Inventory response", exception);
                            return new ServiceProcessingResult<InventoryLocationResponse>
                            {
                                Error = new ProcessingError("An error occurred processing BudgetREST API response", "", true),
                                IsSuccessful = false
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ServiceProcessingResult<InventoryLocationResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "Contact support", true),
                    IsSuccessful = false
                };
            }
        }

        public async Task<ServiceProcessingResult<OrderDeviceResponse>> OrderDevice(string deviceId, string IMSI, string orderId)
        {
            var result = new ServiceProcessingResult<OrderDeviceResponse> {};
            try
            {
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
                {
                    using (
                        var httpClient = new HttpClient(handler)
                        {
                            BaseAddress = new Uri(ApplicationConfig.BudgetRESTApiBaseUrl)
                        })
                    {
                        //Todo: Move credentials to support multiple tenants
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            AuthorizationType, Base64Encode("LifeServPortal" + ":" + "Eo8J7oLlqNf4Xn4G"));
                        cookieContainer.Add(new Uri(ApplicationConfig.BudgetRESTApiBaseUrl),
                            BudgetRestAuthorizationCookie);

                        var logEntry = CreateInitialOrderDeviceApiLogEntry(deviceId);

                        var jsonRequestData = JsonConvert.SerializeObject(new {orderId = orderId, deviceId=deviceId, imsi = IMSI});
                        var postContent = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(OrderDeviceEndpoint + orderId, postContent);
                        if (response.Content == null)
                        {
                            result.IsSuccessful = false;
                            result.Error = new ProcessingError("An error occurred calling Budget REST API",
                                "Contact support", true);
                            return result;
                        }

                        try
                        {
                            var apiResponse = await response.Content.ReadAsStringAsync();

                            FinishLogEntry(logEntry, apiResponse);

                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                result.IsSuccessful = false;
                                result.Error.UserHelp =
                                    "Unauthorized BudgetRESTAPI Response. Please try again, if error persists contact support";
                                return result;
                            }

                            if (!response.IsSuccessStatusCode)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError("Unauthorized BudgetRESTAPI Response", "An error occurred calling Budget REST API. Please try again, if error persists contact support", true);
                                return result;
                            }
                            var jsonContent = JsonConvert.DeserializeObject<OrderDeviceResponse>(apiResponse);

                            if (jsonContent.IsError)
                            {
                                result.IsSuccessful = false;
                                result.Error = new ProcessingError(jsonContent.ErrorMessage, jsonContent.ErrorMessage, true);
                            }


                            result.Data = new OrderDeviceResponse
                            {
                                IsError = jsonContent.IsError,
                                ErrorMessage = jsonContent.ErrorMessage,
                                ErrorCode = jsonContent.ErrorCode
                            };
                            if (!jsonContent.IsError) {
                                result.IsSuccessful = true;
                            }

                            return result;
                        }
                        catch (Exception exception)
                        {
                            FinishLogEntry(logEntry, null);
                            Logger.Error("Failed to read BudgetREST API OrderDevice response", exception);
                            return new ServiceProcessingResult<OrderDeviceResponse>
                            {
                                Error = new ProcessingError("An error occurred processing BudgetREST API response", "", true),
                                IsSuccessful = false
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ServiceProcessingResult<OrderDeviceResponse>
                {
                    Error = new ProcessingError("An error occurred calling BudgetREST api", "Contact support", true),
                    IsSuccessful = false
                };
            }
        }


        private static ApiLogEntry CreateInitialOrderSaveApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.OrderSave,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialDeviceWarrantyApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.DeviceWarranty,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialValidateSimApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.ValidateSIM,
                Input = input
            };
        }


        private static ApiLogEntry CreateInitialInventoryCheckApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.InventoryCheck,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialInventoryPriceApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.InventoryPrice,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialInventoryLocationApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.InventoryLocation,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialChangeOrderFulfillmentApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.ChangeOrderFulfillment,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialGetOrderApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.GetOrder,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialOrderDeviceApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.OrderDevice,
                Input = input
            };
        }

        private static ApiLogEntry CreateInitialOrderCommitApiLogEntry(string input)
        {
            return new ApiLogEntry
            {
                Api = ApiName,
                DateStarted = DateTime.UtcNow,
                Function = ApiFunctions.OrderCommit,
                Input = input
            };
        }

        private static void FinishLogEntry(ApiLogEntry logEntry, string content)
        {
            logEntry.Response = content;
            logEntry.DateEnded = DateTime.UtcNow;

            var logEntryRepository = new ApiLogEntryRepository();
            logEntryRepository.Add(logEntry);
        }
    }


}
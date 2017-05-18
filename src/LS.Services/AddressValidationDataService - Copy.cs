using System.Collections.Generic;
using System.Linq;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Threading.Tasks;
using LS.Utilities;
using LS.ApiBindingModels;
using System;
using System.Data;
using System.Text.RegularExpressions;
using LS.Services.ServiceObjectsService;
using System.Data.SqlClient;
using System.ServiceModel;
using Exceptionless;
using LS.Services.Logging;

namespace LS.Services {
    public class AddressValidationDataService {
       //Required Tag/Optional Value : Address Line 1 is used to provide an apartment or suite number, if applicable.  Maximum characters allowed: 38. For example: <Address1></Address1>
        public async Task<ServiceProcessingResult<AddressStandardizeResponse>> Standardize(AddressStandardizeRequest model) {
            var retResult = new ServiceProcessingResult<AddressStandardizeResponse> { IsSuccessful = true, Data = new AddressStandardizeResponse() };
            /// <remark>If we have any problems validating the address, just return the inputted address</remark>
            if (string.IsNullOrEmpty(model.Address2)) { retResult.Data.Address2 = ""; } else { retResult.Data.Address2 = model.Address2.Trim(); }
            retResult.Data.Address1 = model.Address.Trim();
            retResult.Data.City = model.City.Trim();
            retResult.Data.State = model.State.Trim();
            retResult.Data.Zip = model.Zip.Trim();

            /// <summary>Perform basic validation on inputted address</summary>
            #region Basic Validation
                if (retResult.Data.Address1 == "") {
                    retResult.Data.ValidationRejections.Add("ADDRESS 1 IS MISSING");
                    retResult.Data.IsBypassable = false;
                    return retResult;
                }
                Regex r = new Regex("(?:[^a-z0-9/ ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                if (r.IsMatch(retResult.Data.Address1)) {
                    retResult.Data.ValidationRejections.Add("PLEASE REMOVE THE SPECIAL CHARACTER FROM THE ADDRESS FIELD.");
                    retResult.Data.IsBypassable = false;
                    return retResult;
                }
                if (r.IsMatch(retResult.Data.Address2)) {
                    retResult.Data.ValidationRejections.Add("PLEASE REMOVE THE SPECIAL CHARACTER FROM THE ADDRESS FIELD.");
                    retResult.Data.IsBypassable = false;
                    return retResult;
                }
                if (retResult.Data.Address2 != "" && retResult.Data.Address2.Substring(0, 1) == "0") {
                    retResult.Data.ValidationRejections.Add("THE MAIN ADDRESS VALIDATED BUT 0 IN FRONT OF SUPPLEMENTAL ADDRESS HAS BEEN DESIGNATED AN INVALID SUPPLEMENTAL ADDRESS.PLEASE VERIFY ADDRESS AND TRY AGAIN.");
                    retResult.Data.IsBypassable = false;
                    return retResult;
                }
                if (retResult.Data.City == "") {
                    retResult.Data.ValidationRejections.Add("CITY IS MISSING");
                    retResult.Data.IsBypassable = false;
                    return retResult;
                }
                if (retResult.Data.State == "") {
                    retResult.Data.ValidationRejections.Add("STATE IS MISSING");
                    retResult.Data.IsBypassable = false;
                    return retResult;
                }
                if (model.Zip.Trim() == "") {
                    retResult.Data.ValidationRejections.Add("ZIP CODE IS MISSING");
                    retResult.Data.IsBypassable = false;
                    return retResult;
                }        
            #endregion

            /// <summary>Make API call to external address validation/scrub and process the results</summary>
            #region External Address Validation Call/Processing
            try {
                string UnmodifiedAddress = "";
                
                if (model.Address.Trim() != "") {
                    if (model.Address.Trim().Substring(0, 1) == "0") {
                        UnmodifiedAddress = model.Address;
                        model.Address = model.Address.Remove(0, 1);
                    }
                }

                var validatedAddressResult = ValidateAddressWithSuiteLink(model.Address, model.Address2, model.City, model.State, model.Zip);
                if (validatedAddressResult.IsSuccessful && !validatedAddressResult.Data.IsValid && UnmodifiedAddress != "") { 
                    //If error returned by ServiceObjects during validation, and we had modified the address, try again with unmodified address
                    validatedAddressResult = ValidateAddressWithSuiteLink(UnmodifiedAddress, model.Address2, model.City, model.State, model.Zip);
                }

                if (!validatedAddressResult.IsSuccessful) {
                    retResult.IsSuccessful = false;
                    retResult.Error = validatedAddressResult.Error;
                    return retResult;
                }

                if (validatedAddressResult.Data.IsValid) {
                    #region DPVCode Processing
                    //TODO: If a DPV code other than 1 is still a "scrubbed"/valid address, then we should move the code that sets the processingResult.Data to here

                    switch (Convert.ToInt32(validatedAddressResult.Data.Data.DPV)) {
                        case 1:
                            retResult.Data.Address1 = validatedAddressResult.Data.Data.Address.ToString().Trim();

                            if (validatedAddressResult.Data.Data.Address2.ToString().Trim().Length > 0) {
                                retResult.Data.Address1 = retResult.Data.Address1 + " " + validatedAddressResult.Data.Data.Address2.ToString().Trim();
                            }

                            retResult.Data.Address2 = "";
                            retResult.Data.City = validatedAddressResult.Data.Data.City;
                            retResult.Data.State = validatedAddressResult.Data.Data.State;
                            retResult.Data.Zip = validatedAddressResult.Data.Data.Zip;

                            if (retResult.Data.Zip.Substring(retResult.Data.Zip.Length - 3, 3) == "000") {
                                retResult.Data.ValidationRejections.Add("INVALID ZIPCODE");
                                retResult.Data.IsBypassable = false;
                                return retResult;
                            }
                            if (retResult.Data.Address1.IndexOf("#") > 1 && (retResult.Data.Address1.Length - retResult.Data.Address1.IndexOf("#"))>9) {
                                retResult.Data.ValidationRejections.Add("THE MAIN ADDRESS VALIDATED BUT SUPPLEMENTAL(APT,SUITE) INFORMATION IS NOT FORMATTED CORRECTLY. PLEASE VERIFY ADDRESS AND TRY AGAIN.");
                                return retResult;
                            }
                            //Checking to see if the Fragment ("The parsed "Fragment" box, apartment or unit number. Same as FragmentPMBNumber.") starts with "0"
                            if (validatedAddressResult.Data.Data.Fragment.ToString().Trim() != "" && validatedAddressResult.Data.Data.Fragment.ToString().Trim().Substring(0, 1) == "0") {
                                retResult.Data.ValidationRejections.Add("THE MAIN ADDRESS VALIDATED BUT 0 HAS BEEN DESIGNATED AN INVALID SUPPLEMENTAL ADDRESS. PLEASE VERIFY ADDRESS AND TRY AGAIN.");
                                retResult.Data.IsBypassable = false;
                                return retResult;
                            }
                            if (validatedAddressResult.Data.Data.FragmentHouse.ToString().Trim().Length < 1) {
                                retResult.Data.ValidationRejections.Add("THE MAIN ADDRESS WAS NOT ABLE TO BE VALIDATED. PLEASE VERIFY ADDRESS AND TRY AGAIN.");
                                return retResult;
                            }
                            if (retResult.Data.State.Trim() != model.State.Trim()) {
                                retResult.Data.ValidationRejections.Add("THE VALIDATED STATE IS NOT MATCHING THE STATE YOU ENTERED.");
                                retResult.Data.IsBypassable = false;
                                return retResult;
                            }

                            if (model.ValidateLifeline) {
                                //TODO: Can we make this un-bypassable?
                                if ((retResult.Data.Address1).IndexOf("PO BOX", StringComparison.CurrentCultureIgnoreCase) != -1) {
                                    retResult.Data.ValidationRejections.Add("LIFELINE SERVICES ARE NOT AVAILABLE FOR PO BOXES.");
                                    retResult.Data.IsBypassable = false;
                                    return retResult;
                                }
                                if ((retResult.Data.Address1).IndexOf("GENERAL DELIVERY", StringComparison.CurrentCultureIgnoreCase) != -1) {
                                    retResult.Data.ValidationRejections.Add("Lifeline Services are not available for GENERAL DELIVERY.");
                    retResult.Data.IsBypassable = false;
                                    return retResult;
                                }
                            }

                            /// <summary>Black List Check</summary>
                            #region Blacklist Check
                            try {
                                var sqlClient = new SQLQuery();
                                var sqlText = @"
                                    SELECT * 
                                    FROM BlackListedAddresses (NOLOCK)
                                    WHERE 1=1
                                      AND LTRIM(RTRIM(MainAddress)) = @Address1
                                      AND LTRIM(RTRIM(SecondaryAddressUnit)) = @Address2
                                      AND (LTRIM(RTRIM(City)) = @City OR Zip = @Zip) 
                                      AND LTRIM(RTRIM(State)) = @State 
                                      AND [Override] = 0
                                ";
                                SqlParameter[] parameters = new SqlParameter[] {
                                    new SqlParameter("@Address1", retResult.Data.Address1),
                                    new SqlParameter("@Address2", retResult.Data.Address2),
                                    new SqlParameter("@City", retResult.Data.City),
                                    new SqlParameter("@State", retResult.Data.State),
                                    new SqlParameter("@Zip", retResult.Data.Zip)
                                };
                                var addressBlackListResult = await sqlClient.ExecuteReaderAsync(CommandType.Text, sqlText, parameters);
                                if (!addressBlackListResult.IsSuccessful) {
                                    retResult.Data.ValidationRejections.Add("Error retrieving blacklisted addresses.");
                                    retResult.Data.IsBypassable = false;
                                    return retResult;
                                }
                                if (addressBlackListResult.Data.Rows.Count > 1) {
                                    retResult.Data.ValidationRejections.Add("THIS ADDRESS IS INVALID FOR SERVICE.");
                                    retResult.Data.IsBypassable = false;
                                    return retResult;
                                }
                            } catch (Exception E) {
                                retResult.Data.ValidationRejections.Add(E.Message);
                                return retResult;
                            }
                            #endregion

                            retResult.Data.IsValid = true;
                            break;
                        default:
                            retResult.Data.ValidationRejections.Add("DPV Code:" + validatedAddressResult.Data.Data.DPV + " (" + validatedAddressResult.Data.Data.DPVDesc + ")");
                            break;
                    }
                    #endregion
                } else {
                    if (validatedAddressResult.Data.Errors.Count > 0) {
                        foreach (var error in validatedAddressResult.Data.Errors) {
                            retResult.Data.ValidationRejections.Add(error);
                        }
                    } else {
                        retResult.Data.ValidationRejections.Add("Unknown Rejection.");
                    }
                }
                
                #endregion              
            } catch (Exception ex) {
                retResult.Data.ValidationRejections.Add("Unknown rejection (error processing address: "+ ex.Message + ").");
                ex.ToExceptionless().AddTags("Address Validation").AddObject(model,"Model").Submit();
            }

            return retResult;
        }

        public ServiceProcessingResult<ValidationResult<DPVAddress>> ValidateAddressWithSuiteLink(string Address1, string Address2, string City, string State, string Zip) {
            var processingResult = new ServiceProcessingResult<ValidationResult<DPVAddress>> { IsSuccessful = false };
            processingResult.Data = new ValidationResult<DPVAddress> { IsValid = false, Data = new DPVAddress() };
            var addressValidationResult = new DPVAddress();

            try {
                var licenseKey = "WS3-PFJ4-GEC2";
                
                var binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                var addressValidation = new DOTSAddressValidateSoapClient(binding, new EndpointAddress("https://ws.serviceobjects.com/av/AddressValidate.asmx"));
                addressValidation.Endpoint.Behaviors.Add(new ValidateAddressWithSuiteLinkInspectorBehavior());
                addressValidationResult = addressValidation.ValidateAddressWithSuiteLink(string.Empty, RemoveSpecialCharacters(Address1 + " " + Address2), string.Empty, City, State, Zip, licenseKey);
            } catch (Exception ex) {
                ex.ToExceptionless().AddObject(Address1 + " | " + Address2 + " | " + City + " | " + State + " | " + Zip).Submit();
                processingResult.Error = new ProcessingError("Error calling Address Validation service", "Error calling Address Validation service", false, false);
                return processingResult;
            }

            processingResult.IsSuccessful = true;
            processingResult.Data.Data = addressValidationResult;

            //At this point the call is "successful", but we need to check if the address is valid by processing the Error container
            if (addressValidationResult.Error != null) {
                processingResult.Data.Errors.Add(addressValidationResult.Error.Desc);
                return processingResult;
            }
            processingResult.Data.IsValid = true;

            return processingResult;
        }

        public static string RemoveSpecialCharacters(string input) {
            Regex r = new Regex("(?:[^a-z0-9/ ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, String.Empty);
        }
    }
}
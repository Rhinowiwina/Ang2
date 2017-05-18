using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Services.Factories;
using LS.ApiBindingModels;
using LS.Domain.ExternalApiIntegration;
using System.Configuration;
using System;

namespace LS.Services {
    public class ValidateAddressDataService {

        public async Task<ServiceProcessingResult<ValidatedAddresses>> ValidateAddressesAsync(GetTFVerifyInfoRequestBindingModel request) {

            //Only validates service address from tfVerify function in ordes controller
            var validatedAddresses = new ValidatedAddresses();
            var processingResult = new ServiceProcessingResult<ValidatedAddresses> { IsSuccessful = true, Data = validatedAddresses };

            bool AddressVelocityBypass = false;
            var addressValidationService = new AddressValidationDataService();

            validatedAddresses.ServiceAddressStreet1 = request.Address1;
            validatedAddresses.ServiceAddressStreet2 = request.Address2;
            validatedAddresses.ServiceAddressCity = request.City;
            validatedAddresses.ServiceAddressState = request.State;
            validatedAddresses.ServiceAddressZip = request.Zip;
            validatedAddresses.ValidatedAddressID = -1;
            validatedAddresses.ServiceAddressIsValid = true;
            validatedAddresses.AllowServiceAddressBypass = false;

            var addressToStandardize = new AddressStandardizeRequest {
                Address = request.Address1,
                Address2 = request.Address2,
                City = request.City,
                State = request.State,
                Zip = request.Zip,
                ValidateLifeline = true
            };

            var serviceAddressResult = await addressValidationService.Standardize(addressToStandardize);
            if (!serviceAddressResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = serviceAddressResult.Error;
                return processingResult;
            }

            if (serviceAddressResult.Data.IsValid) {
                validatedAddresses.ServiceAddressCity = serviceAddressResult.Data.City;
                validatedAddresses.ServiceAddressState = serviceAddressResult.Data.State;
                validatedAddresses.ServiceAddressStreet1 = serviceAddressResult.Data.Address1;
                validatedAddresses.ServiceAddressStreet2 = serviceAddressResult.Data.Address2;
                validatedAddresses.ServiceAddressZip = serviceAddressResult.Data.Zip;
            } else {
                if (request.HasServiceAddressBypass && serviceAddressResult.Data.IsBypassable) {
                    //Do nothing.  The invalid address has been bypassed.
                } else {
                    if (request.AddressBypassCount > Convert.ToInt32(ConfigurationManager.AppSettings["NumServiceAddressCorrections"]) && serviceAddressResult.Data.IsBypassable) {
                        AddressVelocityBypass = true;
                    } else {
                        validatedAddresses.ServiceAddressIsValid = false;
                        validatedAddresses.Errors.Add("Service Address Rejection");
                        foreach (var msg in serviceAddressResult.Data.ValidationRejections) {
                            validatedAddresses.Errors.Add("- " + msg);
                        }
                        //IsSuccessful set to true to be inline with the way tpiv bypass works. Js checks for true and if has errors to open by pass html
                        processingResult.IsSuccessful = true;
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["AllowAddressBypasses"])) {
                            validatedAddresses.AllowServiceAddressBypass = true;
                        }
                        processingResult.Data = validatedAddresses;
                        return processingResult;
                    }
                }
            }

            //var shippingAddressValid = false;
            if ((request.City == request.ShippingCity) && (request.State == request.ShippingState) && (request.Address1 == request.ShippingAddress1) && (request.Address2 == request.ShippingAddress2) && (request.Zip == request.ShippingZip) && !AddressVelocityBypass) {
                //set shipping address to scrubbed service address
                validatedAddresses.ShippingAddressCity = validatedAddresses.ServiceAddressCity;
                validatedAddresses.ShippingAddressState = validatedAddresses.ServiceAddressState;
                validatedAddresses.ShippingAddressStreet1 = validatedAddresses.ServiceAddressStreet1;
                validatedAddresses.ShippingAddressStreet2 = validatedAddresses.ServiceAddressStreet2;
                validatedAddresses.ShippingAddressZip = validatedAddresses.ServiceAddressZip;
                validatedAddresses.ShippingAddressIsValid = true;
                //shippingAddressValid = true;
            } else {
                var shippingAddressResult = await addressValidationService.Standardize(new AddressStandardizeRequest {
                    City = request.ShippingCity,
                    State = request.ShippingState,
                    Address = request.ShippingAddress1,
                    Address2 = request.ShippingAddress2,
                    Zip = request.ShippingZip,
                    ValidateLifeline = false
                });

                if (!shippingAddressResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = shippingAddressResult.Error;
                    return processingResult;
                }

                validatedAddresses.ShippingAddressCity = shippingAddressResult.Data.City;
                validatedAddresses.ShippingAddressState = shippingAddressResult.Data.State;
                validatedAddresses.ShippingAddressStreet1 = shippingAddressResult.Data.Address1;
                validatedAddresses.ShippingAddressStreet2 = shippingAddressResult.Data.Address2;
                validatedAddresses.ShippingAddressZip = shippingAddressResult.Data.Zip;

                if (shippingAddressResult.Data.IsValid) {
                    //shippingAddressValid = true;
                } else {
                    validatedAddresses.ShippingAddressIsValid = false;
                    //set shipping address to orginial data
                    processingResult.Data.Errors.Add("Shipping Address Validation");

                    foreach (var msg in shippingAddressResult.Data.ValidationRejections) {
                        processingResult.Data.Errors.Add("- " + msg);
                    }
                    processingResult.IsSuccessful = false;
                    processingResult.Data = validatedAddresses;
                    return processingResult;
                }
            }

            processingResult.Data = validatedAddresses;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<ValidatedAddresses>> ValidateAddressesAsync(SubmitOrderBindingModel request, int numSubmissions) {
            var validatedAddresses = new ValidatedAddresses();
            var processingResult = new ServiceProcessingResult<ValidatedAddresses> { IsSuccessful = true, Data = validatedAddresses };

            bool AddressVelocityBypass = false;
            var addressValidationService = new AddressValidationDataService();

            validatedAddresses.ServiceAddressStreet1 = request.ServiceAddressStreet1;
            validatedAddresses.ServiceAddressStreet2 = request.ServiceAddressStreet2;
            validatedAddresses.ServiceAddressCity = request.ServiceAddressCity;
            validatedAddresses.ServiceAddressState = request.ServiceAddressState;
            validatedAddresses.ServiceAddressZip = request.ServiceAddressZip;
            validatedAddresses.ServiceAddressIsValid = true;
            validatedAddresses.ValidatedAddressID = -1;
            validatedAddresses.ShippingAddressStreet1 = request.ShippingAddressStreet1;
            validatedAddresses.ShippingAddressStreet2 = request.ShippingAddressStreet2;
            validatedAddresses.ShippingAddressCity = request.ShippingAddressCity;
            validatedAddresses.ShippingAddressState = request.ShippingAddressState;
            validatedAddresses.ShippingAddressZip = request.ShippingAddressZip;
            validatedAddresses.ShippingAddressIsValid = true;
            validatedAddresses.AllowServiceAddressBypass = false;

            var addressToStandardize = new AddressStandardizeRequest {
                Address = request.ServiceAddressStreet1,
                Address2 = request.ServiceAddressStreet2,
                City = request.ServiceAddressCity,
                State = request.ServiceAddressState,
                Zip = request.ServiceAddressZip,
                ValidateLifeline = true
            };
            //if (api.ToString() == "Nlad") { addressToStandardize.DoDuplicateCheck = false; } //If we ever add a duplicate check, we should toggle it and unREM this code

            var serviceAddressResult = await addressValidationService.Standardize(addressToStandardize);
            if (!serviceAddressResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = serviceAddressResult.Error;
                return processingResult;
            }

            if (serviceAddressResult.Data.IsValid) {
                validatedAddresses.ServiceAddressCity = serviceAddressResult.Data.City;
                validatedAddresses.ServiceAddressState = serviceAddressResult.Data.State;
                validatedAddresses.ServiceAddressStreet1 = serviceAddressResult.Data.Address1;
                validatedAddresses.ServiceAddressStreet2 = serviceAddressResult.Data.Address2;
                validatedAddresses.ServiceAddressZip = serviceAddressResult.Data.Zip;
            } else {
                if (!request.HasServiceAddressBypass) {
                    validatedAddresses.ServiceAddressIsValid = false;
                    if (numSubmissions > Convert.ToInt32(ConfigurationManager.AppSettings["NumServiceAddressCorrections"])) {
                        AddressVelocityBypass = true;
                    } else {
                        validatedAddresses.Errors.Add("Service Address Rejection");
                        foreach (var msg in serviceAddressResult.Data.ValidationRejections) {
                            validatedAddresses.Errors.Add("- " + msg);
                        }
                        //IsSuccessful set to true to be inline with the way tpiv bypass works. Js checks for true and if has errors to open by pass html
                        processingResult.IsSuccessful = true;
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["AllowAddressBypasses"])) { validatedAddresses.AllowServiceAddressBypass = true; }
                        processingResult.Data = validatedAddresses;
                        return processingResult;
                    }
                }
            }

            //var shippingAddressValid = false;
            if ((request.ShippingAddressCity == request.ServiceAddressCity) && (request.ShippingAddressState == request.ServiceAddressState) && (request.ShippingAddressStreet1 == request.ServiceAddressStreet1) && (request.ShippingAddressStreet2 == request.ServiceAddressStreet2) && (request.ShippingAddressZip == request.ServiceAddressZip) && !AddressVelocityBypass) {
                //set shipping address to scrubbed service address
                validatedAddresses.ShippingAddressCity = validatedAddresses.ServiceAddressCity;
                validatedAddresses.ShippingAddressState = validatedAddresses.ServiceAddressState;
                validatedAddresses.ShippingAddressStreet1 = validatedAddresses.ServiceAddressStreet1;
                validatedAddresses.ShippingAddressStreet2 = validatedAddresses.ServiceAddressStreet2;
                validatedAddresses.ShippingAddressZip = validatedAddresses.ServiceAddressZip;
                //shippingAddressValid = true;
            } else {
                var shippingAddressResult = await addressValidationService.Standardize(new AddressStandardizeRequest {
                    City = request.ShippingAddressCity,
                    State = request.ShippingAddressState,
                    Address = request.ShippingAddressStreet1,
                    Address2 = request.ShippingAddressStreet2,
                    Zip = request.ShippingAddressZip,
                    ValidateLifeline = false
                });

                if (!shippingAddressResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = shippingAddressResult.Error;
                    return processingResult;
                }

                validatedAddresses.ShippingAddressCity = shippingAddressResult.Data.City;
                validatedAddresses.ShippingAddressState = shippingAddressResult.Data.State;
                validatedAddresses.ShippingAddressStreet1 = shippingAddressResult.Data.Address1;
                validatedAddresses.ShippingAddressStreet2 = shippingAddressResult.Data.Address2;
                validatedAddresses.ShippingAddressZip = shippingAddressResult.Data.Zip;

                if (shippingAddressResult.Data.IsValid) {
                    //shippingAddressValid = true;
                } else {
                    validatedAddresses.ServiceAddressIsValid = false;
                    //set shipping address to orginial data
                    processingResult.Data.Errors.Add("Shipping Address Validation");

                    foreach (var msg in shippingAddressResult.Data.ValidationRejections) {
                        processingResult.Data.Errors.Add("- " + msg);
                    }
                    processingResult.IsSuccessful = false;
                    processingResult.Data = validatedAddresses;
                    return processingResult;
                }
            }

            //if (shippingAddressValid) {
            //    if ((request.BillingAddressCity == request.ServiceAddressCity) && (request.BillingAddressState == request.ServiceAddressState) && (request.BillingAddressStreet1 == request.ServiceAddressStreet1) && (request.BillingAddressStreet2 == request.ServiceAddressStreet2) && (request.BillingAddressZip == request.ServiceAddressZip)) {
            //        // No need to scrub billing address since it is the same as service address. Set billing address same as service address
            //        validatedAddresses.BillingAddressCity = validatedAddresses.ServiceAddressCity;
            //        validatedAddresses.BillingAddressState = validatedAddresses.ServiceAddressState;
            //        validatedAddresses.BillingAddressStreet1 = validatedAddresses.ServiceAddressStreet1;
            //        validatedAddresses.BillingAddressStreet2 = validatedAddresses.ServiceAddressStreet2;
            //        validatedAddresses.BillingAddressZip = validatedAddresses.ServiceAddressZip;
            //    } else if ((request.ShippingAddressCity == request.BillingAddressCity) && (request.ShippingAddressState == request.BillingAddressState) && (request.ShippingAddressStreet1 == request.BillingAddressStreet1) && (request.ShippingAddressStreet2 == request.BillingAddressStreet2) && (request.ShippingAddressZip == request.BillingAddressZip)) {
            //        //set billing address to scrubbed shipping address
            //        validatedAddresses.BillingAddressCity = validatedAddresses.ShippingAddressCity;
            //        validatedAddresses.BillingAddressState = validatedAddresses.ShippingAddressState;
            //        validatedAddresses.BillingAddressStreet1 = validatedAddresses.ShippingAddressStreet1;
            //        validatedAddresses.BillingAddressStreet2 = validatedAddresses.ShippingAddressStreet2;
            //        validatedAddresses.BillingAddressZip = validatedAddresses.ShippingAddressZip;

            //        shippingAddressValid = true;
            //    } else {
            //        var billingAddressResult = await companyProviderValidation.ScrubAddress(new ValidatedAddress {
            //            City = request.BillingAddressCity,
            //            State = request.BillingAddressState,
            //            Street1 = request.BillingAddressStreet1,
            //            Street2 = request.BillingAddressStreet2,
            //            Zip = request.BillingAddressZip,
            //            BypassRestraints = true
            //        });

            //        if (billingAddressResult.IsValid) {
            //            //set billing address to scrubbed address
            //            validatedAddresses.BillingAddressCity = billingAddressResult.Data.City;
            //            validatedAddresses.BillingAddressState = billingAddressResult.Data.State;
            //            validatedAddresses.BillingAddressStreet1 = billingAddressResult.Data.Street1;
            //            validatedAddresses.BillingAddressStreet2 = billingAddressResult.Data.Street2;
            //            validatedAddresses.BillingAddressZip = billingAddressResult.Data.Zip;
            //            validatedAddresses.ValidatedAddressID = -1;
            //        } else {
            //            //    //set billing address to orginial data
            //            //    processingResult.Data.Errors.Add("CAMs Message - Billing Address");

            //            //    foreach (var msg in billingAddressResult.Errors) {
            //            //        processingResult.Data.Errors.Add("- " + msg);
            //            //    }
            //            //    validatedAddresses.AllowServiceAddressBypass = false; //this fails we are done so be sure by pass is false
            //            //    processingResult.IsSuccessful = false;
            //            //    processingResult.Data = validatedAddresses;
            //            //    return processingResult;
            //            validatedAddresses.BillingAddressCity = request.BillingAddressCity;
            //            validatedAddresses.BillingAddressState = request.BillingAddressState;
            //            validatedAddresses.BillingAddressStreet1 = request.BillingAddressStreet1;
            //            validatedAddresses.BillingAddressStreet2 = request.BillingAddressStreet2;
            //            validatedAddresses.BillingAddressZip = request.BillingAddressZip;
            //            validatedAddresses.ValidatedAddressID = -1;
            //        }

            //    }
            //}

            //1) Check Lifeline DB
            //  - If NLAD, scrub Service Address
            //  - If Non - NLAD, validate Service Address
            //2) Check for address scrub/ validation error
            //  - If good, save scrubbed Service address and ValidatedAddressID(if non - NLAD set to API result, else set to -1) to result
            //  -If bad, copy errors into result set
            //3) Check to see if billing exactly same as service (the one that was passed in, not the scrubbed version) 
            //  -If same, set result to scrubbed version of Service
            // - If different, scrub Billing
            //4) Check for address scrub/ validation error
            //  - If good, save scrubbed Billing address to result
            //  - If bad, copy errors into result set
            //5)
            //                Check to see if shipping exactly same as service or billing (the ones that were passed in, not the scrubbed versions) 
            //  -If same, set result to scrubbed version of matching address
            //  -If different, scrub
            //6) Check for address scrub/ validation error
            //  - If good, save scrubbed address to result
            //  - If bad, copy errors into result set
            //7)
            //  Use the scrubbed versions from that point on(update the temp order in DB ?)

            processingResult.Data = validatedAddresses;

            return processingResult;
        }
    }
}

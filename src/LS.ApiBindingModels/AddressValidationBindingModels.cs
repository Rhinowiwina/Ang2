using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CGM;
using System;
using LS.Core;

namespace LS.ApiBindingModels {
    public class BlackListedAddressResponse {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class AddressStandardizeRequest {
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public Boolean ValidateLifeline { get; set; }
    }

    public class AddressStandardizeResponse {
        public bool IsValid { get; set; } = false;
        public bool IsBypassable { get; set; } = true;
        public List<string> ValidationRejections { get; set; } = new List<string>();
        public string Address1 { get; set; }    //Required Tag/Optional Value : Address Line 1 is used to provide an apartment or suite number, if applicable.  Maximum characters allowed: 38. For example: <Address1></Address1>
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string DPV_Code { get; set; }
        public string DPV_Desc { get; set; }
    }

    public class AddressValidationRequest {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
    }

    public class AddressValidationResponse {
        public bool IsValid { get; set; } = false;
        public int ValidatedAddressID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public AddressValidationResponse() {
            Errors = new List<string>();
        }

        public List<string> Errors { get; set; }
    }
}
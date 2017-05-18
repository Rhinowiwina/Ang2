using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CGM;
using System;

namespace LS.ApiBindingModels
{
    public class ServerEnvironmentBindingModel {
        public string Environment { get; set; }
        public bool IsDev { get; set; } = false;
        public bool IsStaging { get; set; } = false;
        public bool IsProd { get; set; } = false;
        public bool IsDeveloperMachine { get; set; } = false;
        public string JSExceptionlessKey { get; set; }
        public string Version { get; set; }
        public ApplicationUser LoggedInUser{get;set;}
    }
}
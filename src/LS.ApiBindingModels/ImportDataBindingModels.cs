using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CGM;
using System;
using LS.Core;

namespace LS.ApiBindingModels {
    public class TrueUpRequest {
        public string EnrollmentNumbers { get; set; }
    }
}
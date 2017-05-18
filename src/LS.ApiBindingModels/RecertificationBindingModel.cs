using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LS.ApiBindingModels
{
    public class RecertificationInsertDetailsBindingModel {
        public string BudgetMobileID { get; set; }
        public string Beneficiary { get; set; }
        public string Signature { get; set; }
        public string SignatureType { get; set; }//Tablet,Topaz,ePad
        public string SigFileName { get; set; }//name of file stored on amazon
        public string SalesTeamID { get; set; }
    }
}


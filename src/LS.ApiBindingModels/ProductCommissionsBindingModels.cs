using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LS.ApiBindingModels
{
    public class ProductCommissionsCreateBindingModel {
        public string Id { get; set; }
        [Required(ErrorMessage = "Product Type is missing")]
        public string ProductType { get; set; }
        public decimal? Amount { get; set; }
        [Required(ErrorMessage = "Sales Team Id is missing.")]
        public string SalesTeamID { get; set; }
        [Required(ErrorMessage = "Recipient Type is missing")]
        public string RecipientType { get; set; }
        [Required(ErrorMessage = "Recipient  is missing")]
        public string RecipientUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ProductCommissionsUpdateBindingModel {
        public string Id { get; set; }
        [Required(ErrorMessage = "Product Type is missing")]
        public string ProductType { get; set; }
        public decimal? Amount { get; set; }
        [Required(ErrorMessage = "Sales Team Id is missing.")]
        public string SalesTeamID { get; set; }
        [Required(ErrorMessage = "Recipient Type is missing")]
        public string RecipientType { get; set; }
        [Required(ErrorMessage = "Recipient  is missing")]
        public string RecipientUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}


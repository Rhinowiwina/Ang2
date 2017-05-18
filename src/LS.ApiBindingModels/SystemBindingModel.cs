using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LS.ApiBindingModels
{
    public class SystemUpdateBindingModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Please enter a system name.")]
        public string System { get; set; }
        [Required(ErrorMessage = "Please select a status.")]
        public string Status { get; set; }
        public string Details { get; set; }
        public string Eta { get; set; }
        public bool ExternalSystem { get; set; }
        public string SiteStatusColName { get; set; }
        public int SortOrder { get; set; }
        public bool Display { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class SystemCreateBindingModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Please enter a system name.")]
        public string System { get; set; }
        [Required(ErrorMessage = "Please select a status.")]
        public string Status { get; set; }
        public string Details { get; set; }
        public string Eta { get; set; }
        public bool ExternalSystem { get; set; }
        public string SiteStatusColName { get; set; }
        public int SortOrder { get; set; }
        public bool Display { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }

}


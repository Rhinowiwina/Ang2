using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace LS.ApiBindingModels {
    public class ExternalStorageListDirectoryBindingModel {
        public string ETag { get; set; }
        public string Key { get; set; }
        public DateTime LastModified { get; set; }
        public string Owner { get; set; }
        public string Size { get; set; }
        public string StorageClass { get; set; }       
    }
}


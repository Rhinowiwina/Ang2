using System.IO;
using Amazon.S3.Model;
using System.Collections.Generic;
using System;

namespace LS.Core.Interfaces
{
    public class IExternalStorageListDirectory
    {
        public string ETag { get; set; }
        public string Key { get; set; }
        public DateTime LastModified { get; set; }
        public string Owner { get; set; }
        public string Size { get; set; }
        public string StorageClass { get; set; }
    }
}
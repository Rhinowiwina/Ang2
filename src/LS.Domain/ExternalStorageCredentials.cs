using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ExternalStorageCredentials : IEntity<string>
    {
        public ExternalStorageCredentials()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Type { get; set; }
        public string System { get; set; }
        public string Path { get; set; }
        public int MaxImageSize { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
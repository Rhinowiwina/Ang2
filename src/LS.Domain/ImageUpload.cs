using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ImageUpload : IEntity<string>
    {
        public ImageUpload()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string ImageCode { get; set; }
        public int MaxImageSize { get; set; }

        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public Company Company { get; set; }
        public string CompanyId { get; set; }
        public ExternalStorageCredentials StorageCredentials { get; set; }
        public string StorageCredentialsId { get; set; }
       
        public string UploadType { get; set; }
        public string DeviceDetails { get; set; }
        public bool HasBeenUploaded { get; set; }
        public DateTime? DateUploaded { get; set; } 

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
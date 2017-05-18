using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class ImageUploadEFConfig : EntityTypeConfiguration<ImageUpload>
    {
        public ImageUploadEFConfig()
        {
            Property(iu => iu.ImageCode)
                .IsRequired()
                .HasMaxLength(20);
            Property(iu => iu.HasBeenUploaded)
                .IsRequired();

            Property(iu => iu.UserId)
                .IsRequired();

            Property(iu => iu.UploadType)
                .HasMaxLength(20);

            HasRequired(iu => iu.Company)
                .WithMany()
                .WillCascadeOnDelete(false);

            HasRequired(iu => iu.StorageCredentials)
                .WithMany()
                .WillCascadeOnDelete(false);

        }
    }
}
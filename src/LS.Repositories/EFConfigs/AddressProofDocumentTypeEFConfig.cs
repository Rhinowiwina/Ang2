using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class AddressProofDocumentTypeEFConfig : EntityTypeConfiguration<AddressProofDocumentType>
    {
        public AddressProofDocumentTypeEFConfig()
        {
            Property(t => t.Type)
                 .HasMaxLength(5);

            Property(t => t.Name)
                .HasMaxLength(125);

            Property(t => t.LifelineSystem)
                .HasMaxLength(10);

            Property(t => t.LifelineSystemId)
                .HasMaxLength(10);
            }
    }
}

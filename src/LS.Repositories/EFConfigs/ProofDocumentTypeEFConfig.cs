using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class ProofDocumentTypeEFConfig : EntityTypeConfiguration<ProofDocumentType>
    {
        public ProofDocumentTypeEFConfig()
        {
            Property(p => p.ProofType)
                .HasColumnType("VARCHAR")
                .HasMaxLength(20);

            Property(p => p.Name)
                .HasColumnType("VARCHAR")
                .HasMaxLength(175);

            Property(p => p.StateCode)
                .HasColumnType("VARCHAR")
                .HasMaxLength(2);
        }
    }
}

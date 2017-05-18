using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class TpivProofDocumentTypeEFConfig : EntityTypeConfiguration<TpivProofDocumentType>
    {
        public TpivProofDocumentTypeEFConfig()
        {
            Property(t => t.Type)
                .HasMaxLength(5);

            Property(t => t.Name)
                .HasMaxLength(100);

            Property(t => t.LifelineSystem)
                .HasMaxLength(10);

            Property(t => t.LifelineSystemId)
                .HasMaxLength(10);
        }
    }
}

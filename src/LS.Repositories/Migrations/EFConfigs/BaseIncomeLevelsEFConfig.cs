using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class BaseIncomeLevelsEFConfig : EntityTypeConfiguration<BaseIncomeLevels>
    {
        public BaseIncomeLevelsEFConfig()
        {
            Property(b => b.StateCode)
                .HasMaxLength(2);
        }
    }
}

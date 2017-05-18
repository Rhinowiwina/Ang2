using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class RedemptionCodeEFConfig : EntityTypeConfiguration<RedemptionCode>
    {
        public RedemptionCodeEFConfig()
        {
            Property(x => x.Code).IsRequired().HasMaxLength(20);
            Property(x => x.CompanyId).IsRequired();
        }
    }
}

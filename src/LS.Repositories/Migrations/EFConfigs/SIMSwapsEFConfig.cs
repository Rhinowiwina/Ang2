using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class SIMSwapsEFConfig : EntityTypeConfiguration<SIMSwaps>
    {
        public SIMSwapsEFConfig()
        {
            Property(c => c.SalesTeamID).HasMaxLength(128);
            Property(c => c.UserID).HasMaxLength(128);
            Property(c => c.TenantAccountID).HasMaxLength(128);
            Property(c => c.OldIMSI).HasMaxLength(128);
            Property(c => c.NewIMSI).HasMaxLength(128);
            Property(c => c.CompanyID).HasMaxLength(128);
            Property(c => c.Notes);
            Property(c => c.Reason).HasMaxLength(128);
        }
    }
}

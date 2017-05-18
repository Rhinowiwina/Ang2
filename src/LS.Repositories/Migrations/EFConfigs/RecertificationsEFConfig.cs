using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class RecertificationsEFConfig : EntityTypeConfiguration<Recertifications>
    {
        public RecertificationsEFConfig()
        {
            Property(r => r.BudgetMobileID).HasColumnType("VARCHAR").HasMaxLength(128);
            Property(r => r.SalesTeamID).HasColumnType("VARCHAR").HasMaxLength(128);
            Property(r => r.UserID).HasColumnType("VARCHAR").HasMaxLength(128);
            Property(r => r.NewExpDate).HasColumnType("DATETIME");
        }
    }
}
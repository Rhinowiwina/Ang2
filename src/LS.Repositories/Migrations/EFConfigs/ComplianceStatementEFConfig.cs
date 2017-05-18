using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class ComplianceStatementEFConfig : EntityTypeConfiguration<ComplianceStatement>
    {
        public ComplianceStatementEFConfig()
        {
            HasRequired(s => s.Company);

            Property(s => s.StateCode)
                .HasMaxLength(2);

            Property(s => s.Statement);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class StateAgreementEFConfig : EntityTypeConfiguration<StateAgreement>
    {
        public StateAgreementEFConfig()
        {
            Property(a => a.StateCode)
                .HasColumnType("VARCHAR")
                .HasMaxLength(2);

            Property(a => a.Agreement);
        }
    }
}

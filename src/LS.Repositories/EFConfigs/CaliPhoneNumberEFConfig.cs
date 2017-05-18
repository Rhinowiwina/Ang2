using System.Data.Entity.ModelConfiguration;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;

namespace LS.Repositories.EFConfigs
{
    class CaliPhoneNumberEFConfig : EntityTypeConfiguration<CaliPhoneNumber>
    {
        public CaliPhoneNumberEFConfig()
        {
            HasRequired(c => c.Company);
        }
    }
}

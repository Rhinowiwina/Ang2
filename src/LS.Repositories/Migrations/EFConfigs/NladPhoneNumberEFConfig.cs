using System.Data.Entity.ModelConfiguration;
using LS.Domain.ExternalApiIntegration.Nlad;

namespace LS.Repositories.EFConfigs
{
    class NladPhoneNumberEFConfig : EntityTypeConfiguration<NladPhoneNumber>
    {
        public NladPhoneNumberEFConfig()
        {
            HasRequired(c => c.Company);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class CompanyEFConfig : EntityTypeConfiguration<Company>
    {
        public CompanyEFConfig()
        {
            Property(c => c.Name).HasMaxLength(100);
            Property(c => c.EmailRequiredForOrder);
            Property(c => c.ContactPhoneRequiredForOrder);
            Property(c => c.TimeZone).HasMaxLength(100);
            Property(c => c.OrderStart).HasMaxLength(100);
            Property(c => c.OrderEnd).HasMaxLength(100);
        }
    }
}

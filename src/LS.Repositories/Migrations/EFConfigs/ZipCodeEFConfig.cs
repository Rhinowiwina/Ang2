using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class ZipCodeEFConfig : EntityTypeConfiguration<ZipCode>
    {
        public ZipCodeEFConfig()
        {
            Property(z => z.PostalCode).HasMaxLength(10);
            Property(z => z.State).HasMaxLength(50);
            Property(z => z.StateAbbreviation).HasMaxLength(2);
            Property(z => z.CountyFips).HasMaxLength(10);
            Property(z => z.City).HasMaxLength(50);
        }
    }
}

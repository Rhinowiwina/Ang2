using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class AddressValidationEFConfig : EntityTypeConfiguration<AddressValidation>
    {
        public AddressValidationEFConfig()
        {
            Property(c => c.Id).HasMaxLength(128);
            Property(c => c.Street1).HasMaxLength(128);
            Property(c => c.Street2).HasMaxLength(128);
            Property(c => c.City).HasMaxLength(100);
            Property(c => c.State).HasMaxLength(50);
            Property(c => c.Zipcode).HasMaxLength(12);
            Property(c => c.IsShelter);
            Property(c => c.DateCreated);
            Property(c => c.DateModified);
            Property(c => c.ModifiedByUserID).HasMaxLength(128);
        }
    }
}

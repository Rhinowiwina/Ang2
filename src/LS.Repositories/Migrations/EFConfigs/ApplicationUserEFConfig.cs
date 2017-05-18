using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class ApplicationUserEFConfig : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserEFConfig()
        {
            HasRequired(u => u.Company);

            Property(u => u.UserName)
                .HasMaxLength(250);

            Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(75);

            Property(u => u.LastName)
                .HasMaxLength(75);

            Property(u => u.Language).IsRequired().HasMaxLength(4);

            Property(u => u.PayPalEmail).HasMaxLength(100);
            Property(u => u.ModifiedByUserId).HasMaxLength(128);
            Property(u => u.PhoneNumber).HasMaxLength(20);


            Property(u => u.PermissionsAccountOrder).IsRequired();
        }
    }
}

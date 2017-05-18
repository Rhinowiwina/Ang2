using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class SalesTeamEFConfig : EntityTypeConfiguration<SalesTeam>
    {
        public SalesTeamEFConfig()
        {
            HasRequired(st => st.Company);

            HasMany(st => st.Users)
                .WithOptional(au => au.SalesTeam);

            Property(st => st.Name)
                .HasMaxLength(250);

            Property(st => st.ExternalPrimaryId)
                .HasMaxLength(50);

            Property(st => st.ExternalDisplayName)
                .HasMaxLength(50);

            Property(st => st.Address1)
                .HasMaxLength(100);

            Property(st => st.Address2)
                .HasMaxLength(100);

            Property(st => st.City)
                .HasMaxLength(50);

            Property(st => st.State)
                .HasMaxLength(50);

            Property(st => st.Zip)
                .HasMaxLength(10);

            Property(st => st.Phone)
                .HasMaxLength(100);

            Property(st => st.CycleCountTypeDevice)
                .HasMaxLength(10);

            Property(st => st.CycleCountTypeSim)
                .HasMaxLength(10);
        }
    }
}

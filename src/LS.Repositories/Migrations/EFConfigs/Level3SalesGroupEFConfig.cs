using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class Level3SalesGroupEFConfig : EntityTypeConfiguration<Level3SalesGroup>
    {
        public Level3SalesGroupEFConfig()
        {
            HasMany(s => s.Managers)
                .WithMany();

            HasRequired(s => s.Company)
                .WithMany()
                .WillCascadeOnDelete(false);

            HasRequired(s => s.CreatedByUser)
                .WithMany()
                .WillCascadeOnDelete(false);

            HasRequired(s => s.ParentSalesGroup);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class Level2SalesGroupEFConfig : EntityTypeConfiguration<Level2SalesGroup>
    {
        public Level2SalesGroupEFConfig()
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

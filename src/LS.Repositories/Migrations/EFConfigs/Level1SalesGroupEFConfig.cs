using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class Level1SalesGroupEFConfig :EntityTypeConfiguration<Level1SalesGroup>
    {
        public Level1SalesGroupEFConfig()
        {
            HasMany(s => s.Managers)
                .WithMany();

            HasRequired(s => s.Company)
                .WithMany()
                .WillCascadeOnDelete(false);

            HasRequired(s => s.CreatedByUser)
                .WithMany()
                .WillCascadeOnDelete(false);        }
    }
}

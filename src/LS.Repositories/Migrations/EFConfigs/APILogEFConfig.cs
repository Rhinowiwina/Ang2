using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class ApiLogEFConfig : EntityTypeConfiguration<ApiLogEntry>
    {
        public ApiLogEFConfig()
        {
            Property(a => a.Api)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            Property(a => a.Function)
                .HasColumnType("VARCHAR")
                .HasMaxLength(250);

            Property(a => a.Input)
                .HasColumnType("TEXT");

            Property(a => a.Response)
                .HasColumnType("TEXT");
        }
    }
}

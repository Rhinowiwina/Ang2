using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    internal class WebApplicationLogEntryEFConfig : EntityTypeConfiguration<WebApplicationLogEntry>
    {
        public WebApplicationLogEntryEFConfig()
        {
            ToTable("WebApplicationLog");
            HasKey(le => le.Id)
                .Property(le => le.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}

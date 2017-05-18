using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class ExternalStorageCredentialsEFConfig : EntityTypeConfiguration<ExternalStorageCredentials>
    {
        public ExternalStorageCredentialsEFConfig()
        {
            Property(sc => sc.AccessKey)
                .IsRequired();

            Property(sc => sc.SecretKey)
                .IsRequired();

            Property(sc => sc.Type)
                .IsRequired();

            Property(sc => sc.System)
                .IsRequired();

            Property(sc => sc.CompanyId)
                .IsRequired();
        }
    }
}
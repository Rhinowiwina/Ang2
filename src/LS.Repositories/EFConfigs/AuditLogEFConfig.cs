using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class AuditLogEFConfig : EntityTypeConfiguration<AuditLog>
    {
        public AuditLogEFConfig() {
            Property(c => c.Id).HasMaxLength(100);
            Property(c => c.ModifiedByUserID).HasMaxLength(100);
            Property(c => c.DateCreated);
            Property(c => c.TableName).HasMaxLength(100);
            Property(c => c.TableRowID).HasMaxLength(100);
            Property(c => c.TablePreviousData);
            Property(c => c.Reason).HasMaxLength(300);
        }
    }
}

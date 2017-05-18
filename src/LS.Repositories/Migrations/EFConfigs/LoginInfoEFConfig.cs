using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class LoginInfoEFConfig : EntityTypeConfiguration<LoginInfo>
    {
        public LoginInfoEFConfig()
        {
            ToTable("LoginInfo");
            Property(li => li.UserId).HasMaxLength(128).IsRequired();
            Property(li => li.SessionId).HasMaxLength(128).IsRequired();
        }
    }
}

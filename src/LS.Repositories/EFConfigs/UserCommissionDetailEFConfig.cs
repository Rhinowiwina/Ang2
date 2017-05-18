using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class UserCommissionDetailEFConfig : EntityTypeConfiguration<UserCommissionDetail>
    {
        public UserCommissionDetailEFConfig()
        {
            HasRequired(u => u.Order).WithMany().HasForeignKey(u => u.OrderId);
            HasOptional(u => u.User).WithMany().HasForeignKey(u => u.UserId);
            HasRequired(u => u.SalesTeam).WithMany().HasForeignKey(u => u.SalesTeamId);
            HasOptional(u => u.Payment).WithMany().HasForeignKey(u => u.PaymentID);
        }
    }
}

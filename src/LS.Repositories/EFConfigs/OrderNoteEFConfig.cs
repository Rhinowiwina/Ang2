using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    class OrderNoteEFConfig : EntityTypeConfiguration<OrderNote>
    {
        public OrderNoteEFConfig()
        {
            Property(c => c.OrderID).HasMaxLength(50);
            Property(c => c.Note);
            Property(c => c.AddedBy).HasMaxLength(128);
        }
    }
}

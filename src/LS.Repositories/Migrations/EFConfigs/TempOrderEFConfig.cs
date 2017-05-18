using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class TempOrderEFConfig : EntityTypeConfiguration<TempOrder>
    {
        public TempOrderEFConfig()
        {
            Property(o => o.QBFirstName)
               .HasMaxLength(50);

            Property(o => o.MiddleInitial)
                .HasMaxLength(1);

            Property(o => o.QBLastName)
                .HasMaxLength(50);

            Property(o => o.IPImageFileName)
                .HasMaxLength(100);

            Property(o => o.CurrentLifelinePhoneNumber)
                .HasMaxLength(20);

            Property(o => o.LPProofNumber)
                .HasMaxLength(50);

            Property(o => o.LPImageFileName)
                .HasMaxLength(100);

            Property(o => o.FirstName)
                .HasMaxLength(50);

            Property(o => o.LastName)
                .HasMaxLength(50);

            Property(o => o.EmailAddress)
                .HasMaxLength(50);

            HasMany(o => o.AgreementStatements);

            Property(o => o.ServiceAddressStreet1)
                .HasMaxLength(100);

            Property(o => o.ServiceAddressStreet2)
                .HasMaxLength(100);

            Property(o => o.ServiceAddressCity)
                .HasMaxLength(50);

            Property(o => o.ServiceAddressState)
                .HasMaxLength(2);

            Property(o => o.ServiceAddressZip)
                .HasMaxLength(10);

            Property(o => o.BillingAddressStreet1)
                .HasMaxLength(100);

            Property(o => o.BillingAddressStreet2)
                .HasMaxLength(100);

            Property(o => o.BillingAddressCity)
                .HasMaxLength(50);

            Property(o => o.BillingAddressState)
                .HasMaxLength(2);

            Property(o => o.BillingAddressZip)
                .HasMaxLength(10);

            Property(o => o.ShippingAddressStreet1)
                .HasMaxLength(100);

            Property(o => o.ShippingAddressStreet2)
                .HasMaxLength(100);

            Property(o => o.ShippingAddressCity)
                .HasMaxLength(50);

            Property(o => o.ShippingAddressState)
                .HasMaxLength(2);

            Property(o => o.ServiceAddressZip)
                .HasMaxLength(10);

            Property(o => o.HohAdultsOtherText)
                .HasMaxLength(50);

            Property(o => o.HohShareLifelineNames)
                .HasMaxLength(500);

            Property(o => o.DeviceIdentifier)
                .HasMaxLength(50);

            Property(o => o.SimIdentifier)
                .HasMaxLength(50);

            Property(o => o.TpivBypassSignature)
                .HasMaxLength(100);

            Property(o => o.TpivBypassSsnCardLastFour)
                .HasMaxLength(4);

            Property(o => o.TpivBypassDobCardLastFour)
                .HasMaxLength(4);

            Property(o => o.PaymentType)
                .HasMaxLength(10);

            Property(o => o.CreditCardReference)
                .HasMaxLength(100);

            Property(o => o.CreditCardTransactionId)
                .HasMaxLength(100);

            Property(o => o.LifelineEnrollmentId)
                .HasMaxLength(50);

            Property(o => o.LifelineEnrollmentType)
                .HasMaxLength(50);

            Property(o => o.AIInitials)
                .HasMaxLength(5);

            Property(o => o.AIFrequency)
                .HasMaxLength(20);

            Property(o => o.UserId)
               .HasMaxLength(300);

            Property(o => o.FulfillmentType).HasMaxLength(20);
        }
    }
}

using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class OrderEFConfig : EntityTypeConfiguration<Order>
    {
        public OrderEFConfig()
        {
            Property(o => o.QBFirstName)
                .HasMaxLength(50);

            Property(o => o.MiddleInitial)
                .HasMaxLength(1);

            Property(o => o.QBLastName)
                .HasMaxLength(50);

            Property(o => o.CurrentLifelinePhoneNumber)
                .HasMaxLength(20);

            Property(o => o.LPProofNumber)
                .HasMaxLength(50);

            Property(o => o.FirstName)
                .HasMaxLength(50);

            Property(o => o.LastName)
                .HasMaxLength(50);

            Property(o => o.EmailAddress)
                .HasMaxLength(50);

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

            HasMany(o => o.AgreementStatements);

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

            Property(o => o.TPIVBypassSSNProofNumber)
                .HasMaxLength(4);

            Property(o => o.TpivBypassDobProofNumber)
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


            Property(o => o.ParentOrderId)
                .HasMaxLength(128);


            Property(o => o.Gender)
                .HasMaxLength(20);


            Property(o => o.TransactionId)
                .HasMaxLength(128);

            Property(o => o.ExternalVelocityCheck)
                .HasMaxLength(10);


            Property(o => o.SigFileName).HasMaxLength(500);

            Property(o => o.SignatureType).HasMaxLength(20);
            Property(o => o.TpivCode).HasMaxLength(50);
            Property(o => o.TpivBypassDobProofTypeId).HasMaxLength(128);
            Property(o => o.TpivBypassDobProofTypeId).HasMaxLength(128);
            Property(o => o.DeviceId).HasMaxLength(50);
            Property(o => o.CarrierId).HasMaxLength(128);
            Property(o => o.Signature);
            Property(o => o.ShippingAddressZip).HasMaxLength(10);
            Property(o => o.ContactPhoneNumber).HasMaxLength(20);
            Property(o => o.IDProofTypeID).HasMaxLength(128);
            Property(o => o.SecondaryStateProgramNumber).HasMaxLength(50);
            Property(o => o.SecondaryStateProgramId).HasMaxLength(128);
            Property(o => o.StateProgramNumber).HasMaxLength(50);
            Property(o => o.StateProgramId).HasMaxLength(128);
            Property(o => o.LPProofTypeId).HasMaxLength(128);
            Property(o => o.LifelineProgramId).HasMaxLength(128);

            Property(o => o.AINumHousehold);

            Property(o => o.AIAvgIncome);
            Property(o => o.FulfillmentType).HasMaxLength(20);
            Property(o => o.DeviceModel).HasMaxLength(50);
            Property(o => o.TpivNasScore).HasMaxLength(200);
            Property(o => o.TpivRiskIndicators).HasMaxLength(300);
            Property(o => o.TpivTransactionID).HasMaxLength(128);
            Property(o => o.StatusID).HasMaxLength(50);

            Property(o => o.RTR_Name).HasMaxLength(100);
            Property(o => o.RTR_Date);
            Property(o => o.RTR_Notes).HasMaxLength(300);
            Property(o => o.RTR_RejectCode).HasMaxLength(50);
            Property(o => o.OrderCode).HasMaxLength(50);

            Property(o => o.LPProofImageFilename).HasMaxLength(128);
            Property(o => o.LPProofImageID).HasMaxLength(100);

            Property(o => o.IDProofImageFilename).HasMaxLength(128);
            Property(o => o.IDProofImageID).HasMaxLength(100);

            Property(o => o.TPIVBypassSSNProofImageFilename).HasMaxLength(128);
            Property(o => o.TPIVBypassSSNProofImageID).HasMaxLength(128);

        }
    }
}

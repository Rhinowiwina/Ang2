using System;
using System.Collections.Generic;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public interface IOrder : IEntity<string>
    {
        string CompanyId { get; set; }
        Company Company { get; set; }

        string SalesTeamId { get; set; }
        SalesTeam SalesTeam { get; set; }

        // Lifeline Compliance
        bool HouseholdReceivesLifelineBenefits { get; set; }
        bool CustomerReceivesLifelineBenefits { get; set; }

        // Qualifying Beneficiary
        string QBFirstName { get; set; }
        string QBLastName { get; set; }
        byte[] QBSsn { get; set; }
        byte[] QBDateOfBirth { get; set; }

        // Lifeline Program & Proof
        string CurrentLifelinePhoneNumber { get; set; }
        string LifelineProgramId { get; set; }
        string LPProofTypeId { get; set; }
        string LPProofNumber { get; set; }
        string LPProofImageID { get; set; }
        string LPProofImageFilename { get; set; }

        // State Program Numbers
        string StateProgramId { get; set; }
        string StateProgramNumber { get; set; }
        string SecondaryStateProgramId { get; set; }
        string SecondaryStateProgramNumber { get; set; }

        // Customer Info
        string FirstName { get; set; }
        string MiddleInitial { get; set; }
        string LastName { get; set; }
        byte[] Ssn { get; set; }
        byte[] DateOfBirth { get; set; }
        string EmailAddress { get; set; }
        string ContactPhoneNumber { get; set; }

        // Identity Proof
        string IDProofTypeID { get; set; }
        string IDProofImageID { get; set; }
        string IDProofImageFilename { get; set; }

        // Service Address
        string ServiceAddressStreet1 { get; set; }
        string ServiceAddressStreet2 { get; set; }
        string ServiceAddressCity { get; set; }
        string ServiceAddressState { get; set; }
        string ServiceAddressZip { get; set; }
        bool ServiceAddressIsPermanent { get; set; }

        // Billing Address
        string BillingAddressStreet1 { get; set; }
        string BillingAddressStreet2 { get; set; }
        string BillingAddressCity { get; set; }
        string BillingAddressState { get; set; }
        string BillingAddressZip { get; set; }

        // Shipping Address
        string ShippingAddressStreet1 { get; set; }
        string ShippingAddressStreet2 { get; set; }
        string ShippingAddressCity { get; set; }
        string ShippingAddressState { get; set; }
        string ShippingAddressZip { get; set; }

        // Head Of Household
        bool HohSpouse { get; set; }
        bool HohAdultsParent { get; set; }
        bool HohAdultsChild { get; set; }
        bool HohAdultsRelative { get; set; }
        bool HohAdultsRoommate { get; set; }
        bool HohAdultsOther { get; set; }
        string HohAdultsOtherText { get; set; }
        bool? HohExpenses { get; set; }
        bool? HohShareLifeline { get; set; } // not sure where this is on the order form
        string HohShareLifelineNames { get; set; } // not sure where this is on the order form
        bool? HohAgreeMultiHouse { get; set; }
        bool HohAgreeViolation { get; set; }

        // Basically a collection of agreement statements and the customer's responses... need to figure out how this will be handled
        ICollection<StateAgreement> AgreementStatements { get; set; }

        // Signature is going to be a long string that is made up of comma-separated coordinates that make up the signature
        string Signature { get; set; }

        // Won't be used until we start implementing the approval phase...
        bool HasDevice { get; set; } // Will be false when order is being taken
        string CarrierId { get; set; } // Set to -1 by default for now
        string DeviceId { get; set; }
        string DeviceIdentifier { get; set; }
        string SimIdentifier { get; set; }
        string PlanId { get; set; } // Set to -1 by default
        Plan Plan { get; set; }


        // Location Coordinates
        float LatitudeCoordinate { get; set; }
        float LongitudeCoordinate { get; set; }

        // Payment Info
        string PaymentType { get; set; }
        string CreditCardReference { get; set; }
        bool CreditCardSuccess { get; set; }
        string CreditCardTransactionId { get; set; }

        // Enrollment Info
        string LifelineEnrollmentId { get; set; }
        string LifelineEnrollmentType { get; set; }

        // Annual Income Enrollment type values
        string AIInitials { get; set; }
        string AIFrequency { get; set; }
        int AIAvgIncome { get; set; }
        int AINumHousehold { get; set; }

        string UnencryptedDateOfBirth { get; set; }
        string UnencryptedSsn { get; set; }
        string FulfillmentType { get; set; }
    }
}

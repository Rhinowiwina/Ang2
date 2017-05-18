using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace LS.Domain
{
    public class TempOrder : IEntity<string>, IOrder
    {
        public TempOrder()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
       
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public string ParentOrderId { get; set; }//used if order is resubmitted from a rejected order
        public string SalesTeamId { get; set; }
        public SalesTeam SalesTeam { get; set; }

        // Lifeline Compliance
        public bool HouseholdReceivesLifelineBenefits { get; set; }
        public bool CustomerReceivesLifelineBenefits { get; set; }

        // Qualifying Beneficiary
        public string QBFirstName { get; set; }
        public string QBLastName { get; set; }
        public byte[] QBSsn { get; set; }
        public byte[] QBDateOfBirth { get; set; }
        [NotMapped]
        public string UnencryptedQBSsn { get; set; }
        [NotMapped]
        public string UnencryptedQBDateOfBirth { get; set; }

        // Lifeline Program & Proof
        public string CurrentLifelinePhoneNumber { get; set; } 
        public string LifelineProgramId { get; set; }
        public string LPProofTypeId { get; set; }
        public string LPProofNumber { get; set; }
        public string LPProofImageID { get; set; }
        public string LPProofImageFilename { get; set; }

        // State Program Numbers
        public string StateProgramId { get; set; }
        public string StateProgramNumber { get; set; }
        public string SecondaryStateProgramId { get; set; }
        public string SecondaryStateProgramNumber { get; set; }

        // Customer Info
        public string Language { get; set; }
       public string CommunicationPreference { get; set;}
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public byte[] Ssn { get; set; }
        public string Gender { get; set; }
        public byte[] DateOfBirth { get; set; }
        [NotMapped]
        public string UnencryptedSsn { get; set; }
        [NotMapped]
        public string UnencryptedDateOfBirth { get; set; }

        public string EmailAddress { get; set; }
        public string ContactPhoneNumber { get; set; }

        // Identity Proof
        public string IDProofTypeID { get; set; }
        public string IDProofImageID { get; set; }
        public string IDProofImageFilename { get; set; }
        public string IDProofImageID2 { get; set; }
        public string IDProofImageFilename2 { get; set; }

        // Service Address
        public bool ServiceAddressBypass { get; set; }
        public string ServiceAddressBypassSignature { get; set; }
        public string ServiceAddressBypassProofTypeID { get; set; }
    
        public string ServiceAddressBypassProofImageFilename { get; set; }
        public string ServiceAddressBypassProofImageID { get; set; }
        public string ServiceAddressStreet1 { get; set; }
        public string ServiceAddressStreet2 { get; set; }
        public string ServiceAddressCity { get; set; }
        public string ServiceAddressState { get; set; }
        public string ServiceAddressZip { get; set; }
        public bool ServiceAddressIsPermanent { get; set; }

        // Billing Address
        public string BillingAddressStreet1 { get; set; }
        public string BillingAddressStreet2 { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressZip { get; set; }

        // Shipping Address
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressZip { get; set; }

        // Head Of Household
        public bool HohSpouse { get; set; }
        public bool HohAdultsParent { get; set; }
        public bool HohAdultsChild { get; set; }
        public bool HohAdultsRelative { get; set; }
        public bool HohAdultsRoommate { get; set; }
        public bool HohAdultsOther { get; set; }
        public string HohAdultsOtherText { get; set; }
        public bool? HohExpenses { get; set; }
        public bool? HohShareLifeline { get; set; } // not sure where this is on the order form
        public string HohShareLifelineNames { get; set; } // not sure where this is on the order form
        public bool? HohAgreeMultiHouse { get; set; }
        public bool HohAgreeViolation { get; set; }
        public bool? HohPuertoRicoAgreeViolation { get; set; }

        // Basically a collection of agreement statements and the customer's responses... need to figure out how this will be handled
        public virtual ICollection<StateAgreement> AgreementStatements { get; set; }

        // Signature is going to be a long string that is made up of comma-separated coordinates that make up the signature
        public string Signature { get; set; }
        public string Initials { get; set; }
        public string SignatureType { get; set; }

        // Won't be used until we start implementing the approval phase...
        public bool HasDevice { get; set; } // Will be false when order is being taken
        public string DeviceCompatibility { get; set; }
        public string CarrierId { get; set; } // Set to -1 by default for now
        public string ByopCarrier { get; set; }
        public string DeviceId { get; set; }//0 is free,1 is customer provided. Anything else is the device
        public string DeviceModel { get; set; }
        public string DeviceIdentifier { get; set; }
  
        public string SimIdentifier { get; set; }
        public string PlanId { get; set; } // Set to -1 by default
        public Plan Plan { get; set; }

        // TPIV Bypass
        public bool TpivBypass { get; set; }
        public string TpivBypassSignature { get; set; }
        public string TpivBypassSsnProofTypeId { get; set; }
        public string TpivBypassSsnProofImageFilename { get; set; }
        
        public string TpivBypassSsnProofImageID { get; set; }
        public string TpivBypassSsnProofNumber { get; set; }
        public string TpivBypassDobProofTypeID { get; set; }
        public string TpivBypassDobProofNumber { get; set; }
        public string TpivCode { get; set; }
        public string TpivRiskIndicators { get; set; }
        public string TpivTransactionID { get; set; }
        public string TpivNasScore { get; set; }

        // Location Coordinates
        public float LatitudeCoordinate { get; set; }
        public float LongitudeCoordinate { get; set; }

        // Payment Info
        public string PaymentType { get; set; }
        public string CreditCardReference { get; set; }
        public bool CreditCardSuccess { get; set; }
        public string CreditCardTransactionId { get; set; }

        // Enrollment Info
        public string LifelineEnrollmentId { get; set; }
        public string LifelineEnrollmentType { get; set; }

        // Annual Income Enrollment type values
        public string AIInitials { get; set; }
        public string AIFrequency { get; set; }
        public int AIAvgIncome { get; set; }
        public int AINumHousehold { get; set; }
        public int AINumHouseAdult { get; set; }
        public int AINumHouseChildren { get; set; }

        public string FulfillmentType { get; set; }
        public string TransactionId { get; set; }
        // IEntity stuff
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}

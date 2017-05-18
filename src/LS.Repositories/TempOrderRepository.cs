using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Repositories
{
    public class TempOrderRepository : BaseRepository<TempOrder, string>
    {
        private static readonly string InsertStoredProcName = "dbo.Temp_Order_Insert";
        private static readonly string Passphrase = "Test3r123!";

        public TempOrderRepository()
            : base(new AmbientDbContextLocator())
        {
        }

        public override DataAccessResult<TempOrder> Add(TempOrder entityToAdd, EntityState childrenEntityState = EntityState.Unchanged)
        {
            var result = new DataAccessResult<TempOrder>();

            try
            {
                var idParam = new SqlParameter("@Id", entityToAdd.Id);
                var userIdParam = new SqlParameter("@UserId", entityToAdd.UserId);
                var companyIdParam = new SqlParameter("@CompanyId", entityToAdd.CompanyId);
                var salesTeamIdParam = new SqlParameter("@SalesTeamId", entityToAdd.SalesTeamId);
                var houseHoldReceivesLifelineBenefitsParam = new SqlParameter("@HouseholdReceivesLifelineBenefits", entityToAdd.HouseholdReceivesLifelineBenefits);
                var customerReceivesLifelineBenefitsParam = new SqlParameter("@CustomerReceivesLifelineBenefits", entityToAdd.CustomerReceivesLifelineBenefits);
                var qbFirstNameParam = new SqlParameter("@QBFirstName", entityToAdd.QBFirstName ?? (object)DBNull.Value);
                var qbLastNameParam = new SqlParameter("@QBLastName", entityToAdd.QBLastName ?? (object)DBNull.Value);
                var currentLifelinePhoneNumberParam = new SqlParameter("@CurrentLifelinePhoneNumber", entityToAdd.CurrentLifelinePhoneNumber ?? (object)DBNull.Value);
                var lifelineProgramIdParam = new SqlParameter("@LifelineProgramId", entityToAdd.LifelineProgramId ?? (object)DBNull.Value);
                var lpProofTypeIdParam = new SqlParameter("@LPProofTypeId", entityToAdd.LPProofTypeId ?? (object)DBNull.Value);
                var lpProofNumberParam = new SqlParameter("@LPProofNumber", entityToAdd.LPProofNumber ?? (object)DBNull.Value);
                var lpImageIdParam = new SqlParameter("@LPProofImageID", entityToAdd.LPProofImageID);
                var lpProofImageUploadFilenameParam = new SqlParameter("@LPProofImageFilename", entityToAdd.LPProofImageFilename);
                var stateProgramIdParam = new SqlParameter("@StateProgramId", entityToAdd.StateProgramId ?? (object)DBNull.Value);
                var stateProgramNumber = new SqlParameter("@StateProgramNumber", entityToAdd.StateProgramNumber ?? (object)DBNull.Value);
                var secondaryStateProgramIdParam = new SqlParameter("@SecondaryStateProgramId", entityToAdd.SecondaryStateProgramId ?? (object)DBNull.Value);
                var secondaryStateProgramNumberParam = new SqlParameter("@SecondaryStateProgramNumber", entityToAdd.SecondaryStateProgramNumber ?? (object)DBNull.Value);
                var firstNameParam = new SqlParameter("@FirstName", entityToAdd.FirstName);
                var middleInitialParam = new SqlParameter("@MiddleInitial", entityToAdd.MiddleInitial ?? (object)DBNull.Value);
                var lastNameParam = new SqlParameter("@LastName", entityToAdd.LastName);
                var emailAddressParam = new SqlParameter("@EmailAddress", entityToAdd.EmailAddress ?? (object)DBNull.Value);
                var contactPhoneNumberParam = new SqlParameter("@ContactPhoneNumber", entityToAdd.ContactPhoneNumber ?? (object)DBNull.Value);
                var idProofTypeIdParam = new SqlParameter("@IDProofTypeID", entityToAdd.IDProofTypeID ?? (object)DBNull.Value);
                var idImageIdParam = new SqlParameter("@IDProofImageID", entityToAdd.IDProofImageID ?? (object)DBNull.Value);
                var idProofImageUploadFilenameParam = new SqlParameter("@IDProofImageFilename", entityToAdd.IDProofImageFilename ?? (object)DBNull.Value);
                var idImageIdParam2 = new SqlParameter("@IDProofImageID2",entityToAdd.IDProofImageID2 ?? (object)DBNull.Value);
                var idProofImageUploadFilenameParam2 = new SqlParameter("@IDProofImageFilename2",entityToAdd.IDProofImageFilename2 ?? (object)DBNull.Value);
                var serviceAddressStreet1Param = new SqlParameter("@ServiceAddressStreet1", entityToAdd.ServiceAddressStreet1);
                var serviceAddressStreet2Param = new SqlParameter("@ServiceAddressStreet2", entityToAdd.ServiceAddressStreet2 ?? (object)DBNull.Value);
                var serviceAddressCityParam = new SqlParameter("@ServiceAddressCity", entityToAdd.ServiceAddressCity);
                var serviceAddressStateParam = new SqlParameter("@ServiceAddressState", entityToAdd.ServiceAddressState);
                var serviceAddressZipParam = new SqlParameter("@ServiceAddressZip", entityToAdd.ServiceAddressZip);
                var serviceAddressIsPermanentParam = new SqlParameter("@ServiceAddressIsPermanent", entityToAdd.ServiceAddressIsPermanent);
                var billingAddressStreet1Param = new SqlParameter("@BillingAddressStreet1", entityToAdd.BillingAddressStreet1);
                var billingAddressStreet2Param = new SqlParameter("@BillingAddressStreet2", entityToAdd.BillingAddressStreet2 ?? (object)DBNull.Value);
                var billingAddressCityParam = new SqlParameter("@BillingAddressCity", entityToAdd.BillingAddressCity);
                var billingAddressStateParam = new SqlParameter("@BillingAddressState", entityToAdd.BillingAddressState);
                var billingAddressZipParam = new SqlParameter("@BillingAddressZip", entityToAdd.BillingAddressZip);
                var shippingAddressStreet1Param = new SqlParameter("@ShippingAddressStreet1", entityToAdd.ShippingAddressStreet1);
                var shippingAddressStreet2Param = new SqlParameter("@ShippingAddressStreet2", entityToAdd.ShippingAddressStreet2 ?? (object)DBNull.Value);
                var shippingAddressCityParam = new SqlParameter("@ShippingAddressCity", entityToAdd.ShippingAddressCity);
                var shippingAddressStateParam = new SqlParameter("@ShippingAddressState", entityToAdd.ShippingAddressState);
                var shippingAddressZipParam = new SqlParameter("@ShippingAddressZip", entityToAdd.ShippingAddressZip);
                var hohSpouseParam = new SqlParameter("@HohSpouse", entityToAdd.HohSpouse);
                var hohAdultsParentParam = new SqlParameter("@HohAdultsParent", entityToAdd.HohAdultsParent);
                var hohAdultsChildParam = new SqlParameter("@HohAdultsChild", entityToAdd.HohAdultsChild);
                var hohAdultsRelativeParam = new SqlParameter("@HohAdultsRelative", entityToAdd.HohAdultsRelative);
                var hohAdultsRoomateParam = new SqlParameter("@HohAdultsRoommate", entityToAdd.HohAdultsRoommate);
                var hohAdultsOtherParam = new SqlParameter("@HohAdultsOther", entityToAdd.HohAdultsOther);
                var hohAdultsOtherTextParam = new SqlParameter("@HohAdultsOtherText", entityToAdd.HohAdultsOtherText ?? (object)DBNull.Value);
                var hohExpensesParam = new SqlParameter("@HohExpenses", entityToAdd.HohExpenses ?? (object)DBNull.Value);
                var hohShareLifelineParam = new SqlParameter("@HohShareLifeline", entityToAdd.HohShareLifeline ?? (object)DBNull.Value);
                var hohShareLifelineNamesParam = new SqlParameter("@HohShareLifelineNames", entityToAdd.HohShareLifelineNames ?? (object)DBNull.Value);
                var hohAgreeMultiHouseParam = new SqlParameter("@HohAgreeMultiHouse", entityToAdd.HohAgreeMultiHouse ?? (object)DBNull.Value);
                var hohAgreeViolationParam = new SqlParameter("@HohAgreeViolation", entityToAdd.HohAgreeViolation);
                var signatureParam = new SqlParameter("@Signature", entityToAdd.Signature);
                var signatureTypeParam = new SqlParameter("@SignatureType", entityToAdd.SignatureType);
                var hasDeviceParam = new SqlParameter("@HasDevice", entityToAdd.HasDevice);
                var carrierIdParam = new SqlParameter("@CarrierId", entityToAdd.CarrierId ?? (object)DBNull.Value);
                var deviceIdParam = new SqlParameter("@DeviceId", entityToAdd.DeviceId ?? (object)DBNull.Value);
                var deviceModelParam = new SqlParameter("@DeviceModel",entityToAdd.DeviceModel ?? (object)DBNull.Value);
                var deviceCompatibilityParam = new SqlParameter("@DeviceCompatibility",entityToAdd.DeviceCompatibility ?? (object)DBNull.Value);
                var deviceIdentifierParam = new SqlParameter("@DeviceIdentifier", entityToAdd.DeviceIdentifier ?? (object)DBNull.Value);
                var simIdentifierParam = new SqlParameter("@SimIdentifier", entityToAdd.SimIdentifier ?? (object)DBNull.Value);
                var planIdParam = new SqlParameter("@PlanId", entityToAdd.PlanId ?? (object)DBNull.Value);
                var tpivBypassParam = new SqlParameter("@TpivBypass", entityToAdd.TpivBypass);
                var tpivBypassSignatureParam = new SqlParameter("@TpivBypassSignature", entityToAdd.TpivBypassSignature ?? (object)DBNull.Value);
                var tpivBypassSsnProofTypeIdParam = new SqlParameter("@TpivBypassSsnProofTypeId", entityToAdd.TpivBypassSsnProofTypeId ?? (object)DBNull.Value);
                var tpivBypassSsnCardLastFourParam = new SqlParameter("@TpivBypassSsnProofNumber", entityToAdd.TpivBypassSsnProofNumber ?? (object)DBNull.Value);
                var tpivBypassDobProofTypeIdParam = new SqlParameter("@TpivBypassDobProofTypeID", entityToAdd.TpivBypassDobProofTypeID ?? (object)DBNull.Value);
                var tpivBypassDobCardLastFourParam = new SqlParameter("@TpivBypassDobProofNumber", entityToAdd.TpivBypassDobProofNumber ?? (object)DBNull.Value);
                var latitudeCoordinateParam = new SqlParameter("@LatitudeCoordinate", entityToAdd.LatitudeCoordinate);
                var longitudeCoordinateParam = new SqlParameter("@LongitudeCoordinate", entityToAdd.LongitudeCoordinate);
                var paymentTypeParam = new SqlParameter("@PaymentType", entityToAdd.PaymentType ?? (object)DBNull.Value);
                var creditCardReferenceParam = new SqlParameter("@CreditCardReference", entityToAdd.CreditCardReference ?? (object)DBNull.Value);
                var creditCardSuccessParam = new SqlParameter("@CreditCardSuccess", entityToAdd.CreditCardSuccess);
                var creditCardTransactionIdParam = new SqlParameter("@CreditCardTransactionId", entityToAdd.CreditCardTransactionId ?? (object)DBNull.Value);
                var lifelineEnrollmentIdParam = new SqlParameter("@LifelineEnrollmentId", entityToAdd.LifelineEnrollmentId ?? (object)DBNull.Value);
                var lifelineEnrollmentTypeParam = new SqlParameter("@LifelineEnrollmentType", entityToAdd.LifelineEnrollmentType ?? (object)DBNull.Value);
                var dateCreatedParam = new SqlParameter("@DateCreated", DateTime.UtcNow);
                var dateModifiedParam = new SqlParameter("@DateModified", DateTime.UtcNow);
                var isDeletedParam = new SqlParameter("@IsDeleted", entityToAdd.IsDeleted);
                var passphraseParam = new SqlParameter("@Passphrase", Passphrase);
                var unencryptedSsnParam = new SqlParameter("@UnencryptedSsn", entityToAdd.UnencryptedSsn);
                var unencryptedDobParam = new SqlParameter("@UnencryptedDob", entityToAdd.UnencryptedDateOfBirth);
                var unencryptedQbSsnParam = new SqlParameter("@UnencryptedQbSsn", entityToAdd.UnencryptedQBSsn ?? (object)DBNull.Value);
                var unencryptedQbDobParam = new SqlParameter("@UnencryptedQbDob", entityToAdd.UnencryptedQBDateOfBirth ?? (object)DBNull.Value);
                var aiNumHouseholdParam = new SqlParameter("@AINumHousehold", entityToAdd.AINumHousehold);
                var aiNumHouseAdultParam = new SqlParameter("@AINumHouseAdult",entityToAdd.AINumHouseAdult);
                var aiNumHouseChildrenParam = new SqlParameter("@AINumHouseChildren",entityToAdd.AINumHouseChildren);
                var aiAvgIncomeParam = new SqlParameter("@AIAvgIncome", entityToAdd.AIAvgIncome);
                var aiFrequencyParam = new SqlParameter("@AIFrequency", entityToAdd.AIFrequency ?? (object)DBNull.Value);
                var aiInitialsParam = new SqlParameter("@AIInitials", entityToAdd.AIInitials ?? (object)DBNull.Value);
                var fulfillmentTypeParam = new SqlParameter("@FulfillmentType", entityToAdd.FulfillmentType ?? (object)DBNull.Value);
                var transactionIdTypeParam = new SqlParameter("@TransactionId", entityToAdd.TransactionId ?? (object)DBNull.Value);
                var parentOrderIdParam = new SqlParameter("@ParentOrderId", entityToAdd.ParentOrderId ?? (object)DBNull.Value);
                var genderParam = new SqlParameter("@Gender", entityToAdd.Gender ?? (object)DBNull.Value);
                var hohPuertoRicoAgreeViolationParam = new SqlParameter("@HohPuertoRicoAgreeViolation", entityToAdd.HohPuertoRicoAgreeViolation ?? (object)DBNull.Value);
                var serviceAddressBypassDocumentProofIdParam = new SqlParameter("@ServiceAddressBypassProofTypeID", entityToAdd.ServiceAddressBypassProofTypeID ?? (object)DBNull.Value);
                var serviceAddressBypassImageIDParam = new SqlParameter("@ServiceAddressBypassProofImageID", entityToAdd.ServiceAddressBypassProofImageID ?? (object)DBNull.Value);
                var serviceAddressBypassImageFileNameParam = new SqlParameter("@ServiceAddressBypassProofImageFilename", entityToAdd.ServiceAddressBypassProofImageFilename ?? (object)DBNull.Value);
                var serviceAddressBypassSignatureParam=new SqlParameter("@ServiceAddressBypassSignature",entityToAdd.ServiceAddressBypassSignature ?? (object)DBNull.Value);
                var serviceAddressBypassParam = new SqlParameter("@ServiceAddressBypass",entityToAdd.ServiceAddressBypass);
                var tpivSsnImageIDParam = new SqlParameter("@TpivBypassSsnProofImageID", entityToAdd.TpivBypassSsnProofImageID ?? (object)DBNull.Value);
                var tpivSsnImageUploadFilenameParam = new SqlParameter("@TpivBypassSsnProofImageFilename ", entityToAdd.TpivBypassSsnProofImageFilename ?? (object)DBNull.Value);
                var initialsParam = new SqlParameter("@Initials",entityToAdd.Initials ?? (object)DBNull.Value);
                var languageParam = new SqlParameter("@Language",entityToAdd.Language ?? (object)DBNull.Value);
                var communicationPreferenceParam = new SqlParameter("@CommunicationPreference",entityToAdd.CommunicationPreference ?? (object)DBNull.Value);
                var tpivCodeParam = new SqlParameter("@TpivCode", entityToAdd.TpivCode ?? (object)DBNull.Value);
                var tpivNasScoreParam = new SqlParameter("@TpivNasScore", entityToAdd.TpivNasScore ?? (object)DBNull.Value);
                var byopCarrierParam = new SqlParameter("@ByopCarrier",entityToAdd.ByopCarrier ?? (object)DBNull.Value);            
                var tpivTransactionIDParam = new SqlParameter("@TpivTransactionID",entityToAdd.TpivTransactionID ?? (object)DBNull.Value);
                var tpivRiskIndicatorsParam = new SqlParameter("@TpivRiskIndicators",entityToAdd.TpivRiskIndicators ?? (object)DBNull.Value);
              
            
                var tempOrderInsertResult = DbContext.TempOrders.SqlQuery("exec " + InsertStoredProcName +
                    " @Id, @CompanyId, @SalesTeamId, @UserId" +
                    ", @HouseholdReceivesLifelineBenefits, @CustomerReceivesLifelineBenefits" +
                    ", @QBFirstName, @QBLastName, @UnencryptedQbSsn, @UnencryptedQbDob" +
                    ", @CurrentLifelinePhoneNumber" +
                    ", @LifelineProgramId, @LPProofTypeId, @LPProofNumber, @LPProofImageID,@LPProofImageFilename" +
                    ", @StateProgramId, @StateProgramNumber, @SecondaryStateProgramId, @SecondaryStateProgramNumber" +
                    ", @FirstName, @MiddleInitial, @LastName, @UnencryptedSsn, @UnencryptedDob, @EmailAddress, @ContactPhoneNumber" +
                    ", @IDProofTypeID, @IDProofImageID,@IDProofImageFilename" +
                    ", @ServiceAddressStreet1, @ServiceAddressStreet2, @ServiceAddressCity, @ServiceAddressState, @ServiceAddressZip, @ServiceAddressIsPermanent" +
                    ", @BillingAddressStreet1, @BillingAddressStreet2, @BillingAddressCity, @BillingAddressState, @BillingAddressZip" +
                    ", @ShippingAddressStreet1, @ShippingAddressStreet2, @ShippingAddressCity, @ShippingAddressState, @ShippingAddressZip" +
                    ", @HohSpouse, @HohAdultsParent, @HohAdultsChild, @HohAdultsRelative, @HohAdultsRoommate, @HohAdultsOther, @HohAdultsOtherText, @HohExpenses, @HohShareLifeline, @HohShareLifelineNames, @HohAgreeMultiHouse, @HohAgreeViolation" +
                    ", @Signature, @SignatureType" +
                    ", @HasDevice, @CarrierId, @DeviceId, @DeviceIdentifier, @SimIdentifier, @PlanId" +
                    ", @TpivBypass, @TpivBypassSignature, @TpivBypassSsnProofTypeId, @TpivBypassSsnProofNumber, @TpivBypassDobProofTypeId, @TpivBypassDOBProofNumber" +
                    ", @LatitudeCoordinate, @LongitudeCoordinate" +
                    ", @PaymentType, @CreditCardReference, @CreditCardSuccess, @CreditCardTransactionId" +
                    ", @LifelineEnrollmentId, @LifelineEnrollmentType" +
                    ", @AIInitials, @AIFrequency, @AIAvgIncome, @AINumHousehold, @FulfillmentType" +
                    ", @DateCreated, @DateModified, @IsDeleted, @Passphrase,@TransactionId,@ParentOrderId,@HohPuertoRicoAgreeViolation,@Gender,@ServiceAddressBypassProofTypeID,@ServiceAddressBypassProofImageFilename,@ServiceAddressBypassProofImageID,@ServiceAddressBypassSignature,@ServiceAddressBypass, @TpivBypassSsnProofImageID,@TPIVBypassSSNProofImageFilename,@Initials,@Language,@CommunicationPreference" +
                    ", @TpivCode, @TpivNasScore, @TpivTransactionID, @TpivRiskIndicators,@AINumHouseAdult,@AINumHouseChildren,@IDProofImageID2,@IDProofImageFilename2,@DeviceCompatibility,@DeviceModel,@ByopCarrier"
                    , idParam, companyIdParam, salesTeamIdParam, userIdParam
                    , houseHoldReceivesLifelineBenefitsParam, customerReceivesLifelineBenefitsParam
                    , qbFirstNameParam, qbLastNameParam, unencryptedQbSsnParam, unencryptedQbDobParam
                    , currentLifelinePhoneNumberParam
                    , lifelineProgramIdParam, lpProofTypeIdParam, lpProofNumberParam, lpImageIdParam, lpProofImageUploadFilenameParam
                    , stateProgramIdParam, stateProgramNumber, secondaryStateProgramIdParam, secondaryStateProgramNumberParam
                    , firstNameParam, middleInitialParam, lastNameParam, unencryptedSsnParam, unencryptedDobParam, emailAddressParam, contactPhoneNumberParam
                    , idProofTypeIdParam, idImageIdParam, idProofImageUploadFilenameParam
                    , serviceAddressStreet1Param, serviceAddressStreet2Param, serviceAddressCityParam, serviceAddressStateParam, serviceAddressZipParam, serviceAddressIsPermanentParam
                    , billingAddressStreet1Param, billingAddressStreet2Param, billingAddressCityParam, billingAddressStateParam, billingAddressZipParam
                    , shippingAddressStreet1Param, shippingAddressStreet2Param, shippingAddressCityParam, shippingAddressStateParam, shippingAddressZipParam
                    , hohSpouseParam, hohAdultsParentParam, hohAdultsChildParam, hohAdultsRelativeParam, hohAdultsRoomateParam, hohAdultsOtherParam, hohAdultsOtherTextParam, hohExpensesParam, hohShareLifelineParam, hohShareLifelineNamesParam, hohAgreeMultiHouseParam, hohAgreeViolationParam
                    , signatureParam,signatureTypeParam
                    , hasDeviceParam, carrierIdParam, deviceIdParam, deviceIdentifierParam, simIdentifierParam, planIdParam
                    , tpivBypassParam, tpivBypassSignatureParam, tpivBypassSsnProofTypeIdParam, tpivBypassSsnCardLastFourParam, tpivBypassDobProofTypeIdParam, tpivBypassDobCardLastFourParam
                    , latitudeCoordinateParam, longitudeCoordinateParam
                    , paymentTypeParam, creditCardReferenceParam, creditCardSuccessParam, creditCardTransactionIdParam
                    , lifelineEnrollmentIdParam, lifelineEnrollmentTypeParam
                    , aiInitialsParam, aiFrequencyParam, aiAvgIncomeParam, aiNumHouseholdParam, fulfillmentTypeParam
                    , dateCreatedParam, dateModifiedParam, isDeletedParam, passphraseParam,transactionIdTypeParam
                    ,parentOrderIdParam,hohPuertoRicoAgreeViolationParam,genderParam,serviceAddressBypassDocumentProofIdParam
                    ,serviceAddressBypassImageFileNameParam,serviceAddressBypassImageIDParam,serviceAddressBypassSignatureParam,serviceAddressBypassParam
                    ,tpivSsnImageIDParam,tpivSsnImageUploadFilenameParam, initialsParam,languageParam,communicationPreferenceParam
                    ,tpivCodeParam,tpivNasScoreParam,tpivTransactionIDParam,tpivRiskIndicatorsParam,aiNumHouseAdultParam,aiNumHouseChildrenParam,idImageIdParam2,idProofImageUploadFilenameParam2,deviceCompatibilityParam,deviceModelParam,byopCarrierParam).ToList();


                //var tempOrderInsertResult = DbContext.TempOrders.SqlQuery("exec " + InsertStoredProcName +
                //    " @Id"
                //    , idParam).ToList();

                if (tempOrderInsertResult.Any())
                {
                    result.Data = tempOrderInsertResult.First();
                    result.IsSuccessful = true;
                    return result;
                }

                result.IsSuccessful = false;
                return result;
            }
            catch (Exception exception)
            {
                exception.ToExceptionless()
                .SetMessage("Exception thrown during temp order saving stored procedure")
                .MarkAsCritical()
                .Submit();

                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Exception thrown during temp order saving stored procedure", exception);
                throw;
            }
        }
    }
}

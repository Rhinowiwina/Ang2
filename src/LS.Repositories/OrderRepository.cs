﻿using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using LS.Core;
using LS.Domain;
using Exceptionless;
using Exceptionless.Models;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories {
    public class OrderRepository : BaseRepository<Order, string> {
        private static readonly string InsertStoredProcName = "dbo.Order_Insert";
        private static readonly string GetOrderForFulfillment_StoredProcName = "dbo.usp_GetOrders";
        private static readonly string Passphrase = "Test3r123!";

        public OrderRepository()
            : base(new AmbientDbContextLocator()) {
        }

        public DataAccessResult<List<CompletedOrder>> GetOrderForFulfillment(string UserID, string CustomerName, ICollection<string> locations, DateTime StartDate, DateTime EndDate, EntityState childrenEntityState = EntityState.Unchanged) {
            var result = new DataAccessResult<List<CompletedOrder>>();
            result.Data = new List<CompletedOrder>();

            var completedOrders = new List<CompletedOrder>();
            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlDataReader rdr = null;
            SqlCommand cmd = new SqlCommand(GetOrderForFulfillment_StoredProcName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@CustomerName", CustomerName));
            cmd.Parameters.Add(new SqlParameter("@UserId", UserID));
            cmd.Parameters.Add(new SqlParameter("@StartDate", StartDate));
            cmd.Parameters.Add(new SqlParameter("@EndDate", EndDate));
            cmd.Parameters.Add(new SqlParameter("@TeamIDs", locations));

            try {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    completedOrders.Add(new CompletedOrder() {
                        Id = rdr["Id"].ToString(),
                        CompanyId = rdr["CompanyId"].ToString(),
                        SalesTeamId = rdr["SalesTeamId"].ToString(),
                        UserId = rdr["UserId"].ToString(),
                        FirstName = rdr["FirstName"].ToString(),
                        MiddleInitial = rdr["MiddleInitial"].ToString(),
                        LastName = rdr["LastName"].ToString(),
                        ServiceAddressStreet1 = rdr["ServiceAddressStreet1"].ToString(),
                        ServiceAddressStreet2 = rdr["ServiceAddressStreet2"].ToString(),
                        ServiceAddressCity = rdr["ServiceAddressCity"].ToString(),
                        ServiceAddressState = rdr["ServiceAddressState"].ToString(),
                        ServiceAddressZip = rdr["ServiceAddressZip"].ToString(),
                        ServiceAddressIsPermanent = rdr.GetBoolean(rdr.GetOrdinal("ServiceAddressIsPermanent")),
                        ServiceAddressIsRural = rdr.GetBoolean(rdr.GetOrdinal("ServiceAddressIsRural")),
                        BillingAddressStreet1 = rdr["BillingAddressStreet1"].ToString(),
                        BillingAddressStreet2 = rdr["BillingAddressStreet2"].ToString(),
                        BillingAddressCity = rdr["BillingAddressCity"].ToString(),
                        BillingAddressState = rdr["BillingAddressState"].ToString(),
                        BillingAddressZip = rdr["BillingAddressZip"].ToString(),
                        ShippingAddressStreet1 = rdr["ShippingAddressStreet1"].ToString(),
                        ShippingAddressStreet2 = rdr["ShippingAddressStreet2"].ToString(),
                        ShippingAddressCity = rdr["ShippingAddressCity"].ToString(),
                        ShippingAddressState = rdr["ShippingAddressState"].ToString(),
                        ShippingAddressZip = rdr["ShippingAddressZip"].ToString(),
                        DeviceId = rdr["DeviceId"].ToString(),
                        DeviceIdentifier = rdr["DeviceIdentifier"].ToString(),
                        SimIdentifier = rdr["SimIdentifier"].ToString(),
                        PlanId = rdr["PlanId"].ToString(),
                        TenantReferenceId = rdr["TenantReferenceId"].ToString(),
                        TenantAccountId = rdr["TenantAccountId"].ToString(),
                        TenantAddressId = rdr["TenantAddressId"].ToString(),
                        FulfillmentType = rdr["FulfillmentType"].ToString(),
                        DeviceModel = rdr["DeviceModel"].ToString(),
                        DateCreated = (DateTime)rdr["DateCreated"],
                        DateModified = rdr["DateModified"].ToString(),
                        SalesTeamExternalDisplayName = rdr["SalesTeamExternalDisplayName"].ToString(),
                        SalesTeamName = rdr["SalesTeamName"].ToString(),
                        EmployeeFirstName = rdr["EmployeeFirstName"].ToString(),
                        EmployeeLastName = rdr["EmployeeLastName"].ToString(),
                        IsDeleted = rdr.GetBoolean(rdr.GetOrdinal("IsDeleted"))
                    });
                }

                result.Data = completedOrders;
                result.IsSuccessful = true;
            } catch (Exception ex) {

                ex.ToExceptionless()
                    .SetMessage("GetOrderForFulfillment failed.")
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("An error occurred with datareader.", ex);
            } finally { connection.Close(); }

            return result;
        }

        public override DataAccessResult<Order> Add(Order entityToAdd, EntityState childrenEntityState = EntityState.Unchanged) {
            var result = new DataAccessResult<Order>();
            try {

                var idParam = new SqlParameter("@Id", entityToAdd.Id);
                var companyIdParam = new SqlParameter("@CompanyId", entityToAdd.CompanyId);
                var salesTeamIdParam = new SqlParameter("@SalesTeamId", entityToAdd.SalesTeamId);
                var userIdParam = new SqlParameter("@UserId", entityToAdd.UserId);
                var houseHoldReceivesLifelineBenefitsParam = new SqlParameter("@HouseholdReceivesLifelineBenefits", entityToAdd.HouseholdReceivesLifelineBenefits);
                var customerReceivesLifelineBenefitsParam = new SqlParameter("@CustomerReceivesLifelineBenefits", entityToAdd.CustomerReceivesLifelineBenefits);
                var qbFirstNameParam = new SqlParameter("@QBFirstName", entityToAdd.QBFirstName ?? (object)DBNull.Value);
                var qbLastNameParam = new SqlParameter("@QBLastName", entityToAdd.QBLastName ?? (object)DBNull.Value);
                var currentLifelinePhoneNumberParam = new SqlParameter("@CurrentLifelinePhoneNumber", entityToAdd.CurrentLifelinePhoneNumber ?? (object)DBNull.Value);
                var lifelineProgramIdParam = new SqlParameter("@LifelineProgramId", entityToAdd.LifelineProgramId);
                var lpProofTypeIdParam = new SqlParameter("@LPProofTypeId", entityToAdd.LPProofTypeId);
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
                var ipProofTypeIdParam = new SqlParameter("@IDProofTypeID", entityToAdd.IDProofTypeID ?? (object)DBNull.Value);
                var ipImageIdParam = new SqlParameter("@IDProofImageID", entityToAdd.IDProofImageID ?? (object)DBNull.Value);
                var ipProofImageUploadFilenameParam = new SqlParameter("@IDProofImageFilename", entityToAdd.IDProofImageFilename ?? (object)DBNull.Value);
                var ipImageIdParam2 = new SqlParameter("@IDProofImageID2",entityToAdd.IDProofImageID2 ?? (object)DBNull.Value);
                var ipProofImageUploadFilenameParam2 = new SqlParameter("@IDProofImageFilename2",entityToAdd.IDProofImageFilename2 ?? (object)DBNull.Value);
                var serviceAddressStreet1Param = new SqlParameter("@ServiceAddressStreet1",entityToAdd.ServiceAddressStreet1);
                var serviceAddressStreet2Param = new SqlParameter("@ServiceAddressStreet2",entityToAdd.ServiceAddressStreet2 ?? (object)DBNull.Value);
                var serviceAddressCityParam = new SqlParameter("@ServiceAddressCity",entityToAdd.ServiceAddressCity);
                var serviceAddressStateParam = new SqlParameter("@ServiceAddressState",entityToAdd.ServiceAddressState);
                var serviceAddressZipParam = new SqlParameter("@ServiceAddressZip",entityToAdd.ServiceAddressZip);
                var serviceAddressIsPermanentParam = new SqlParameter("@ServiceAddressIsPermanent",entityToAdd.ServiceAddressIsPermanent);
                var serviceAddressIsRuralParam = new SqlParameter("@ServiceAddressIsRural",entityToAdd.ServiceAddressIsRural);
                var billingAddressStreet1Param = new SqlParameter("@BillingAddressStreet1",entityToAdd.BillingAddressStreet1);
                var billingAddressStreet2Param = new SqlParameter("@BillingAddressStreet2",entityToAdd.BillingAddressStreet2 ?? (object)DBNull.Value);
                var billingAddressCityParam = new SqlParameter("@BillingAddressCity",entityToAdd.BillingAddressCity);
                var billingAddressStateParam = new SqlParameter("@BillingAddressState",entityToAdd.BillingAddressState);
                var billingAddressZipParam = new SqlParameter("@BillingAddressZip",entityToAdd.BillingAddressZip);
                var shippingAddressStreet1Param = new SqlParameter("@ShippingAddressStreet1",entityToAdd.ShippingAddressStreet1);
                var shippingAddressStreet2Param = new SqlParameter("@ShippingAddressStreet2",entityToAdd.ShippingAddressStreet2 ?? (object)DBNull.Value);
                var shippingAddressCityParam = new SqlParameter("@ShippingAddressCity",entityToAdd.ShippingAddressCity);
                var shippingAddressStateParam = new SqlParameter("@ShippingAddressState",entityToAdd.ShippingAddressState);
                var shippingAddressZipParam = new SqlParameter("@ShippingAddressZip",entityToAdd.ShippingAddressZip);
                var hohSpouseParam = new SqlParameter("@HohSpouse",entityToAdd.HohSpouse);
                var hohAdultsParentParam = new SqlParameter("@HohAdultsParent",entityToAdd.HohAdultsParent);
                var hohAdultsChildParam = new SqlParameter("@HohAdultsChild",entityToAdd.HohAdultsChild);
                var hohAdultsRelativeParam = new SqlParameter("@HohAdultsRelative",entityToAdd.HohAdultsRelative);
                var hohAdultsRoomateParam = new SqlParameter("@HohAdultsRoommate",entityToAdd.HohAdultsRoommate);
                var hohAdultsOtherParam = new SqlParameter("@HohAdultsOther",entityToAdd.HohAdultsOther);
                var hohAdultsOtherTextParam = new SqlParameter("@HohAdultsOtherText",entityToAdd.HohAdultsOtherText ?? (object)DBNull.Value);
                var hohExpensesParam = new SqlParameter("@HohExpenses",entityToAdd.HohExpenses ?? (object)DBNull.Value);
                var hohShareLifelineParam = new SqlParameter("@HohShareLifeline",entityToAdd.HohShareLifeline ?? (object)DBNull.Value);
                var hohShareLifelineNamesParam = new SqlParameter("@HohShareLifelineNames",entityToAdd.HohShareLifelineNames ?? (object)DBNull.Value);
                var hohAgreeMultiHouseParam = new SqlParameter("@HohAgreeMultiHouse",entityToAdd.HohAgreeMultiHouse ?? (object)DBNull.Value);
                var hohAgreeViolationParam = new SqlParameter("@HohAgreeViolation",entityToAdd.HohAgreeViolation);
                var signatureParam = new SqlParameter("@Signature",entityToAdd.Signature);
                var signatureTypeParam = new SqlParameter("@SignatureType",entityToAdd.SignatureType);
                var sigFileNameParam = new SqlParameter("@SigFileName",entityToAdd.SigFileName);
                var hasDeviceParam = new SqlParameter("@HasDevice",entityToAdd.HasDevice);
                var carrierIdParam = new SqlParameter("@CarrierId",entityToAdd.CarrierId ?? (object)DBNull.Value);
                var deviceIdParam = new SqlParameter("@DeviceId",entityToAdd.DeviceId ?? (object)DBNull.Value);
              
                var deviceIdentifierParam = new SqlParameter("@DeviceIdentifier",entityToAdd.DeviceIdentifier ?? (object)DBNull.Value);
                var deviceCompatibilityParam = new SqlParameter("@DeviceCompatibility",entityToAdd.DeviceCompatibility ?? (object)DBNull.Value);
                var simIdentifierParam = new SqlParameter("@SimIdentifier",entityToAdd.SimIdentifier ?? (object)DBNull.Value);
                var planIdParam = new SqlParameter("@PlanId",entityToAdd.PlanId ?? (object)DBNull.Value);
                var tpivBypassParam = new SqlParameter("@TpivBypass",entityToAdd.TpivBypass);
                var tpivBypassSignatureParam = new SqlParameter("@TpivBypassSignature",entityToAdd.TpivBypassSignature ?? (object)DBNull.Value);
                var tpivBypassSsnProofTypeIdParam = new SqlParameter("@TPIVBypassSSNProofTypeID", entityToAdd.TPIVBypassSSNProofTypeID ?? (object)DBNull.Value);
                var tpivBypassSsnCardLastFourParam = new SqlParameter("@TPIVBypassSSNProofNumber", entityToAdd.TPIVBypassSSNProofNumber ?? (object)DBNull.Value);
                var tpivBypassDobProofTypeIdParam = new SqlParameter("@TpivBypassDobProofTypeId", entityToAdd.TpivBypassDobProofTypeId ?? (object)DBNull.Value);
                var tpivBypassDobCardLastFourParam = new SqlParameter("@TpivBypassDobProofNumber", entityToAdd.TpivBypassDobProofNumber ?? (object)DBNull.Value);
                var tpivCodeParam = new SqlParameter("@TpivCode", entityToAdd.TpivCode ?? (object)DBNull.Value);
                var tpivBypassMessageParam = new SqlParameter("@TpivBypassMessage", entityToAdd.TpivBypassMessage ?? (object)DBNull.Value);
                var latitudeCoordinateParam = new SqlParameter("@LatitudeCoordinate", entityToAdd.LatitudeCoordinate);
                var longitudeCoordinateParam = new SqlParameter("@LongitudeCoordinate", entityToAdd.LongitudeCoordinate);
                var paymentTypeParam = new SqlParameter("@PaymentType", entityToAdd.PaymentType ?? (object)DBNull.Value);
                var creditCardReferenceParam = new SqlParameter("@CreditCardReference", entityToAdd.CreditCardReference ?? (object)DBNull.Value);
                var creditCardSuccessParam = new SqlParameter("@CreditCardSuccess", entityToAdd.CreditCardSuccess);
                var creditCardTransactionIdParam = new SqlParameter("@CreditCardTransactionId", entityToAdd.CreditCardTransactionId ?? (object)DBNull.Value);
                var lifelineEnrollmentIdParam = new SqlParameter("@LifelineEnrollmentId", entityToAdd.LifelineEnrollmentId ?? (object)DBNull.Value);
                var lifelineEnrollmentTypeParam = new SqlParameter("@LifelineEnrollmentType", entityToAdd.LifelineEnrollmentType);
                var tenantReferenceIdParam = new SqlParameter("@TenantReferenceId", entityToAdd.TenantReferenceId ?? (object)DBNull.Value);
                var tenantAccountIdParam = new SqlParameter("@TenantAccountId", entityToAdd.TenantAccountId ?? (object)DBNull.Value);
                var tenantAddressIdParam = new SqlParameter("@TenantAddressId", entityToAdd.TenantAddressId);
                var dateCreatedParam = new SqlParameter("@DateCreated", entityToAdd.DateCreated);
                var dateModifiedParam = new SqlParameter("@DateModified", entityToAdd.DateModified);
                var isDeletedParam = new SqlParameter("@IsDeleted", entityToAdd.IsDeleted);
                var passphraseParam = new SqlParameter("@Passphrase", SqlDbType.NVarChar, 50);
                passphraseParam.Value = Passphrase;
                var unencryptedSsnParam = new SqlParameter("@UnencryptedSsn", entityToAdd.UnencryptedSsn);
                var unencryptedDobParam = new SqlParameter("@UnencryptedDob", entityToAdd.UnencryptedDateOfBirth);
                var unencryptedQbSsnParam = new SqlParameter("@UnencryptedQbSsn", entityToAdd.UnencryptedQBSsn ?? (object)DBNull.Value);
                var unencryptedQbDobParam = new SqlParameter("@UnencryptedQbDob", entityToAdd.UnencryptedQBDateOfBirth ?? (object)DBNull.Value);
                var aiNumHouseholdParam = new SqlParameter("@AINumHousehold", entityToAdd.AINumHousehold);
                var aiNumHouseAdultParam = new SqlParameter("@AINumHouseAdult", entityToAdd.AINumHouseAdult);
                var aiNumHouseChildrenParam = new SqlParameter("@AINumHouseChildren", entityToAdd.AINumHouseChildren);
                var aiAvgIncomeParam = new SqlParameter("@AIAvgIncome", entityToAdd.AIAvgIncome);
                var aiFrequencyParam = new SqlParameter("@AIFrequency", entityToAdd.AIFrequency ?? (object)DBNull.Value);
                var aiInitialsParam = new SqlParameter("@AIInitials", entityToAdd.AIInitials ?? (object)DBNull.Value);
                var fulfillmentTypeParam = new SqlParameter("@FulfillmentType", entityToAdd.FulfillmentType ?? (object)DBNull.Value);
                var deviceModelParam = new SqlParameter("@DeviceModel", entityToAdd.DeviceModel ?? (object)DBNull.Value);               
                var pricePlanParam = new SqlParameter("@PricePlan", entityToAdd.PricePlan);
                var priceTotalParam = new SqlParameter("@PriceTotal", entityToAdd.PriceTotal);
                var externalvelocitycheckParam = new SqlParameter("@ExternalVelocityCheck", entityToAdd.ExternalVelocityCheck);
                var transactionIdParam = new SqlParameter("@TransactionId", entityToAdd.TransactionId);
                var parentOrderIdParam = new SqlParameter("@ParentOrderId", entityToAdd.ParentOrderId ?? (object)DBNull.Value);
                var hohPuertoRicoAgreeViolationParam = new SqlParameter("@HohPuertoRicoAgreeViolation", entityToAdd.HohPuertoRicoAgreeViolation ?? (object)DBNull.Value);
                var serviceAddressBypassDocumentProofIdParam = new SqlParameter("@ServiceAddressBypassProofTypeID", entityToAdd.ServiceAddressBypassProofTypeID ?? (object)DBNull.Value);
                var serviceAddressBypassImageIDParam = new SqlParameter("@ServiceAddressBypassProofImageID", entityToAdd.ServiceAddressBypassProofImageID ?? (object)DBNull.Value);
                var serviceAddressBypassImageFileNameParam = new SqlParameter("@ServiceAddressBypassProofImageFilename", entityToAdd.ServiceAddressBypassProofImageFilename ?? (object)DBNull.Value);
                var serviceAddressBypassSignatureParam = new SqlParameter("@ServiceAddressBypassSignature", entityToAdd.ServiceAddressBypassSignature ?? (object)DBNull.Value);
                var serviceAddressBypassParam = new SqlParameter("@ServiceAddressBypass", entityToAdd.ServiceAddressBypass);
                var tpivSsnImageIDParam = new SqlParameter("@TPIVBypassSSNProofImageID", entityToAdd.TPIVBypassSSNProofImageID ?? (object)DBNull.Value);
                var tpivSsnImageUploadFilenameParam = new SqlParameter("@TPIVBypassSSNProofImageFilename", entityToAdd.TPIVBypassSSNProofImageFilename ?? (object)DBNull.Value);
                var genderParam = new SqlParameter("@Gender", entityToAdd.Gender ?? (object)DBNull.Value);
                var initialsParam = new SqlParameter("@Initials", entityToAdd.Initials ?? (object)DBNull.Value);
                var initialsFileNameParam = new SqlParameter("@InitialsFileName", entityToAdd.InitialsFileName);//will never be null
                var languageParam = new SqlParameter("@Language", entityToAdd.Language ?? (object)DBNull.Value);
                var communicationPreferenceParam = new SqlParameter("@CommunicationPreference", entityToAdd.CommunicationPreference ?? (object)DBNull.Value);
                var tpivNasScoreParam = new SqlParameter("@TpivNasScore", entityToAdd.TpivNasScore ?? (object)DBNull.Value);
                var tpivTransactionIDParam = new SqlParameter("@TpivTransactionID", entityToAdd.TpivTransactionID ?? (object)DBNull.Value);
                var tpivRiskIndicatorsParam = new SqlParameter("@TpivRiskIndicators", entityToAdd.TpivRiskIndicators ?? (object)DBNull.Value);
                var RTR_NameParam = new SqlParameter("@RTR_Name", entityToAdd.RTR_Name ?? (object)DBNull.Value);
                var RTR_DateParam = new SqlParameter("@RTR_Date", entityToAdd.RTR_Date ?? (object)DBNull.Value);
                var byopCarrierParam = new SqlParameter("@ByopCarrier",entityToAdd.ByopCarrier ?? (object)DBNull.Value);
                var RTR_NotesParam = new SqlParameter("@RTR_Notes", entityToAdd.RTR_Notes ?? (object)DBNull.Value);
                var RTR_RejectCodeParam = new SqlParameter("@RTR_RejectCode", entityToAdd.RTR_RejectCode ?? (object)DBNull.Value);
                var OrderCodeParam = new SqlParameter("@OrderCode", entityToAdd.OrderCode ?? (object)DBNull.Value);

                var orderInsertResult = DbContext.Orders.SqlQuery("exec " + InsertStoredProcName + " @Id, @CompanyId, @SalesTeamId, @UserId, @HouseholdReceivesLifelineBenefits," +
                    "@CustomerReceivesLifelineBenefits, @QBFirstName, @QBLastName,@UnencryptedQbSsn, @UnencryptedQbDob," +
                    "@CurrentLifelinePhoneNumber, @LifelineProgramId, @LPProofTypeId, @LPProofNumber, @LPProofImageID,@LPProofImageFilename," +
                    "@StateProgramId, @StateProgramNumber, @SecondaryStateProgramId,@SecondaryStateProgramNumber, @FirstName," +
                    "@MiddleInitial, @LastName, @UnencryptedSsn, @UnencryptedDob, @EmailAddress," +
                    "@ContactPhoneNumber, @IDProofTypeID, @IDProofImageID,@IDProofImageFilename, @ServiceAddressStreet1,@ServiceAddressStreet2," +
                    "@ServiceAddressCity, @ServiceAddressState, @ServiceAddressZip, @ServiceAddressIsPermanent, @ServiceAddressIsRural," +
                    "@BillingAddressStreet1, @BillingAddressStreet2,@BillingAddressCity, @BillingAddressState, @BillingAddressZip," +
                    "@ShippingAddressStreet1, @ShippingAddressStreet2, @ShippingAddressCity, @ShippingAddressState, @ShippingAddressZip," +
                    "@HohSpouse,@HohAdultsParent, @HohAdultsChild, @HohAdultsRelative, @HohAdultsRoommate," +
                    "@HohAdultsOther, @HohAdultsOtherText, @HohExpenses, @HohShareLifeline, @HohShareLifelineNames," +
                    "@HohAgreeMultiHouse,@HohAgreeViolation, @Signature,@SignatureType,@SigFileName," +
                    "@HasDevice, @CarrierId, @DeviceId, @DeviceIdentifier, @SimIdentifier," +
                    "@PlanId, @TpivBypass, @TpivBypassSignature, @TpivBypassSsnProofTypeId, @TpivBypassSsnProofNumber," +
                    "@TpivBypassDobProofTypeId, @TpivBypassDobProofNumber, @TpivCode, @TpivBypassMessage, @LatitudeCoordinate," +
                    "@LongitudeCoordinate, @PaymentType, @CreditCardReference, @CreditCardSuccess, @CreditCardTransactionId," +
                    "@LifelineEnrollmentId, @LifelineEnrollmentType, @AIInitials, @AIFrequency, @AIAvgIncome," +
                    "@AINumHousehold, @FulfillmentType, @DeviceModel,@PricePlan, @PriceTotal, @TenantReferenceId, @TenantAccountId," +
                    "@TenantAddressId, @DateCreated, @DateModified, @IsDeleted,@Passphrase,"+
                 "@ExternalVelocityCheck,@TransactionId,@ParentOrderId,@HohPuertoRicoAgreeViolation,@Gender,"+"@ServiceAddressBypassProofTypeID,@ServiceAddressBypassProofImageFilename,@ServiceAddressBypassProofImageID,@ServiceAddressBypassSignature,@ServiceAddressBypass,@TpivBypassSsnProofImageID,@TpivBypassSsnProofImageFilename," + "@Initials,@InitialsFileName,@Language,@CommunicationPreference, @TpivNasScore,"+                    "@TpivTransactionID, @TpivRiskIndicators, @RTR_Name, @RTR_Date, @RTR_Notes,"+ "@RTR_RejectCode,@AINumHouseAdult,@AINumHouseChildren,@OrderCode,@IDProofImageID2,@IDProofImageFilename2,@DeviceCompatibility,@ByopCarrier",
                    idParam,companyIdParam,salesTeamIdParam,userIdParam,houseHoldReceivesLifelineBenefitsParam,                customerReceivesLifelineBenefitsParam,qbFirstNameParam,qbLastNameParam,unencryptedQbSsnParam,unencryptedQbDobParam,currentLifelinePhoneNumberParam,lifelineProgramIdParam,lpProofTypeIdParam,lpProofNumberParam, lpImageIdParam, lpProofImageUploadFilenameParam,                   stateProgramIdParam,stateProgramNumber,secondaryStateProgramIdParam,secondaryStateProgramNumberParam,firstNameParam,middleInitialParam,lastNameParam,unencryptedSsnParam,unencryptedDobParam,emailAddressParam,
contactPhoneNumberParam,ipProofTypeIdParam,ipImageIdParam,ipProofImageUploadFilenameParam,serviceAddressStreet1Param, serviceAddressStreet2Param,             serviceAddressCityParam,serviceAddressStateParam,serviceAddressZipParam,serviceAddressIsPermanentParam,serviceAddressIsRuralParam,                    billingAddressStreet1Param,billingAddressStreet2Param,billingAddressCityParam,billingAddressStateParam,billingAddressZipParam,                   shippingAddressStreet1Param,shippingAddressStreet2Param,shippingAddressCityParam,shippingAddressStateParam,shippingAddressZipParam,              hohSpouseParam,hohAdultsParentParam,hohAdultsChildParam,hohAdultsRelativeParam,hohAdultsRoomateParam,       hohAdultsOtherParam,hohAdultsOtherTextParam,hohExpensesParam,hohShareLifelineParam,hohShareLifelineNamesParam,hohAgreeMultiHouseParam,hohAgreeViolationParam,signatureParam,signatureTypeParam,sigFileNameParam,
  hasDeviceParam,carrierIdParam,deviceIdParam,deviceIdentifierParam,simIdentifierParam,                  planIdParam,tpivBypassParam,tpivBypassSignatureParam,tpivBypassSsnProofTypeIdParam,tpivBypassSsnCardLastFourParam,tpivBypassDobProofTypeIdParam,tpivBypassDobCardLastFourParam,tpivCodeParam,tpivBypassMessageParam,latitudeCoordinateParam,longitudeCoordinateParam,paymentTypeParam,creditCardReferenceParam,creditCardSuccessParam,creditCardTransactionIdParam,lifelineEnrollmentIdParam,lifelineEnrollmentTypeParam,aiInitialsParam,aiFrequencyParam,aiAvgIncomeParam,aiNumHouseholdParam,fulfillmentTypeParam,deviceModelParam,pricePlanParam,priceTotalParam,tenantReferenceIdParam,tenantAccountIdParam,                    tenantAddressIdParam,dateCreatedParam,dateModifiedParam,isDeletedParam,passphraseParam,                 externalvelocitycheckParam,transactionIdParam,parentOrderIdParam,hohPuertoRicoAgreeViolationParam,genderParam,serviceAddressBypassDocumentProofIdParam,serviceAddressBypassImageFileNameParam, serviceAddressBypassImageIDParam, serviceAddressBypassSignatureParam, serviceAddressBypassParam, tpivSsnImageIDParam,tpivSsnImageUploadFilenameParam,initialsParam,initialsFileNameParam,languageParam,communicationPreferenceParam,tpivNasScoreParam,tpivTransactionIDParam, tpivRiskIndicatorsParam, RTR_NameParam,RTR_DateParam,RTR_NotesParam,RTR_RejectCodeParam,aiNumHouseAdultParam,aiNumHouseChildrenParam,OrderCodeParam,ipImageIdParam2,ipProofImageUploadFilenameParam2,deviceCompatibilityParam,byopCarrierParam).ToList();

                if (orderInsertResult.Any()) {
                    result.Data = orderInsertResult.First();
                    result.IsSuccessful = true;
                    return result;
                }

                result.IsSuccessful = false;
                return result;
            } catch (Exception exception) {
                exception.ToExceptionless()
                    .SetMessage("Exception thrown during final order saving stored procedure")
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Exception thrown during final order saving stored procedure",exception);
                throw;
            }
        }

        public DataAccessResult<Boolean> OrderExist(string orderId) {
            var result = new DataAccessResult<Boolean>() { Data = false, IsSuccessful = false };
            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlDataReader rdr = null;
            SqlCommand cmd = new SqlCommand("Select id from orders where id=@id", connection);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@id", orderId));
            try {
                connection.Open();
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows) {
                    result.Data = true;
                    result.IsSuccessful = true;
                } else {

                    result.IsSuccessful = true;
                    //data is false

                }
            } catch (Exception ex) {
                ex.ToExceptionless()
                    .SetMessage("An error occurred with datareader.")
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("An error occurred with datareader.", ex);
            } finally { connection.Close(); }

            return result;
        }

        public DataAccessResult<string> UpdateTenantAccountInfo(string orderId, string tenantId) {
            //return Successful or Failure
            var result = new DataAccessResult<string>() { Data = "Failure", IsSuccessful = false };

            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);

            SqlCommand cmd = new SqlCommand("Update Orders set TenantAccountId=@TenantAccountId where id=@id", connection);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@id", orderId));
            cmd.Parameters.Add(new SqlParameter("@TenantAccountId", tenantId));
            try {
                connection.Open();
                cmd.ExecuteNonQuery();

                result.Data = "Successful";
                result.IsSuccessful = true;

            } catch (Exception ex) {
                ex.ToExceptionless()
                    .SetMessage("An error occurred updating tenant ID.")
                    .MarkAsCritical()
                    .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                result.Data = "Failure";
                //Logger.Error("An error occurred updating tenant ID.", ex);
            } finally { connection.Close(); }

            return result;
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using System.Collections.Generic;
using LS.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LS.WebApp.Utilities;
using LS.WebApp.Controllers.api;
using LS.Repositories;
using LS.Repositories.DBContext;
using LS.Solix;
using System.Data.SqlClient;
using System.Data;
using Exceptionless;
using Exceptionless.Models;
using Newtonsoft.Json;
namespace LS.WebApp.Controllers.webapi
{
    [RoutePrefix("webapi/order")]

    public class ExternalOrdersController : BaseApiController
    {
        private static readonly string BasicAuthType = "Basic";
        private static readonly string AuthHeader = "Authorization";

        private bool RequestIsAuthorized()
        {
            var authHeader = HttpContext.Current.Request.Headers[AuthHeader];
            if (authHeader == null || !authHeader.StartsWith(BasicAuthType)) {
                return false;
            }
            var encodedUsernamePassword = authHeader.Substring((BasicAuthType + " ").Length).Trim();
            var usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var credsArray = usernamePassword.Split(':');
            var username = credsArray[0];
            var password = credsArray[1];

            var basicAuthIsValid = username == ApplicationConfig.WebAPIUsername && password == ApplicationConfig.WebAPIPassword;

            return basicAuthIsValid;
        }


        [HttpPost]
        [Route("updateOrderStatus")]
        public async Task<IHttpActionResult> updateOrderStatus()
        {
            var processingResult = new ServiceProcessingResult<BaseResponseDataResult> { IsSuccessful = true, Data = new BaseResponseDataResult() };

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            //Approved, Pending Solix = 1
            //Submitted To Solix = 100
            //Rejected = -100

            // string Id, bool Approved

            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            var request = new UpdateOrderStatusRequest {
                Id = provider.FormData.GetValues("Id")[0],
                Approved = Convert.ToBoolean(provider.FormData.GetValues("Approved")[0]),
                RTR_Name = provider.FormData.GetValues("RTR_Name")[0],
                RTR_Date = DateTime.Now,
                RTR_Notes = provider.FormData.GetValues("RTR_Notes")[0],
                RTR_RejectCode = provider.FormData.GetValues("RTR_RejectCode")[0]
            };

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", request.Id),
            };
            var sqlQuery = "SELECT TOP 1 Id, StatusID FROM Orders WHERE Id=@Id";

            var query = new SQLQuery();
            var orderExistResult = await query.ExecuteReaderAsync<UpdateOrderStatusGetOrderRequest>(CommandType.Text, sqlQuery, parameters);
            if (!orderExistResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting order for status update.", "Error getting order for status update.", false, false);
                return Ok(processingResult);
            }

            var orderExistList = (List<UpdateOrderStatusGetOrderRequest>)orderExistResult.Data;

            if (orderExistList.Count > 0) {
                var status = "";
                if (request.Approved) { status = "1"; } else { status = "-100"; }
                if (request.Approved && orderExistList[0].StatusID.Contains("100")) {  //Would be nice to check for StatusID 1 also, but that would result in orders that fail during the external account creation process below never being tried again...
                    //No need to do anything, this order has already had an external account created
                } else {
                    parameters = new SqlParameter[] {
                        new SqlParameter("@Id", request.Id),
                        new SqlParameter("@Status", status),
                        new SqlParameter("@RTR_Name", request.RTR_Name),
                        new SqlParameter("@RTR_Date", request.RTR_Date),
                        new SqlParameter("@RTR_Notes", request.RTR_Notes),
                        new SqlParameter("@RTR_RejectCode", request.RTR_RejectCode)
                    };
                    sqlQuery = "UPDATE Orders SET StatusID=@Status, RTR_Name=@RTR_Name, RTR_Date=@RTR_Date, RTR_Notes=@RTR_Notes,RTR_RejectCode=@RTR_RejectCode  WHERE Id=@Id";

                    query = new SQLQuery();
                    var setOrderStatusResult = await query.ExecuteNonQueryAsync(CommandType.Text, sqlQuery, parameters);
                    if (!setOrderStatusResult.IsSuccessful) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = new ProcessingError("Error updating order for status update.", "Error updating order for status update.", false, false);
                        return Ok(processingResult);
                    }

                    if (request.Approved) {
                        parameters = new SqlParameter[] { new SqlParameter("@Id", request.Id) };
                        sqlQuery = @"
                        SELECT TOP 1  
						    U.ExternalUserID,
					        CT.TranslatedID AS TranslatedLifelineProgram, O.LifelineProgramId, CASE WHEN LP.ProgramName = 'Annual Income' THEN 'I'  ELSE 'P' END AS EligibilityType, 
						    PDT.Name AS LPProofType,PDT2.Name AS IPProofType,PDT3.Name As SSNProofType,CONVERT(varchar, DECRYPTBYPASSPHRASE('Test3r123!', O.Ssn)) AS UnencryptedSsn,
						    CONVERT(varchar(10),CONVERT(datetime, CONVERT(varchar, DECRYPTBYPASSPHRASE('Test3r123!', O.DateOfBirth))),101)  AS UnencryptedDateOfBirth, O.Id, O.CompanyId,
						    O.CurrentLifelinePhoneNumber, O.LifelineProgramId, O.LPProofTypeId,
						    O.LPProofNumber, O.LPProofImageID,O.LPProofImageFilename, O.StateProgramId,
						    O.StateProgramNumber, O.SecondaryStateProgramId, O.SecondaryStateProgramNumber,
						    O.Language, O.CommunicationPreference, O.FirstName,
						    O.MiddleInitial, O.LastName, O.Gender,
						    O.Ssn, O.DateOfBirth, O.EmailAddress,
						    O.ContactPhoneNumber, O.IDProofTypeID, O.IDProofImageID,O.IDProofImageFilename,O.IDProofImageID2,O.IDProofImageFilename2,
						    O.ServiceAddressBypass, O.ServiceAddressBypassSignature, O.ServiceAddressBypassProofTypeID,
						    O.ServiceAddressBypassProofImageFilename, O.ServiceAddressStreet1, O.ServiceAddressStreet2,
						    O.ServiceAddressCity, O.ServiceAddressState, O.ServiceAddressZip,
						    O.ServiceAddressIsPermanent, O.ServiceAddressIsRural, O.BillingAddressStreet1,
						    O.BillingAddressStreet2, O.BillingAddressCity, O.BillingAddressState,						  
						    O.BillingAddressZip, O.ShippingAddressStreet1, O.ShippingAddressStreet2,
						    O.ShippingAddressCity, O.ShippingAddressState, O.ShippingAddressZip,
						    O.HohSpouse, O.HohAdultsParent, O.HohAdultsChild,
						    O.HohAdultsRelative, O.HohAdultsRoommate, O.HohAdultsOther,
						    O.HohAdultsOtherText, CAST(COALESCE(O.HohExpenses,0) as bit) AS HohExpenses, CAST(COALESCE(O.HohShareLifeline,0) as bit) AS HohShareLifeline, 
						    O.HohShareLifelineNames, CAST(COALESCE(O.HohAgreeMultiHouse,0) as bit) AS HohAgreeMultiHouse, CAST(COALESCE(O.HohAgreeViolation,0) as bit) AS HohAgreeViolation,
						    O.HohPuertoRicoAgreeViolation, O.Signature, O.Initials,
						    O.InitialsFileName, O.SignatureType, O.SigFileName,
						    O.HasDevice, O.CarrierId, O.DeviceId,
						    O.DeviceIdentifier, O.SimIdentifier, O.PlanId,
						    O.FullFillmentDate, O.TpivBypass, O.TpivBypassSignature,
						    O.TpivBypassSsnProofTypeID, O.TpivBypassSsnProofImageFilename, O.TpivBypassSsnProofNumber,
						    O.TpivBypassDobProofTypeId, O.TpivBypassDobProofNumber, O.TpivCode,
						    O.TpivBypassMessage, O.LatitudeCoordinate, O.LongitudeCoordinate,
						    O.PaymentType, O.CreditCardReference, O.CreditCardSuccess,
						    O.CreditCardTransactionId, O.LifelineEnrollmentId, O.LifelineEnrollmentType,
						    O.AIInitials, O.AIFrequency, O.AIAvgIncome,
						    O.AINumHousehold, O.AINumHouseAdult, O.AINumHouseChildren, O.TenantReferenceId, O.TenantAccountId,
						    O.TenantAddressId, O.PricePlan, O.PriceTotal,
						    O.FulfillmentType, O.DeviceModel, O.ExternalVelocityCheck,
						    O.TransactionId, O.DateCreated, O.DateModified,
						    O.IsDeleted, O.TpivRiskIndicators, O.TpivTransactionID,
						    O.TpivNasScore, O.StatusID, O.RTR_Name,
						    O.RTR_Date, O.RTR_Notes,O.RTR_RejectCode,O.CustomerReceivesLifelineBenefits,      O.LatitudeCoordinate,O.LongitudeCoordinate,     O.LPProofTypeId,O.LPProofImageID,O.LPProofImageFilename,O.IDProofTypeID,IDProofImageID,IDProofImageFilename,O.ByopCarrier,SV.IsByop,SV.IsRequalification,SV.RequalificationAppId,SV.IsFreePhoneEligible,SV.RequalificationMDN
                        FROM Orders O
                            LEFT JOIN LifelinePrograms LP ON O.LifelineProgramId=LP.Id
                            LEFT JOIN ProofDocumentTypes PDT ON O.LPProofTypeId=PDT.Id
                            LEFT JOIN ProofDocumentTypes PDT2 ON O.IDProofTypeId=PDT2.Id
                            LEFT JOIN ProofDocumentTypes PDT3 ON O.TpivBypassSsnProofTypeID=PDT3.Id
				            LEFT JOIN CompanyTranslations CT ON O.LifelineProgramId=Convert(NVARCHAR(128),CT.LSID) AND CT.Type='SolixLifelineProgram'
						    LEFT JOIN AspNetUsers U ON O.UserId=U.Id
                            Left Join SolixValidationDetails SV on O.id=SV.OrderId
                        WHERE O.Id=@Id AND O.IsDeleted=0
                    ";

                        query = new SQLQuery();
                        var getOrderResult = await query.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                        if (!getOrderResult.IsSuccessful) {
                            processingResult.IsSuccessful = false;
                            processingResult.Error = new ProcessingError("Error looking up order for status update.", "Error looking up order for status update.", false, false);
                            return Ok(processingResult);
                        }

                        string EligibilityType = null;
                        string LPProofType = null;
                        string IPProofType = null;
                        string SSNProofType = null;
                        string TranslatedLifelineProgram = null;
                        string ExternalUserID = null;

                        var model = new Order();
                        var IsByop = "";
                        var IsRequalification = "";
                        var RequalificationAppId = "";
                        var RequalificationMDN = "";
                        var IsFreePhoneEligible = "";
                        foreach (DataRow row in getOrderResult.Data.Rows) {
                            model.Id = row["Id"].ToString();
                            model.CustomerReceivesLifelineBenefits = Convert.ToBoolean(row["CustomerReceivesLifelineBenefits"]);
                            model.CompanyId = row["CompanyId"].ToString();
                            model.CurrentLifelinePhoneNumber = row["CurrentLifelinePhoneNumber"].ToString();
                            model.LifelineProgramId = row["LifelineProgramId"].ToString();
                            model.LPProofTypeId = row["LPProofTypeId"].ToString();
                            model.LPProofNumber = row["LPProofNumber"].ToString();
                            model.LPProofImageID = row["LPProofImageID"].ToString();
                            model.LPProofImageFilename = row["LPProofImageFilename"].ToString();
                            model.StateProgramId = row["StateProgramId"].ToString();
                            model.StateProgramNumber = row["StateProgramNumber"].ToString();
                            model.SecondaryStateProgramId = row["SecondaryStateProgramId"].ToString();
                            model.SecondaryStateProgramNumber = row["SecondaryStateProgramNumber"].ToString();
                            model.FirstName = row["FirstName"].ToString();
                            model.MiddleInitial = row["MiddleInitial"].ToString();
                            model.LastName = row["LastName"].ToString();
                            model.UnencryptedSsn = row["UnencryptedSsn"].ToString();
                            model.UnencryptedDateOfBirth = row["UnencryptedDateOfBirth"].ToString();
                            model.EmailAddress = row["emailAddress"].ToString();
                            model.ContactPhoneNumber = row["ContactPhoneNumber"].ToString();
                            model.IDProofTypeID = row["IDProofTypeID"].ToString();
                            model.IDProofImageID = row["IDProofImageID"].ToString();
                            model.IDProofImageFilename = row["IDProofImageFilename"].ToString();
                            //-------------------
                            model.IDProofImageID2 = row["IDProofImageID2"].ToString();
                            model.IDProofImageFilename2 = row["IDProofImageFilename2"].ToString();
                            model.ServiceAddressStreet1 = row["ServiceAddressStreet1"].ToString();
                            model.ServiceAddressStreet2 = row["ServiceAddressStreet2"].ToString();
                            model.ServiceAddressCity = row["ServiceAddressCity"].ToString();
                            model.ServiceAddressState = row["ServiceAddressState"].ToString();
                            model.ServiceAddressZip = row["ServiceAddressZip"].ToString();
                            model.ServiceAddressIsPermanent = Convert.ToBoolean(row["ServiceAddressIsPermanent"].ToString());
                            model.ShippingAddressStreet1 = row["ShippingAddressStreet1"].ToString();
                            model.ShippingAddressStreet2 = row["ShippingAddressStreet2"].ToString();
                            model.ShippingAddressCity = row["ShippingAddressCity"].ToString();
                            model.ShippingAddressState = row["ShippingAddressState"].ToString();
                            model.ShippingAddressZip = row["ShippingAddressZip"].ToString();
                            model.HohSpouse = Convert.ToBoolean(row["HohSpouse"].ToString());
                            model.HohAdultsParent = Convert.ToBoolean(row["HohAdultsParent"].ToString());
                            model.HohAdultsChild = Convert.ToBoolean(row["HohAdultsChild"].ToString());
                            model.HohAdultsRelative = Convert.ToBoolean(row["HohAdultsRelative"].ToString());
                            model.HohAdultsRoommate = Convert.ToBoolean(row["HohAdultsRoommate"].ToString());
                            model.HohAdultsOther = Convert.ToBoolean(row["HohAdultsOther"].ToString());
                            model.HohAdultsOtherText = row["HohAdultsOtherText"].ToString();
                            model.HohExpenses = Convert.ToBoolean(row["HohExpenses"].ToString());
                            model.HohShareLifeline = Convert.ToBoolean(row["HohShareLifeline"].ToString());
                            model.HohShareLifelineNames = row["HohShareLifelineNames"].ToString();
                            model.HohAgreeMultiHouse = Convert.ToBoolean(row["HohAgreeMultiHouse"].ToString());
                            model.HohAgreeViolation = Convert.ToBoolean(row["HohAgreeViolation"].ToString());
                            model.AIFrequency = row["AIFrequency"].ToString();
                            model.AIAvgIncome = Convert.ToInt32(row["AIAvgIncome"].ToString());
                            model.AINumHousehold = Convert.ToInt32(row["AINumHousehold"].ToString());
                            model.AINumHouseAdult = Convert.ToInt32(row["AINumHouseAdult"].ToString());
                            model.AINumHouseChildren = Convert.ToInt32(row["AINumHouseChildren"].ToString());
                            model.FulfillmentType = row["FulfillmentType"].ToString();
                            model.TPIVBypassSSNProofTypeID = row["TpivBypassSsnProofTypeID"].ToString();
                            model.TPIVBypassSSNProofImageFilename = row["TpivBypassSsnProofImageFilename"].ToString();
                            model.TpivCode = row["TpivCode"].ToString();
                            model.TpivNasScore = row["TpivNasScore"].ToString();
                            model.TpivRiskIndicators = row["TpivRiskIndicators"].ToString();
                            model.TpivTransactionID = row["TpivTransactionID"].ToString();
                            model.SigFileName = row["SigFileName"].ToString();
                            model.InitialsFileName = row["InitialsFileName"].ToString();
                            model.CommunicationPreference = row["CommunicationPreference"].ToString();
                            model.LatitudeCoordinate = (float)row["LatitudeCoordinate"];
                            model.LongitudeCoordinate = (float)row["LongitudeCoordinate"];
                            model.ByopCarrier = row["ByopCarrier"].ToString();
                            model.DeviceId = row["DeviceId"].ToString();
                            EligibilityType = row["EligibilityType"].ToString();
                            LPProofType = row["LPProofType"].ToString();
                            IPProofType = row["IPProofType"].ToString();
                            SSNProofType = row["SSNProofType"].ToString();
                            TranslatedLifelineProgram = row["TranslatedLifelineProgram"].ToString();
                            ExternalUserID = row["ExternalUserID"].ToString();

                            //Below is returned from query, is used in Solix CreateCustomer, but is not in Order model.
                            IsByop = row["IsByop"].ToString();
                            IsRequalification = row["IsRequalification"].ToString();
                            RequalificationAppId = row["RequalificationAppId"].ToString();
                            RequalificationMDN = row["RequalificationMDN"].ToString();
                            IsFreePhoneEligible = row["IsFreePhoneEligible"].ToString();
                        }

                        if ((!string.IsNullOrEmpty(RequalificationAppId) && RequalificationAppId != "0") && string.IsNullOrEmpty(RequalificationMDN)) {
                            RequalificationMDN = "8888888888";
                        }

                        if (model.CommunicationPreference == "Standard Print") {
                            model.CommunicationPreference = "1";
                        } else if (model.CommunicationPreference == "Large Print") {
                            model.CommunicationPreference = "2";
                        } else {
                            model.CommunicationPreference = "3";
                        }


                        var DocumentImage1 = "";
                        var DocumentFileName1 = "";
                        var DocumentFileDescription1 = "";

                        var DocumentImage2 = "";
                        var DocumentFileName2 = "";
                        var DocumentFileDescription2 = "";

                        var DocumentImage3 = "";
                        var DocumentFileName3 = "";
                        var DocumentFileDescription3 = "";

                        var DocumentImage4 = "";
                        var DocumentFileName4 = "";
                        var DocumentFileDescription4 = "";

                        var DocumentImage5 = "";
                        var DocumentFileName5 = "";
                        var DocumentFileDescription5 = "";

                        var SignatureImage = "";
                        var CustomerInitialsImage = "";

                        parameters = new SqlParameter[] { new SqlParameter("@CompanyId", model.CompanyId), new SqlParameter("@Type", "Proof") };
                        sqlQuery = "SELECT AccessKey, Path, SecretKey FROM ExternalStorageCredentials WHERE CompanyId=@CompanyId AND Type=@Type AND IsDeleted=0";
                        var sqlQueryService = new SQLQuery();
                        var proofExternalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                        var proofStorageRecordCount = false;
                        if (proofExternalStorageCredentialsResult.IsSuccessful) {
                            proofStorageRecordCount = true;
                        }

                        parameters = new SqlParameter[] { new SqlParameter("@CompanyId", model.CompanyId), new SqlParameter("@Type", "Signatures") };
                        sqlQuery = "SELECT AccessKey, Path, SecretKey FROM ExternalStorageCredentials WHERE CompanyId=@CompanyId AND Type=@Type AND IsDeleted=0";
                        sqlQueryService = new SQLQuery();
                        var signaturesExternalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                        var signaturesStorageRecordCount = false;
                        if (signaturesExternalStorageCredentialsResult.IsSuccessful) {
                            signaturesStorageRecordCount = true;
                        }

                        if (proofStorageRecordCount && signaturesStorageRecordCount) {
                            var proofStorageCreds = new ExternalStorageCredentials();
                            foreach (DataRow row in proofExternalStorageCredentialsResult.Data.Rows) {
                                proofStorageCreds.AccessKey = row["AccessKey"].ToString();
                                proofStorageCreds.Path = row["Path"].ToString();
                                proofStorageCreds.SecretKey = row["SecretKey"].ToString();
                            }
                            var proofExternalStorageService = new ExternalStorageService(proofStorageCreds);

                            var signaturesStorageCreds = new ExternalStorageCredentials();
                            foreach (DataRow row in signaturesExternalStorageCredentialsResult.Data.Rows) {
                                signaturesStorageCreds.AccessKey = row["AccessKey"].ToString();
                                signaturesStorageCreds.Path = row["Path"].ToString();
                                signaturesStorageCreds.SecretKey = row["SecretKey"].ToString();
                            }
                            var signatureExternalStorageService = new ExternalStorageService(signaturesStorageCreds);

                            // LP Image
                            var s3LPImage = proofExternalStorageService.GetAsBase64(model.LPProofImageFilename);
                            if (!s3LPImage.IsSuccessful) {
                                processingResult.Data.IsError = true;
                                processingResult.Data.ErrorMessage = "Lifeline Proof Image- " + s3LPImage.Error.UserHelp;
                                return Ok(processingResult);
                            }
                            DocumentImage1 = s3LPImage.Data;
                            DocumentFileName1 = model.LPProofImageFilename;
                            DocumentFileDescription1 = LPProofType;

                            // Signature Image
                            var s3SignatureImage = signatureExternalStorageService.GetAsBase64(model.SigFileName);
                            if (!s3SignatureImage.IsSuccessful) {
                                processingResult.Data.IsError = true;
                                processingResult.Data.ErrorMessage = "Signature Image - " + s3SignatureImage.Error.UserHelp;
                                return Ok(processingResult);
                            }
                            SignatureImage = s3SignatureImage.Data;

                            // Customer InitialsImage
                            var s3CustomerInitialsImage = signatureExternalStorageService.GetAsBase64(model.InitialsFileName);
                            if (!s3CustomerInitialsImage.IsSuccessful) {
                                processingResult.Data.IsError = true;
                                processingResult.Data.ErrorMessage = "Initials Image - " + s3CustomerInitialsImage.Error.UserHelp;
                                return Ok(processingResult);
                            }
                            CustomerInitialsImage = s3CustomerInitialsImage.Data;

                            //SSN TPIV ByPass Proof
                            if (!String.IsNullOrEmpty(model.TPIVBypassSSNProofImageFilename)) {
                                // ssn Image
                                var s3IPImage = proofExternalStorageService.GetAsBase64(model.TPIVBypassSSNProofImageFilename);
                                if (!s3IPImage.IsSuccessful) {
                                    processingResult.Data.IsError = true;
                                    processingResult.Data.ErrorMessage = "TPIV SSN Proof Image - " + s3IPImage.Error.UserHelp;
                                    return Ok(processingResult);
                                }
                                DocumentImage3 = s3IPImage.Data;
                                DocumentFileName3 = model.TPIVBypassSSNProofImageFilename;
                                //DocumentFileDescription3 = SSNProofType;
                                DocumentFileDescription3 = "TPIV Bypass Proof (SSN)";
                            }

                            // IP Image
                            if (!String.IsNullOrEmpty(model.IDProofImageFilename)) {
                                var s3IPImage = proofExternalStorageService.GetAsBase64(model.IDProofImageFilename);
                                if (!s3IPImage.IsSuccessful) {
                                    processingResult.Data.IsError = true;
                                    processingResult.Data.ErrorMessage = "Identity Proof Image - " + s3IPImage.Error.UserHelp;
                                    return Ok(processingResult);
                                }
                                DocumentImage4 = s3IPImage.Data;
                                DocumentFileName4 = model.IDProofImageFilename;
                                //DocumentFileDescription4 = IPProofType;
                                DocumentFileDescription4 = "TPIV Bypass Proof (ID)";
                            }
                            //IP Image 2
                            if (!String.IsNullOrEmpty(model.IDProofImageFilename2)) {
                                // IP Image
                                var s3IPImage = proofExternalStorageService.GetAsBase64(model.IDProofImageFilename2);
                                if (!s3IPImage.IsSuccessful) {
                                    processingResult.Data.IsError = true;
                                    processingResult.Data.ErrorMessage = "Identity Proof Image - " + s3IPImage.Error.UserHelp;
                                    return Ok(processingResult);
                                }
                                DocumentImage5 = s3IPImage.Data;
                                DocumentFileName5 = model.IDProofImageFilename2;
                                //DocumentFileDescription5 = IPProofType;
                                DocumentFileDescription5 = "TPIV Bypass Proof (ID 2)";
                            }

                            var solixCreateCustomerRequestData = model.ToSolixCreateCustomerModel(ExternalUserID, TranslatedLifelineProgram, EligibilityType, DocumentImage1, DocumentFileName1, DocumentFileDescription1, DocumentImage2, DocumentFileName2, DocumentFileDescription2, DocumentImage3, DocumentFileName3, DocumentFileDescription3, DocumentImage4, DocumentFileName4, DocumentFileDescription4, DocumentImage5, DocumentFileName5, DocumentFileDescription5, SignatureImage, CustomerInitialsImage, IsByop, IsRequalification, RequalificationAppId, RequalificationMDN, IsFreePhoneEligible);


                            var solixAPI = new SolixAPI();
                            var solixCreateCustomerResult = await solixAPI.CreateCustomer(solixCreateCustomerRequestData, Convert.ToDateTime(solixCreateCustomerRequestData.DOB));
                            if (!solixCreateCustomerResult.IsSuccessful) {
                                processingResult.IsSuccessful = false;
                                processingResult.Error = solixCreateCustomerResult.Error;
                                return Ok(processingResult);
                            }

                            if (solixCreateCustomerResult.Data.IsError) {
                                processingResult.IsSuccessful = true;
                                processingResult.Data.IsError = true;
                                processingResult.Data.ErrorMessage = solixCreateCustomerResult.Data.ErrorMessage;
                                return Ok(processingResult);
                            }

                            parameters = new SqlParameter[] {
                                new SqlParameter("@Id", request.Id),
                                new SqlParameter("@OrderCode", solixCreateCustomerResult.Data.EnrollmentID)
                            };
                            sqlQuery = "SELECT TOP 1 OrderCode FROM Orders WHERE OrderCode=@OrderCode AND Id!=@Id ";

                            var orderCodeExistResult = await query.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                            if (!orderCodeExistResult.IsSuccessful) {
                                processingResult.IsSuccessful = false;
                                processingResult.Error = new ProcessingError("Error getting order for status update. (Duplicate Order Code)", "Error getting order for status update. (Duplicate Order Code)", false, false);
                                return Ok(processingResult);
                            }

                            if (orderCodeExistResult.Data.Rows.Count > 0) {
                                parameters = new SqlParameter[] {
                                    new SqlParameter("@Id", request.Id),
                                    new SqlParameter("@Status", -100),
                                    new SqlParameter("@RTR_Notes", "Duplicate Order Code found."),
                                    new SqlParameter("@RTR_RejectCode", "SYS")
                                };
                                sqlQuery = "UPDATE Orders SET StatusID=@Status,RTR_Notes=@RTR_Notes,RTR_RejectCode=@RTR_RejectCode WHERE Id=@Id";

                                var setDuplicateOrderCodeStatusResult = await query.ExecuteNonQueryAsync(CommandType.Text, sqlQuery, parameters);
                                if (!setDuplicateOrderCodeStatusResult.IsSuccessful) {
                                    processingResult.IsSuccessful = false;
                                    processingResult.Error = new ProcessingError("Error updating order for status update. (Duplicate Enrollment Number)", "Error updating order for status update. (Duplicate Enrollment Number)", false, false);
                                    return Ok(processingResult);
                                }

                                processingResult.Data.ErrorMessage = "Duplicate enrollment number found. Order marked as rejected.";
                                processingResult.Data.IsError = true;

                                return Ok(processingResult);
                            }

                            status = "100";
                            parameters = new SqlParameter[] { new SqlParameter("@Id", request.Id), new SqlParameter("@Status", status), new SqlParameter("@OrderCode", solixCreateCustomerResult.Data.EnrollmentID), new SqlParameter("@TenantAccountId", solixCreateCustomerResult.Data.EnrollmentID) };
                            sqlQuery = "UPDATE Orders SET StatusID=@Status,TenantAccountId=@TenantAccountId,OrderCode=@OrderCode,DateExternalAccountCreated=GETDATE() WHERE Id=@Id";

                            query = new SQLQuery();
                            setOrderStatusResult = await query.ExecuteNonQueryAsync(CommandType.Text, sqlQuery, parameters);
                            if (!setOrderStatusResult.IsSuccessful) {
                                processingResult.IsSuccessful = true;
                                processingResult.Data.IsError = true;
                                processingResult.Data.ErrorMessage = "Error updating order for status update (completion)";
                                return Ok(processingResult);
                            }
                        } else {
                            processingResult.Data.ErrorMessage = "Unable to retrieve file storage info.";
                            processingResult.Data.IsError = true;
                        }
                    } else {
                        //Order was rejected. Status is updated above
                    }
                }
            } else {
                processingResult.Data.ErrorMessage = "Order Not Found";
                processingResult.Data.IsError = true;
                processingResult.IsSuccessful = true;
            }

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("updateFullFillmentInfo")]
        public async Task<IHttpActionResult> updateOrder()
        {
            var processingResult = new ServiceProcessingResult<string>();

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            //list of fields passed
            var OrderID = provider.FormData.GetValues("OrderID")[0];
            var DeviceIdentifier = provider.FormData.GetValues("DeviceIdentifier")[0];
            var SimIdentifier = "";
            try //SimIdentifier may not exist in some cases so use try statement
            {
                SimIdentifier = provider.FormData.GetValues("SimIdentifier")[0];
            } catch {
                SimIdentifier = "";
            }

            var FulfillmentType = provider.FormData.GetValues("FulfillmentType")[0];
            var FullFillmentDate = provider.FormData.GetValues("FullFillmentDate")[0];

            //Check if order is there
            var orderDataService = new LS.Services.OrderDataService();
            var checkResults = orderDataService.Get(OrderID);
            if (!checkResults.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Fatal backend error", "Fatal backend error.", true, false);
                processingResult.Data = "Failure";
                return Ok(processingResult);
            }
            if (checkResults.Data == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Order was not found.", "Order was not found.", true, false);
                processingResult.Data = "Failure";
                return Ok(processingResult);
            }

            using (var db = new ApplicationDbContext()) {
                db.Configuration.ValidateOnSaveEnabled = false;
                try {
                    var orderRow = new Order { Id = OrderID, DeviceIdentifier = DeviceIdentifier, SimIdentifier = SimIdentifier, FulfillmentType = FulfillmentType, FullFillmentDate = Convert.ToDateTime(FullFillmentDate) };
                    db.Orders.Attach(orderRow);
                    db.Entry(orderRow).Property(p => p.DeviceIdentifier).IsModified = true;
                    db.Entry(orderRow).Property(p => p.SimIdentifier).IsModified = true;
                    db.Entry(orderRow).Property(p => p.FulfillmentType).IsModified = true;
                    db.Entry(orderRow).Property(p => p.FullFillmentDate).IsModified = true;
                    db.SaveChanges();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Data = "Failure";
                    processingResult.Error = new ProcessingError("Fatal backend error", "Fatal backend error.", true, false);
                    return Ok(processingResult);

                } finally { db.Configuration.ValidateOnSaveEnabled = true; }
            }
            processingResult.IsSuccessful = true;
            processingResult.SuccessMessage = "Order has been updated";
            processingResult.Data = "Successful";
            return Ok(processingResult);
        }

        [HttpPost]
        [Route("updateTenantAccountInfo")]
        public async Task<IHttpActionResult> updateTenantAccountInfo()
        {

            var processingResult = new ServiceProcessingResult<string>();

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }


            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            //list of fields passed
            var OrderID = provider.FormData.GetValues("OrderID")[0];
            var TenantAccountId = provider.FormData.GetValues("TenantAccountId")[0];


            //Check if order is there
            var orderDataService = new LS.Services.OrderDataService();
            var checkResults = orderDataService.OrderExist(OrderID);
            if (!checkResults.Result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Fatal backend error", "Fatal backend error.", true, false);
                processingResult.Data = "Failure";
                return Ok(processingResult);
            }
            if (!checkResults.Result.Data) {
                //order does not exist 
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Order was not found", "Order was not found.", true, false);
                processingResult.Data = "Failure";
                return Ok(processingResult);

            }

            var updateResult = orderDataService.UpdateTenantAccountInfo(OrderID, TenantAccountId);
            if (!updateResult.Result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Fatal error updating tenant account id.", "Fatal error updating tenant account id.", true, false);
                processingResult.Data = "Failure";
                return Ok(processingResult);
            }
            processingResult.IsSuccessful = true;
            processingResult.SuccessMessage = "Tenant account id has been updated";
            processingResult.Data = "Successful";
            return Ok(processingResult);
        }



    }
    public class ClearOrderRequest
    {
        public string TableName { get; set; }
        public string FromDays { get; set; }
        public string ToDays { get; set; }

    }
}

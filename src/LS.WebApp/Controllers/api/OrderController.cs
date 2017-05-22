using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Http;
using System.Drawing;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.CGM;
using LS.CGM;
using LS.LexisNexis;
using LS.Services;
using LS.Services.ExternalApiIntegration;
using LS.WebApp.Models;
using LS.Utilities;
using System.Collections.Generic;
using LS.BudgetMobile;
using LS.Core.Interfaces;
using LS.Solix;
using System.Data;
using System.Data.SqlClient;
using Exceptionless;
using Exceptionless.Models;
using LS.WebApp.CustomAttributes;
using ApiBindingModels;

namespace LS.WebApp.Controllers.api
{

	[SingleSessionAuthorize]
	//[Authorize]

	[RoutePrefix("api/order")]
	public class OrderController : BaseApiController
	{
		private static readonly string OrderValidationFailedUserMessage = "Order validation failed.";
		private static readonly string CaliforniaApiStateCode = "CA";
		private static readonly string TexasApiStateCode = "TX";
		private static readonly string PuertoRicoApiStateCode = "PR";

		public static readonly string ProofImageExt = ImageExts.jpg.ToString();
		public static readonly string SignatureImageExt = ImageExts.png.ToString();
		public static readonly string InitialsImageExt = ImageExts.png.ToString();

		public async Task<IHttpActionResult> GetOrder(string id)
		{
			var dataService = new OrderDataService();

			var processingResult = await dataService.GetWhereAsync(id);
			if (processingResult.IsFatalFailure()) {
				//var logMessage = String.Format("A fatal error occurred while retrieving Order with Id: {0}", id);
				//Logger.Fatal(logMessage);
				ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, String.Format("A fatal error occurred while retrieving Order with Id: {0}", id), "Error").AddTags("Controller Error").Submit();
			}


			return Ok(processingResult);
		}

		[HttpPost]
		[Route("submitOrder")]
		public async Task<IHttpActionResult> SubmitOrder(SubmitOrderBindingModel model)
		{
			var processingResult = new ServiceProcessingResult<SubmitOrderResult> { Data = new SubmitOrderResult() };
			if (!ModelState.IsValid) {
				var userHelp = GetModelStateErrorsAsString(ModelState);
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError(OrderValidationFailedUserMessage, userHelp, false, true);
				return Ok(processingResult);
			}
			var regex = "^([a-zA-Z]+[,.]?[ ]?|[a-zA-Z]+['-]?)+$";
			if (!Regex.IsMatch(model.FirstName, regex, RegexOptions.IgnoreCase) || !Regex.IsMatch(model.LastName, regex, RegexOptions.IgnoreCase) || (model.MiddleInitial != null && model.MiddleInitial != "" && !Regex.IsMatch(model.MiddleInitial, regex, RegexOptions.IgnoreCase)))
			{
				processingResult.Error = new ProcessingError("Invalid character in name.", "Invalid character in name.", false, true);
				return Ok(processingResult);


			}

			if (model.DeviceId == "BYOP" || model.DeviceId == "EXT") {
				model.FulfillmentType = "store";
			}

			//if (model.ValidationDetails.IsFreePhoneEligible != "Y") {
			//	processingResult.IsSuccessful = false;
			//	processingResult.Error = new ProcessingError("IsFreePhoneEligible != Y","This customer is not eligible for a free phone.  They must contact TracFone directly to make their order.", false, true);
			//	return Ok(processingResult);
			//} else if (model.ValidationDetails.AgentCommission != "Y") {
			//	processingResult.IsSuccessful = false;
			//	processingResult.Error = new ProcessingError("AgentCommission != Y","This customer is not eligible for a commission.  They must contact TracFone directly to make their order.", false, true);
			//	return Ok(processingResult);
			//}

			//Set this at the start. If true we need the values later on to show by passes if both need shown if false they will be set to true later on if a bypass is needed.
			processingResult.Data.AddressBypassAvailable = model.HasServiceAddressBypass;
			processingResult.Data.TpivBypassAvailable = model.TpivBypass;
			processingResult.Data.TpivBypassFailureType = model.TpivBypassType;


			//Check that order is being placed with in the correct time of operations
			////Get Operations Time
			var sqlQueryTime = new SQLQuery();

			var queryString = "SELECT OrderStart, OrderEnd, TimeZone FROM Companies where Id=@Id ";

			SqlParameter[] timeparameters = new SqlParameter[] {
				new SqlParameter("@Id",LoggedInUser.CompanyId)};
			var timeResult = await sqlQueryTime.ExecuteReaderAsync<OperationTime>(CommandType.Text, queryString, timeparameters);
			if (!timeResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Failed to retrieve time of operations.", "Failed to retrieve time of operations.", true, false);
				ExceptionlessClient.Default.CreateLog("Error retriveing company times of operations", "Error").AddObject(timeparameters).Submit();
				return Ok(processingResult);
			};

			var opTimeResult = (List<OperationTime>)timeResult.Data;

			var companyNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(opTimeResult[0].TimeZone));

			TimeSpan beginTime = new TimeSpan();
			TimeSpan endTime = new TimeSpan();
			try {
				if (!TimeSpan.TryParse(opTimeResult[0].OrderStart, out beginTime)) { throw new ArgumentNullException("beginTime"); }
				if (!TimeSpan.TryParse(opTimeResult[0].OrderEnd, out endTime)) { throw new ArgumentNullException("endTime"); }
			} catch (Exception ex) {

				ex.ToExceptionless()
					   .SetMessage("Error converting string to timespan.")
					   .MarkAsCritical()
					   .Submit();
				processingResult.IsSuccessful = false;

				processingResult.Error = new ProcessingError("Invalid operation time.", "Invalid operation time.", true, false);
				return Ok(processingResult);

			}
			if ((beginTime < endTime && (companyNow.TimeOfDay < beginTime || companyNow.TimeOfDay > endTime)) || (endTime < beginTime && (companyNow.TimeOfDay < beginTime && companyNow.TimeOfDay > endTime))) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Currently outside of operating hours. Order cannot be submitted.", "It is currently outside of operating hours. Order cannot be submitted.", true, false);
				return Ok(processingResult);
			}


			var userService = new ApplicationUserDataService();
			var userResult = await userService.GetAsync(LoggedInUser.Id);
			if (userResult.Data.SalesTeam != null && userResult.Data.SalesTeam.IsActive == false) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("The team you belong to has been deactivated. You can not place orders.", "The team you belong to has been deactivated. You can not place orders.", true, false);
				return Ok(processingResult);
			}

			if (model.ServiceAddressState == "PR" && (model.HohPuertoRicoAgreeViolation == false || model.HohPuertoRicoAgreeViolation == null)) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Puerto Rico penalties acceptance must be checked.", "Puerto Rico penalties acceptance must be checked.", false, true);
				return Ok(processingResult);
			}
			if (!string.IsNullOrEmpty(model.ContactPhoneNumber)) {
				model.ContactPhoneNumber = Regex.Replace(model.ContactPhoneNumber, "[^0-9]*", "");
			}

			//for cgm check and enroll
			model.AgentFirstName = LoggedInUser.FirstName;
			model.AgentLastName = LoggedInUser.LastName;
			var companyId = GetLoggedInUserCompanyId();
			model.CompanyID = companyId;
			var companyDataService = new CompanyDataService();
			var getCompanyResult = companyDataService.GetWithSacEntries(companyId);
			if (!getCompanyResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = getCompanyResult.Error;
				return Ok(processingResult);
			}
			var company = getCompanyResult.Data;
			// **via Specs**
			// Verify API Key

			var salesTeamDataService = new SalesTeamDataService(LoggedInUserId);
			var salesTeamResult = await salesTeamDataService.GetSalesTeamsForUserWhereAsync(model.SalesTeamId, LoggedInUserId);
			if (!salesTeamResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = salesTeamResult.Error;
				return Ok(processingResult);
			}
			var salesTeam = salesTeamResult.Data;

			var tempOrderService = new TempOrderDataService();

			model.LPProofImageUploadFilename = Utils.CreateFilenameFromID(model.LPProofImageUploadId, ProofImageExt);
			model.IPProofImageUploadFilename = Utils.CreateFilenameFromID(model.IPProofImageUploadId, ProofImageExt);
			model.IPProofImageUploadFilename2 = Utils.CreateFilenameFromID(model.IPProofImageUploadId2, ProofImageExt);
			model.TpivSsnImageUploadFilename = Utils.CreateFilenameFromID(model.TpivSsnImageUploadId, ProofImageExt);

			// Write to temp DB table(s)
			var tempOrder = model.ToTempOrder(companyId, LoggedInUser);
			// Validate Signature and type present.
			if (string.IsNullOrEmpty(model.Signature)) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Signature is not present.", "Signature is not present.", true, false);
				return Ok(processingResult);
			}

			if (string.IsNullOrEmpty(model.Initials)) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Initials are not present.", "Initials are not present.", true, false);
				return Ok(processingResult);
			}
			//Temp order Insert
			var writeTempRowResult = await tempOrderService.AddAsync(tempOrder);
			if (!writeTempRowResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = ErrorValues.GENERIC_ADD_TEMP_ORDER_ERROR;
				return Ok(processingResult);
			}
			model.OrderId = writeTempRowResult.Data.Id;

			var tempOrderValidationResult = await tempOrderService.ValidateTempOrderAsync(model, LoggedInUserId, companyId);
			if (!tempOrderValidationResult.IsSuccessful || !tempOrderValidationResult.Data.IsValid) {
				if (tempOrderValidationResult.IsFatalFailure()) {
					//Logger.Fatal("A fatal error occurred while validating new order.");
					ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "A fatal error occurred while validating new order.", "Error").AddTags("Controller Error").Submit();
				}
				processingResult.IsSuccessful = false;
				processingResult.Error = tempOrderValidationResult.IsSuccessful
					? tempOrderValidationResult.Data.ToProcessingError("Order validation failed.")
					: tempOrderValidationResult.Error;
				return Ok(processingResult);
			}

			//TODO: Validate Program and ID Proof files exist in external storage
			var credentialsService = new ExternalStorageCredentialsDataService();
			var getCredentialsResult = await credentialsService.GetProofImageStorageCredentialsFromCompanyId(companyId, "Proof");
			if (!getCredentialsResult.IsSuccessful || getCredentialsResult.Data == null) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("An error occurred while retrieving storage credentials.", "An error occurred while retrieving storage credentials. If the problem presists, please contact support.", true);
				return Ok(processingResult);
			}

			var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
			if (!externalStorageService.DoesFileExist(Utils.CreateFilenameFromID(model.LPProofImageUploadId, ProofImageExt))) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Program Proof image could not be found", "Program Proof image could not be found", true, false);
				return Ok(processingResult);
			}

			//if (!externalStorageService.DoesFileExist(model.IPProofImageUploadId + ".png")) {
			//    processingResult.IsSuccessful = false;
			//    processingResult.Error = new ProcessingError("ID Proof image could not be found", "ID Proof image could not be found", true, false);
			//    return Ok(processingResult);
			//}


			var sqlQuery = "SELECT COUNT(*) AS TotalSubmissions FROM TempOrders WHERE TransactionID=@TransactionID AND DateCreated>DateAdd(day, -2, getdate())";
			SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@TransactionID", model.TransactionId) };


			var sqlQueryService = new SQLQuery();
			var numOrderSubmissionsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
			if (!numOrderSubmissionsResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Error performing lookup for Transaction ID velocity", "There was a temporary error looking up order information.  Please try again.", false, false);
				return Ok(processingResult);
			}

			int totalSubmissions = 0;
			foreach (DataRow row in numOrderSubmissionsResult.Data.Rows) {
				totalSubmissions = Convert.ToInt32(row["TotalSubmissions"].ToString());
			}

			//Check the scrubbed zip code against the coverage table
			//**The scrub could cause the zip code to change, which means it could be a zip code that is not in the coverage.
			var Zip5 = model.ServiceAddressZip.Substring(0, 5);
			var checkZipCoverageSqlClient = new SQLQuery();
			SqlParameter[] checkZipCoverageParams = new SqlParameter[] {
				new SqlParameter("@PostalCode", Zip5)
			};
			var checkZipCoverageSqlCmd = "SELECT PostalCode FROM ZipcodeCoverage WHERE PostalCode=@PostalCode ";
			var checkZipCoverageResult = await checkZipCoverageSqlClient.ExecuteReaderAsync(CommandType.Text, checkZipCoverageSqlCmd, checkZipCoverageParams);
			if (!checkZipCoverageResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Error checking zipcode (" + Zip5 + ") coverage after address validation. Please contact support.", "Error checking zipcode  (" + Zip5 + ") coverage after address validation. Please contact support.", false, false);
				return Ok(processingResult);
			}

			if (checkZipCoverageResult.Data.Rows.Count == 0) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("The service address' zip code (" + Zip5 + ") does not have cell phone coverage in our network.", "The service address' zip code (" + Zip5 + ") does not have cell phone coverage in our network.", false, false);
				return Ok(processingResult);
			}


			var apiResult = GetExternalApiFrom(model, company);
			if (!apiResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError(apiResult.Error.UserMessage, apiResult.Error.UserMessage, true, false);
				//Logger.Fatal("The order could not be submitted because a valid SAC number was not found");
				ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "The order could not be submitted because a valid SAC number was not found", "Error").AddTags("Controller Error").Submit();
				return Ok(processingResult);
			}

			var externalApiToUse = apiResult.Data;

			string tpivCode = model.TpivCode;
			string tpivRiskIndicators = model.TpivRiskIndicators;
			string tpivNasScore = model.TpivNasScore;
			string tpivTransactionID = model.TpivTransactionID;

			if (tpivCode != null) {
				var tpivCodeCheckSqlQuery = new SQLQuery();
				var tpivCodeCheckString = "SELECT TpivCode FROM Orders (NOLOCK) WHERE TpivCode=@TpivCode AND TenantAccountId IS NOT NULL AND DateCreated < DATEADD(day, @DupLxNxIDAccountAgeDays, GETDATE())";

				SqlParameter[] tpivCodeCheckStringParameters = new SqlParameter[] {
					new SqlParameter("@TpivCode",tpivCode),
					new SqlParameter("@DupLxNxIDAccountAgeDays", Convert.ToInt32(ConfigurationManager.AppSettings["DupLxNxIDAccountAgeDays"]))
				};
				var tpivCodeCheckResult = await tpivCodeCheckSqlQuery.ExecuteReaderAsync(CommandType.Text, tpivCodeCheckString, tpivCodeCheckStringParameters);
				if (!tpivCodeCheckResult.IsSuccessful) {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("Error performing internal duplicate TPIV Code check", "Error performing internal duplicate TPIV Code check", true, false);
					ExceptionlessClient.Default.CreateLog("Error performing internal duplicate TPIV Code check", "Error").AddObject(tpivCode).Submit();
					return Ok(processingResult);
				};

				if (tpivCodeCheckResult.Data.Rows.Count >= 1) {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("This customer is a duplicate of a previously created account (Internal TPIV Code Check)", "This customer is a duplicate of a previously created account (Internal TPIV Code Check)", true, false);
					return Ok(processingResult);
				}
			}

			//Start LifelineDB Verify
			var lifelineApplicationService = new LifelineApplicationDataService(externalApiToUse, LoggedInUserId);
			var checkStatusRequestData = model.ToCheckStatusRequestData();


			var checkStatusResult = await lifelineApplicationService.CheckCustomerStatusAsync(checkStatusRequestData);
			if (!checkStatusResult.IsSuccessful || (checkStatusResult.Data.Errors != null && checkStatusResult.Data.Errors.Count > 0)) {
				processingResult.IsSuccessful = checkStatusResult.IsSuccessful;

				if (checkStatusResult.Data == null) {
					processingResult.Error = checkStatusResult.Error;
					return Ok(processingResult);
				}

				var fullerror = "";
				if (checkStatusResult.Data.Errors.Count > 0) {
					foreach (var error in checkStatusResult.Data.Errors) {
						fullerror += externalApiToUse + ": " + error;
						fullerror += "<br>";
					}
					processingResult.Error = new ProcessingError("", fullerror, false, true);
					//if already true keep true value if false see if there is a change
					if (!processingResult.Data.AddressBypassAvailable) { processingResult.Data.AddressBypassAvailable = checkStatusResult.Data.AddressBypassAvailable; }
					if (!processingResult.Data.TpivBypassAvailable) {
						processingResult.Data.TpivBypassAvailable = checkStatusResult.Data.TpivBypassAvailable;
						processingResult.Data.TpivBypassFailureType = checkStatusResult.Data.TpivBypassFailureType;
					}
				} else {
					processingResult.Error = checkStatusResult.Error;
				}

				return Ok(processingResult);
			}

			if (externalApiToUse == ExternalApi.Nlad) {
				model.IsHoh = checkStatusResult.Data.Hoh;
			}

			model.ServiceAddressIsRural = checkStatusResult.Data.ServiceAddressIsRural;
			//end LifelineDB Verify

			/* PER Budget (5/10/2016) - No longer do CGM stuff
            var cgmModel = BindingModelsConverter.ToCGMCheckRequestModel(model);
            var cgmCheckResult = new ServiceProcessingResult<CGMCovertedResponse>();
            //if (model.ServiceAddressState == "CA") { //Since we are doing a CGM enroll in all states, we have to do a check to get the Transaction ID
            //CGM CHECK
            cgmCheckResult = await cgmCheck(cgmModel);
            if (!cgmCheckResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = cgmCheckResult.Error;
                return Ok(processingResult);
            }
            model.ExternalVelocityCheck = cgmCheckResult.Data.ExternalVelocityCheck;
            if (model.ServiceAddressState != "CA") {
                model.ExternalVelocityCheck = "";
            }
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["StopForVelocityFailure"])) {
                if (model.ExternalVelocityCheck == "FAIL") {
                    processingResult.IsSuccessful = false;
                    string transfermsg = "Applicant must call current Lifeline carrier to cancel existing service prior to initiating and completing the application process.  Once existing service with current Lifeline carrier has been canceled, applicant may call Budget Mobile’s Customer Call Center at 1-844-840-3733 to initiate and complete the application process.  Handset will be shipped within 5-7 business days after account has been approved.";
                    string newmsg = "Applicant must call Budget Mobile’s Customer Call Center at 1-844-840-3733 to initiate and complete the application process.  Handset will be shipped within 5-7 business days after the application has been approved.";
                    if (checkStatusResult.Data.EnrollmentType == EnrollmentType.Transfer) { processingResult.Error = new ProcessingError("CGM Velocity Trigger - Transfer", transfermsg, true, false); } else { processingResult.Error = new ProcessingError("CGM Velocity Trigger - New", newmsg, true, false); }
                    return Ok(processingResult);
                }
            }*/
			model.ExternalVelocityCheck = "";

			//if Nevada do Nevada SolixLifelineApplicationService If NLAD was bad we would not be here.
			if (checkStatusRequestData.ServiceAddressState == "NV") {
				var solixApplicationService = new NevadaSolixLifelineApplicationService();
				var nevadaSolixData = model.ToSolixRequestData();
				var nevadaSolixResult = await solixApplicationService.NevadaEligibilityAsync(nevadaSolixData);
				if (nevadaSolixResult.IsSuccessful) {
					if ((nevadaSolixResult.Data.Eligible == "True" || nevadaSolixResult.Data.Eligible == "true") && nevadaSolixResult.Data.StatusCode == "1") {
						//do nothing continue with order customer is eligible(1).
					} else {
						string msg = "Customer deemed not-eligible to received Lifeline benefits, per the NV DHHS eligibility check.  Budget may not accept any documentation from customer to prove eligibility should customer be deemed not-eligible.  Customer should contact their case worker to dispute not-eligible results.  Customer may also visit nvlifeline.org or call 1-844-3630-1867 to complete Solix’s Nevada Lifeline Database application process.";
						processingResult.IsSuccessful = false;
						processingResult.Error = new ProcessingError(msg, "NevadaSolix Message:" + msg, true, false);
						return Ok(processingResult);
					}
				} else {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError(nevadaSolixResult.Error.UserMessage, nevadaSolixResult.Error.UserMessage, true, false);
					return Ok(processingResult);
				}
			}


			//var companyProviderOptions = new CompanyProviderOptions(LoggedInUser.FullName);
			//var CAMsDuplicateCheck = companyProviderOptions.DuplicateCheck(model.FirstName, model.LastName, model.DateOfBirth, model.Ssn, tpivCode);
			//if (!CAMsDuplicateCheck.IsSuccessful) {
			//    processingResult.IsSuccessful = false;
			//    processingResult.Error = CAMsDuplicateCheck.Error;
			//    return Ok(processingResult);
			//}

			//if (CAMsDuplicateCheck.Data.IsDuplicate) {
			//    processingResult.IsSuccessful = false;
			//    processingResult.Error = new ProcessingError("CAMs - Duplicate subscriber", "CAMs - Duplicate subscriber", false, false);
			//    return Ok(processingResult);
			//}

			//Final Order Saving
			var orderDataService = new OrderDataService();

			model.SignatureImageFilename = Utils.CreateFilenameFromID(model.OrderId, SignatureImageExt);
			model.InitialsFileName = Utils.CreateFilenameFromID("InitialSig_" + model.OrderId, InitialsImageExt);
			model.ServiceAddressByPassImageFileName = Utils.CreateFilenameFromID(model.ServiceAddressByPassImageID, ProofImageExt);

			var finalOrder = model.ToFinalOrder(checkStatusResult.Data, LoggedInUser, companyId);
			finalOrder.Id = tempOrder.Id;
			if (externalApiToUse != ExternalApi.Nlad) {
				finalOrder.TpivCode = tpivCode;
				finalOrder.TpivRiskIndicators = tpivRiskIndicators;
				finalOrder.TpivNasScore = tpivNasScore;
				finalOrder.TpivTransactionID = tpivTransactionID;

			}

			//Allow transfers to be fulfilled in store. 02/10/2016 per KW
			if (externalApiToUse == ExternalApi.California) {
				if ((ConfigurationManager.AppSettings["LimitCATransferFulfillType"] == "1" && checkStatusResult.Data.EnrollmentType != EnrollmentType.New)) {
					finalOrder.FulfillmentType = "mail";
					finalOrder.DeviceModel = "$0 Free Phone";
					finalOrder.DeviceId = "261";
					processingResult.WarningMessage = "Based on internal screening of customer's application, the order will be fulfilled and mailed to customer within 10 days.";
				}
			}

			//check if duplicate submission
			//AccountId Is requierd checked in SubmitOrderBindingModel.If null count will be > 0 and create a error.

			var dupCheckResult = await orderDataService.TransactionIDExistInOrders(finalOrder.TransactionId);
			if (!dupCheckResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = dupCheckResult.Error;

				return Ok(processingResult);
			} else {
				if (dupCheckResult.Data) {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("This order has already been successfully submitted and cannot be submitted again.  Please check the Order Status page for more detail.", "This order has already been successfully submitted and cannot be submitted again.  Please check the Order Status page for more detail.", false, true);

					return Ok(processingResult);
				}
			}
			//save solix validation if fails stop order
			var solixResult = await orderDataService.AddSolixValidationDetails(model.ValidationDetails, finalOrder.Id);
			if (!solixResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = solixResult.Error;
				return Ok(processingResult);
			}

			var finalOrderSaveResult = await orderDataService.AddAsync(finalOrder);
			if (!finalOrderSaveResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = finalOrderSaveResult.Error;
				return Ok(processingResult);
			}

			try {
				var productCommissionsDataService = new ProductCommissionsDataService();

				var productCommissionsResult = await productCommissionsDataService.LogProductCommission("Account", salesTeam.Id, finalOrder.Id, LoggedInUser.Id);
				if (!productCommissionsResult.IsSuccessful) {
					processingResult.IsSuccessful = false;
					processingResult.Error = productCommissionsResult.Error;

					finalOrder.IsDeleted = true;
					var finalOrderUpdateResult = await orderDataService.UpdateAsync(finalOrder);
					if (!finalOrderUpdateResult.IsSuccessful) {
						//Logger.Fatal("Error updating final order details (after commissions failure)");
						ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Error updating final order details (after commissions failure)", "Error").AddTags("Controller Error").Submit();
					}
					return Ok(processingResult);
				}


				/* PER Budget (5/10/2016) - No longer do CGM stuff
                //if (model.ServiceAddressState == "CA") { // Per Budget, do enroll in all states
                //CGM Enroll let CGM know about customer attempt to subscribe
                var cgmEnrollModel = BindingModelsConverter.ToCGMEnrollModel(cgmModel);
                cgmEnrollModel.Token = cgmCheckResult.Data.Token;
                cgmEnrollModel.TransactionId = cgmCheckResult.Data.Transactionid;
                cgmEnrollModel.EnrollmentType = "Enrollment";
                var cgmEnrollResult = await cgmEnroll(cgmEnrollModel);
                //We don't care if it fails go on this is a curtesy info to cgm
                //}*/

				processingResult.IsSuccessful = true;
			} catch (Exception ex) {
				finalOrder.IsDeleted = true;

				var finalOrderUpdateResult = await orderDataService.UpdateAsync(finalOrder);
				if (!finalOrderUpdateResult.IsSuccessful) {
					//TODO: Email developers
					//Logger.Fatal("Error updating final order details (taxes, commissions, budget rest insert)");
					ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Error updating final order details (taxes, commissions, budget rest insert)", "Error").AddTags("Controller Error").Submit();
				}
				//Logger.Fatal("Error saving final order details", ex);
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Error saving final order details", "There was an error saving this order.  Please try submitting the order again.  If the problem persists, please contact support.", true, false);
			}
			var sigService = new SignatureToImage();
			var saveSigImageResult = sigService.GetImage(finalOrder.Signature, finalOrder.SignatureType, false);
			if (saveSigImageResult.IsSuccessful) {
				var storageResult = await SendImageToStorage(saveSigImageResult.Data, finalOrder.SigFileName, "png");
				if (!storageResult.IsSuccessful) {
					//Logger.Fatal(new ProcessingError("Image " + finalOrder.Id + " was not save to external storage", "Image " + finalOrder.Id + " was not save to external storage", true, false));

					ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Image " + finalOrder.Id + " was not save to external storage", "Error").AddTags("Controller Error").Submit();
				}
			} else {
				//Logger.Fatal(new ProcessingError("Image " + finalOrder.Id + " could not be created.", "Image " + finalOrder.Id + " could not be created.", true, false));
				ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Image " + finalOrder.Id + " could not be created.", "Error").AddTags("Controller Error").Submit();
			}
			//Initials
			sigService = null;
			sigService = new SignatureToImage();
			var saveSigInitialsImageResult = sigService.GetImage(finalOrder.Initials, finalOrder.SignatureType, true);
			if (saveSigInitialsImageResult.IsSuccessful) {
				var storage2Result = await SendImageToStorage(saveSigInitialsImageResult.Data, finalOrder.InitialsFileName, "png");
				if (!storage2Result.IsSuccessful) {
					//Logger.Fatal(new ProcessingError("Image " + finalOrder.Id + " was not save to external storage", "Image " + finalOrder.Id + " was not save to external storage", true, false));
					ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Image " + finalOrder.Id + " was not save to external storage", "Error").AddTags("Controller Error").Submit();
				}
			} else {
				//Logger.Fatal(new ProcessingError("Image " + finalOrder.Id + " could not be created.", "Image " + finalOrder.Id + " could not be created.", true, false));
				ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Image " + finalOrder.Id + " could not be created.", "Error").AddTags("Controller Error").Submit();
			}


			processingResult.SuccessMessage = "<strong>Order ID:</strong>" + finalOrder.Id;

			return Ok(processingResult);

		}

		private async Task<DataAccessResult> SendImageToStorage(Image file, string orderid, string imageType)
		{
			var AccessResult = new DataAccessResult();

			var credentialService = new ExternalStorageCredentialsDataService();
			var getCredentialsResult = await credentialService.GetProofImageStorageCredentialsFromCompanyId(LoggedInUser.CompanyId, "Signatures");
			if (!getCredentialsResult.IsSuccessful) {
				AccessResult.IsSuccessful = false;
				AccessResult.Error = getCredentialsResult.Error;
				return AccessResult;
			}
			var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
			var result = externalStorageService.SaveImage(file, orderid, imageType);
			if (!result.IsSuccessful) {
				AccessResult.IsSuccessful = false;
				AccessResult.Error = result.Error;
				return AccessResult;
			}
			AccessResult.IsSuccessful = true;
			return AccessResult;
		}

		private DataAccessResult<ExternalApi> GetExternalApiFrom(SubmitOrderBindingModel model, Company company)
		{
			var lifeLineProgramDataService = new LifelineProgramDataService();

			var getResult = lifeLineProgramDataService.Get(model.LifelineProgramId);
			if (!getResult.IsSuccessful) {
				return new DataAccessResult<ExternalApi> {
					Error = getResult.Error,
					IsSuccessful = false
				};
			}

			var lifeLineProgram = getResult.Data;

			if (!company.SacEntries.All(s => s.StateCode != lifeLineProgram.StateCode)) { // Is NLAD State?
				if (string.IsNullOrEmpty(lifeLineProgram.NladEligibilityCode)) {

					ExceptionlessClient.Default.CreateLog(ErrorValues.NO_VALID_ELIGIBILITY_CODE.UserMessage);

					return new DataAccessResult<ExternalApi> {
						IsSuccessful = false,
						Error = ErrorValues.NO_VALID_ELIGIBILITY_CODE
					};
				}

				return new DataAccessResult<ExternalApi> {
					Data = ExternalApi.Nlad,
					IsSuccessful = true
				};
			}

			if (lifeLineProgram.StateCode == CaliforniaApiStateCode) {
				return new DataAccessResult<ExternalApi> {
					Data = ExternalApi.California,
					IsSuccessful = true
				};
			}

			if (lifeLineProgram.StateCode == TexasApiStateCode) {
				return new DataAccessResult<ExternalApi> {
					Data = ExternalApi.TexasSolix,
					IsSuccessful = true
				};
			}
			ExceptionlessClient.Default.CreateLog(ErrorValues.NO_VALID_SAC_NUMBER_ENTRY_FOR_COMPANY.UserMessage);

			return new DataAccessResult<ExternalApi> {
				IsSuccessful = false,
				Error = ErrorValues.NO_VALID_SAC_NUMBER_ENTRY_FOR_COMPANY
			};
		}

		[HttpPost]
		[Route("californiaPrecheck")]
		public async Task<IHttpActionResult> californiaPrecheck(CaliforniaPrecheckBindingModel model)
		{
			var processingResult = new ServiceProcessingResult<CaPreCheckMsg> { IsSuccessful = true };

			model.CompanyID = LoggedInUser.CompanyId;

			if (ConfigurationManager.AppSettings["LimitCATransferFulfillType"] == "1" || Convert.ToBoolean(ConfigurationManager.AppSettings["StopForVelocityFailure"])) {
				var addressValidationService = new AddressValidationDataService();
				var validateAddressResult = await addressValidationService.Standardize(new AddressStandardizeRequest {
					Address = model.Address, Address2 = model.Address2, City = model.City, State = model.State, Zip = model.Zipcode
				});

				//Check validateAddressResult.IsSuccessful.  If success, set model address fields to returned values.  Update Temp Order row?
				if (!validateAddressResult.IsSuccessful) {
					processingResult.IsSuccessful = validateAddressResult.IsSuccessful;
					processingResult.Error = validateAddressResult.Error;
					return Ok(processingResult);
				}

				model.Address = validateAddressResult.Data.Address1;
				model.Address2 = validateAddressResult.Data.Address2;
				model.State = validateAddressResult.Data.State;
				model.City = validateAddressResult.Data.City;
				model.Zipcode = validateAddressResult.Data.Zip;

				if (!validateAddressResult.Data.IsValid) {
					processingResult.IsSuccessful = false;

					var fullerror = "";
					foreach (var error in validateAddressResult.Data.ValidationRejections) {
						fullerror += error + "<br>";
					}
					processingResult.Error = new ProcessingError("", fullerror, false, true);
					return Ok(processingResult);
				}
			}

			var preCheckData = new CaPreCheckMsg();

			if (Convert.ToBoolean(ConfigurationManager.AppSettings["StopForVelocityFailure"])) {
				//var cgmModel = BindingModelsConverter.ToCGMCheckRequestModel(model);
				//cgmModel.AgentFirstName = LoggedInUser.FirstName;
				//cgmModel.AgentLastName = LoggedInUser.LastName;

				//var cgmCheckResult = await cgmCheck(cgmModel);
				//if (!cgmCheckResult.IsSuccessful) {
				//    processingResult.IsSuccessful = false;
				//    processingResult.Error = cgmCheckResult.Error;
				//    return Ok(processingResult);
				//}
				//preCheckData.cgmVelocityCheck = cgmCheckResult.Data.ExternalVelocityCheck.ToString();

			} else {

				preCheckData.cgmVelocityCheck = "Not Checked";
			}

			preCheckData.stopForVelocityFailure = Convert.ToBoolean(ConfigurationManager.AppSettings["StopForVelocityFailure"]) && preCheckData.cgmVelocityCheck == "FAIL";

			if (ConfigurationManager.AppSettings["LimitCATransferFulfillType"] == "1" || preCheckData.stopForVelocityFailure) {
				var lifelineApplicationService = new LifelineApplicationDataService(ExternalApi.California, LoggedInUserId);
				var checkStatusRequestData = model.ToCaliforniaPrecheckCheckStatusRequestData();

				var checkStatusResult = await lifelineApplicationService.CheckCustomerStatusAsync(checkStatusRequestData);
				if (!checkStatusResult.IsSuccessful || (checkStatusResult.Data.Errors != null && checkStatusResult.Data.Errors.Count > 0)) {
					processingResult.IsSuccessful = false;

					var fullerror = "";
					foreach (var error in checkStatusResult.Data.Errors) {
						fullerror += error + "<br>";
					}
					processingResult.Error = new ProcessingError("", fullerror, false, true);

					return Ok(processingResult);
				} else {
					preCheckData.dapAccountType = checkStatusResult.Data.EnrollmentType.ToString();
				}
			} else {
				preCheckData.dapAccountType = "Not Checked";
			}

			preCheckData.delayedFulfillmentQueue = (ConfigurationManager.AppSettings["LimitCATransferFulfillType"] == "1" && preCheckData.dapAccountType != "New");
			if (preCheckData.stopForVelocityFailure) {
				if (preCheckData.dapAccountType == EnrollmentType.Transfer.ToString()) {
					preCheckData.stopForVelocityFailureMsg = "Applicant must call current Lifeline carrier to cancel existing service prior to initiating and completing the application process.  Once existing service with current Lifeline carrier has been canceled, applicant may call Budget Mobile’s Customer Call Center at 1-844-840-3733 to initiate and complete the application process.  Handset will be shipped within 5-7 business days after account has been approved.";
				} else {
					preCheckData.stopForVelocityFailureMsg = "Applicant must call Budget Mobile’s Customer Call Center at 1-844-840-3733 to initiate and complete the application process.  Handset will be shipped within 5-7 business days after the application has been approved.";
				}
			}

			processingResult.Data = preCheckData;
			processingResult.IsSuccessful = true;
			return Ok(processingResult);

		}


		//using CaliforniaPrecheckBindingModel because it is the first call to be made from Js side. Convert to CGMCheckRequest after hitting WebAPi.
		//public async Task<ServiceProcessingResult<CGMCovertedResponse>> cgmCheck(CGMCheckRequest model) {

		//    var processingResult = new ServiceProcessingResult<CGMCovertedResponse> { IsSuccessful = true };

		//    var CGMApplicationService = new CGMService();
		//    var result = await CGMApplicationService.Check(model);
		//    if (!result.IsSuccessful) {
		//        processingResult.IsSuccessful = false;
		//        processingResult.Error = result.Error;
		//        processingResult.Data = null;
		//        return processingResult;
		//    }
		//    processingResult.Data = new CGMCovertedResponse();

		//    processingResult.Data.Blacklist = result.Data.Blacklist;
		//    processingResult.Data.Transactionid = result.Data.Transactionid;
		//    processingResult.Data.Token = result.Data.Token;
		//    processingResult.Data.Status = result.Data.Status;
		//    processingResult.Data.Message = result.Data.Message;
		//    processingResult.Data.ValidationErrors = result.Data.ValidationErrors;
		//    processingResult.Data.ExternalVelocityCheck = result.Data.ExternalVelocityCheck;
		//    processingResult.Data.SubscriberCheck = result.Data.SubscriberCheck;
		//    processingResult.Data.ExternalVelocityCheck = "PASS";

		//    foreach (var check in result.Data.SubscriberCheck) {
		//        switch (check.PeriodDays) {
		//            case 1:
		//            //if (check.Matches >= 1) { processingResult.Data.ExternalVelocityCheck = "FAIL"; }
		//            //break;
		//            case 7:
		//            //if (check.Matches >= 1) { processingResult.Data.ExternalVelocityCheck = "FAIL"; }
		//            //break;
		//            case 30:
		//            //if (check.Matches >= 2) { processingResult.Data.ExternalVelocityCheck = "FAIL"; }
		//            //break;
		//            case 60:
		//            //if (check.Matches >= 2) { processingResult.Data.ExternalVelocityCheck = "FAIL"; }
		//            //break;
		//            case 90:
		//                if (check.Matches >= 1) { processingResult.Data.ExternalVelocityCheck = "FAIL"; }
		//                break;
		//                //took out new rules change 2/11/2016 per email Greg McKervey 
		//                //case 365:
		//                //    if (check.Matches >= 3) { processingResult.Data.ExternalVelocityCheck = "FAIL"; }
		//                //    break;
		//        }
		//    }

		//    processingResult.IsSuccessful = true;
		//    return processingResult;
		//}
		//public async Task<ServiceProcessingResult<CGMEnrollResponse>> cgmEnroll(CGMEnrollRequest cgmEnrollModel) {

		//    var processingResult = new ServiceProcessingResult<CGMEnrollResponse> { IsSuccessful = true };

		//    var CGMApplicationService = new CGMService();
		//    var Result = await CGMApplicationService.Enroll(cgmEnrollModel);

		//    if (!Result.IsSuccessful) {
		//        processingResult.IsSuccessful = false;
		//        processingResult.Error = Result.Error;
		//        processingResult.Data = null;
		//        return processingResult;
		//    }

		//    processingResult.Data = Result.Data;
		//    processingResult.IsSuccessful = true;
		//    return processingResult;
		//}

		[HttpGet]
		[Route("generateImageUploadCode")]
		public async Task<IHttpActionResult> GenerateImageUploadCode()
		{
			var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };

			var imageUploadDataService = new ImageUploadDataService();
			var imageCodeResult = await imageUploadDataService.GenerateImageUploadForProofImageAsync(GetLoggedInUserCompanyId(), LoggedInUserId, "app");//default for uploadapp
			if (!imageCodeResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = imageCodeResult.Error;
				return Ok(processingResult);
			}
			var imageCode = imageCodeResult.Data.ImageCode;

			processingResult.Data = imageCode;
			return Ok(processingResult);
		}

		[HttpPost]
		[Route("uploadBase64ProofImage")]
		public async Task<IHttpActionResult> UploadBase64ProofImage(WebcamUploadBindingModel model)
		{
			var processingResult = new ServiceProcessingResult<ImageUpload>();
			var companyId = GetLoggedInUserCompanyId();

			var imageUploadService = new ImageUploadDataService();
			var createUploadResult =
				await imageUploadService.GenerateImageUploadForProofImageAsync(companyId, LoggedInUserId, model.UpLoadType);
			if (!createUploadResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = createUploadResult.Error;
				return Ok(processingResult);
			}
			var imageUploadObject = createUploadResult.Data;

			var byteArray = Convert.FromBase64String(model.Base64EncodedImage);
			var uploadImageResult = await imageUploadService.UploadImageAsync(byteArray, model.DeviceDetails, imageUploadObject.Id, ImageExts.jpg.ToString());
			if (!uploadImageResult.IsSuccessful) {
				processingResult.IsSuccessful = false;
				processingResult.Error = uploadImageResult.Error;
				return Ok(processingResult);
			}

			return Ok(createUploadResult);
		}

		[HttpGet]
		[Route("verifyMobileUpload")]
		public async Task<IHttpActionResult> VerifyMobileUpload(string imageCode)
		{
			var imageUploadService = new ImageUploadDataService();
			var result = await imageUploadService.VerifyImageHasBeenUploadedAsync(imageCode);
			return Ok(result);

		}
		public ServiceProcessingResult<string> DeviceRetrievalValidationChecks(GetTFVerifyInfoRequestBindingModel model)
		{
			var processingResult = new ServiceProcessingResult<string>();



			return processingResult;
		}

		[HttpPost]
		[Route("getTFVerifyInfo")]
		public async Task<IHttpActionResult> getTFVerifyInfo(GetTFVerifyInfoRequestBindingModel model) {
			var processingResult = new ServiceProcessingResult<DeviceDetailResponse> { IsSuccessful = true, };
			if (!ModelState.IsValid) {
				var userHelp = GetModelStateErrorsAsString(ModelState);
				processingResult.Error = new ProcessingError(OrderValidationFailedUserMessage, userHelp, false, true);
				return Ok(processingResult);
			}
			var regex = "^([a-zA-Z]+[,.]?[ ]?|[a-zA-Z]+['-]?)+$";
			if (!Regex.IsMatch(model.FirstName, regex, RegexOptions.IgnoreCase) || !Regex.IsMatch(model.LastName, regex, RegexOptions.IgnoreCase)) {
				processingResult.Error = new ProcessingError("Invalid character in name.", "Invalid character in name.", false, true);
				return Ok(processingResult);

			}
			var validatedAddress = new DeviceAddressValidationResponse() {
				Address1 = model.Address1,
				Address2 = model.Address2,
				City = model.City,
				State = model.State,
				Zip = model.Zip,
				AddressBypassAvailable = false
			};
			var shippingValidatedAddress = new DeviceAddressValidationResponse()
			{
				Address1 = model.ShippingAddress1,
				Address2 = model.ShippingAddress2,
				City = model.ShippingCity,
				State = model.ShippingState,
				Zip = model.ShippingZip,

			};

			var returnData = new DeviceDetailResponse() {
				TFVerifyData = null,
				ValidatedAddress = validatedAddress,
				ShippingValidatedAddress=shippingValidatedAddress
				
			};

			var companySettingsResult =await new CompanyDataService().GetCompanyAsync(LoggedInUser.CompanyId);
			if (!companySettingsResult.IsSuccessful)
			{
				processingResult.IsSuccessful = false;
				processingResult.Error = companySettingsResult.Error;
				return Ok(processingResult);
			}
			var companyData =(Company) companySettingsResult.Data;






			//Company Setting: Address Validation/Scrub
			if (companyData.DoAddressScrub)
			{
				var validateAddressService = new ValidateAddressDataService();
				var validateAddressResult = await validateAddressService.ValidateAddressesAsync(model);
				//Check validateAddressResult.IsSuccessful.  If success, set model address fields to returned values.  Update Temp Order row?
				if (!validateAddressResult.IsSuccessful)
				{
					processingResult.IsSuccessful = validateAddressResult.IsSuccessful;
					validatedAddress.AddressBypassAvailable = false;
					returnData.ValidatedAddress = validatedAddress;
					processingResult.Data = returnData;

					var fullerror = "";

					foreach (var error in validateAddressResult.Data.Errors)
					{
						fullerror += error;
						fullerror += "<br>";
					}
					processingResult.Error = new ProcessingError("", fullerror, false, true);
					return Ok(processingResult);
				}

				if (validateAddressResult.Data.Errors.Count > 0)
				{
					//Address validation bypass not available at this time so hard coded to false. 
					validatedAddress.AddressBypassAvailable = false;
					validatedAddress.IsValid = validateAddressResult.Data.ServiceAddressIsValid;
					returnData.ValidatedAddress = validatedAddress;
					processingResult.Data = returnData;

					var fullerror = "";

					foreach (var error in validateAddressResult.Data.Errors)
					{
						fullerror += error;
						fullerror += "<br>";
					}
					processingResult.Error = new ProcessingError(fullerror, fullerror, false, true);
					return Ok(processingResult);
				}

				//Update model to make sure we have the correct addresses from here on out
				validatedAddress.City = validateAddressResult.Data.ServiceAddressCity;
				validatedAddress.State = validateAddressResult.Data.ServiceAddressState;
				validatedAddress.Address1 = validateAddressResult.Data.ServiceAddressStreet1;
				validatedAddress.Address2 = validateAddressResult.Data.ServiceAddressStreet2;
				validatedAddress.Zip = validateAddressResult.Data.ServiceAddressZip;
				validatedAddress.AddressBypassAvailable = validateAddressResult.Data.AllowServiceAddressBypass;
				returnData.ValidatedAddress = validatedAddress;
				//Set this at the start. If true we need the values later on to show by passes if both need shown if false they will be set to true later on if a bypass is needed.
				returnData.ValidatedAddress.AddressBypassAvailable = model.HasServiceAddressBypass;

				returnData.ShippingValidatedAddress = new DeviceAddressValidationResponse();
				returnData.ShippingValidatedAddress.Address1 = validateAddressResult.Data.ShippingAddressStreet1;
				returnData.ShippingValidatedAddress.Address2 = validateAddressResult.Data.ShippingAddressStreet2;
				returnData.ShippingValidatedAddress.City = validateAddressResult.Data.ShippingAddressCity;
				returnData.ShippingValidatedAddress.State = validateAddressResult.Data.ShippingAddressState;
				returnData.ShippingValidatedAddress.Zip = validateAddressResult.Data.ShippingAddressZip.Substring(0, 5);
			}//end address scrub

			//Company Setting: Address Velocity Check
			if (companyData.DoWhiteListCheck)
			{
				//Check if shelter using Local AddressValidation Table
				string sqlQuery = "Select * from ADDRESSVALIDATIONS WHERE Street1=@Street1 AND Street2=@Street2 AND City=@City and State=@State AND Zipcode=@Zipcode";
				SqlParameter[] addressparameters = new SqlParameter[] {
				new SqlParameter("@Street1", validatedAddress.Address1),
				new SqlParameter("@Street2", validatedAddress.Address2 ?? (object) DBNull.Value),
				new SqlParameter("@City", validatedAddress.City),
				new SqlParameter("@State", validatedAddress.State),
				new SqlParameter("@Zipcode", validatedAddress.Zip)
			};

				var addressSqlqueryService = new SQLQuery();
				var whiteListAddressResult = await addressSqlqueryService.ExecuteReaderAsync<AddressValidation>
					(CommandType.Text, sqlQuery, addressparameters);
				if (!whiteListAddressResult.IsSuccessful)
				{
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("There was an error checking shelter addresses.", "There was an error checking for shelter addresses.", true, false);
					return Ok(processingResult);
				}


				var whiteListAddresses = (List<AddressValidation>)whiteListAddressResult.Data;
				if (whiteListAddresses == null || whiteListAddresses.Count == 0 || (whiteListAddresses.Count == 1 && !whiteListAddresses[0].IsShelter))
				{
					sqlQuery = "SELECT COUNT(*) AS NumMatchingOrders FROM Orders WHERE ServiceAddressStreet1=@Street1 AND ServiceAddressStreet2=@Street2 AND ServiceAddressCity=@City and ServiceAddressState=@State AND ServiceAddressZip=@Zipcode AND DateCreated>=@TodayStart AND DateCreated<=@TodayEnd AND TenantAccountID IS NOT NULL";
					SqlParameter[] orderaddressparameters1 = new SqlParameter[] {
					new SqlParameter("@Street1",  validatedAddress.Address1),
					new SqlParameter("@Street2",  validatedAddress.Address2 ?? (object) DBNull.Value),
					new SqlParameter("@City",  validatedAddress.City),
					new SqlParameter("@State",  validatedAddress.State),
					new SqlParameter("@Zipcode",validatedAddress.Zip),
					new SqlParameter("@TodayStart",DateTime.Now.Date),
					new SqlParameter("@TodayEnd", DateTime.Now.Date.AddDays(1))
				};

					var orderSqlqueryService = new SQLQuery();
					var orderAddressResult = await orderSqlqueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, orderaddressparameters1);
					var orderCount = Convert.ToInt32(orderAddressResult.Data.Rows[0]["NumMatchingOrders"]);
					int vSubmissions = 0;// allow all submissions to go if not reset.
					var a = int.TryParse(ConfigurationManager.AppSettings["AddressSubmissions"].ToString(), out vSubmissions);
					if (orderCount >= vSubmissions)
					{//If this same address has been used at least 2 times today, don't allow the order
						var errMsg = "The address you submitted has already been submitted by a Safelink representative and is not currently registered as a shelter.  If the address you submitted is a confirmed shelter, please contact your supervisor to have the shelter address registered and added to the shelter list.";
						processingResult.IsSuccessful = false;
						processingResult.Error = new ProcessingError(errMsg, errMsg, true, false);
						return Ok(processingResult);
					}
				}
			}

			try {
				var solixService = new SolixAPI();

				var tfVerifyRequest = new SolixAPITracFoneVerificationRequest {
					CreateUserId = LoggedInUser.ExternalUserID,
					FirstName = model.FirstName,
					LastName = model.LastName,
					ResidentialAddress1 = returnData.ValidatedAddress.Address1,
					ResidentialAddress2 = returnData.ValidatedAddress.Address2,
					ResidentialCity = returnData.ValidatedAddress.City,
					ResidentialState = returnData.ValidatedAddress.State,
					ResidentialZip = returnData.ValidatedAddress.Zip,
					SsnLast4 = model.SsnLast4
				};

				var tfVerifyResult = await solixService.TracFoneVerification(tfVerifyRequest, Convert.ToDateTime(model.Dob));

				if (!tfVerifyResult.IsSuccessful) {
					processingResult.IsSuccessful = false;
					processingResult.Error = tfVerifyResult.Error;
					return Ok(processingResult);
				}

				var tfVerifyData = new TFVerifyResponseBindingModel() {
					IsDuplicate = tfVerifyResult.Data.IsDuplicate,
					IsLxNxVerified = tfVerifyResult.Data.IsLxNxVerified,
					LxnxId = tfVerifyResult.Data.LxnxId,
					LxnxTransactionId = tfVerifyResult.Data.LxnxTransactionId,
					LxnxNameAddressSSNSummary = tfVerifyResult.Data.LxnxNameAddressSSNSummary,
					LxnxRiskIndicators = tfVerifyResult.Data.LxnxRiskIndicators,
					IsBYOPAvailable = tfVerifyResult.Data.IsBYOPAvailable,
					IsRequalification = tfVerifyResult.Data.IsRequalification,
					PreviousMDN = tfVerifyResult.Data.PreviousMDN,
					PreviousEnrollmentId = tfVerifyResult.Data.PreviousEnrollmentId,
					AgentCommission = tfVerifyResult.Data.AgentCommission,
					IsFreePhoneEligible = tfVerifyResult.Data.IsFreePhoneEligible,
					ByopPricePlanEnglish = tfVerifyResult.Data.ByopPricePlanEnglish,
					ByopPricePlanSpanish = tfVerifyResult.Data.ByopPricePlanSpanish,
					NewCustomerPricePlanEnglish = tfVerifyResult.Data.NewCustomerPricePlanEnglish,
					NewCustomerPricePlanSpanish = tfVerifyResult.Data.NewCustomerPricePlanSpanish,
					ReturningCustomerPricePlanEnglish = tfVerifyResult.Data.ReturningCustomerPricePlanEnglish,
					ReturningCustomerPricePlanSpanish = tfVerifyResult.Data.ReturningCustomerPricePlanSpanish,
					AgentNoCommissionMessageEnglish = tfVerifyResult.Data.AgentNoCommissionMessageEnglish,
					AgentNoCommissionMessageSpanish = tfVerifyResult.Data.AgentNoCommissionMessageSpanish,
					IsBlacklistedAddress = tfVerifyResult.Data.IsBlacklistedAddress,
					IsBlacklistedSSN = tfVerifyResult.Data.IsBlacklistedSSN,
					IsError = tfVerifyResult.Data.IsError,
					ErrorMessage = tfVerifyResult.Data.ErrorMessage,
				};

				//get carriers and codes and put them into a list of objects
				var vCarriers = tfVerifyResult.Data.ByopCarriers;
				vCarriers = vCarriers.Replace("|", ",");
				var vCodes = tfVerifyResult.Data.ByopCarrierCodes;
				vCodes = vCodes.Replace("|", ",");
				//var vCarriers = "ATT Compatible Phone|T-Mobile Compatible Phone|GSM Unlocked Phone";
				//var vCodes = "ATT|TMO|OTHER";

				var listCarriers = vCarriers.Split(',').ToList();
				var listCodes = vCodes.Split(',').ToList();
				List<ReturnedCarriers> Carriers = new List<ReturnedCarriers>();
				for (int i = 0; i < listCarriers.Count; i++) {
					ReturnedCarriers element = new ReturnedCarriers();
					element.Name = listCarriers[i].ToString();
					element.Code = listCodes[i].ToString();
					Carriers.Add(element);
				}
				tfVerifyData.ByopCarriers = Carriers;
				tfVerifyData.LxnxRiskIndicators = tfVerifyResult.Data.LxnxRiskIndicators.Replace("|", ",");
				string tpivCode = null;
				string tpivRiskIndicators = null;
				string tpivNasScore = null;
				string tpivTransactionID = null;
				returnData.TFVerifyData = tfVerifyData;
				//Set this at the start. If true we need the values later on to show by passes if both need shown if false they will be set to true later on if a bypass is needed.
				returnData.TFVerifyData.TpivBypassAvailable = model.TpivBypass;
				returnData.TFVerifyData.TpivBypassFailureType = model.TpivBypassType;
				if (tfVerifyResult.Data.IsError) {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("SOLIX - " + tfVerifyResult.Data.ErrorMessage, "SOLIX - " + tfVerifyResult.Data.ErrorMessage, false, false);
					processingResult.Data = returnData;
					return Ok(processingResult);
				}

				if (tfVerifyResult.Data.IsDuplicate == "Y") {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("SOLIX - Duplicate subscriber found.", "SOLIX - Duplicate subscriber found.", false, true);
					returnData.TFVerifyData.TpivBypassAvailable = false;
					processingResult.Data = returnData;
					return Ok(processingResult);
				}
				if (tfVerifyResult.Data.IsBlacklistedAddress == "Y") {
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("SOLIX - This address is not allowed to be used.", "SOLIX - This address is not allowed to be used.", false, true);
					returnData.TFVerifyData.TpivBypassAvailable = false;
					processingResult.Data = returnData;
					return Ok(processingResult);
				}

				var SSNBypassNeeded = false;
				var DOBBypassNeeded = false;
				var HaveTPIVBypass_SSN = model.TpivBypass && !string.IsNullOrEmpty(model.TpivBypassSsnDocumentId);
				var HaveTPIVBypass_DOB = model.TpivBypass && !string.IsNullOrEmpty(model.TpivBypassDobDocumentId);
				var riskIndicatorsList = tfVerifyResult.Data.LxnxRiskIndicators.Split('|');

				if (tfVerifyResult.Data.IsLxNxVerified == "N") {
					var convertedNameAddressSSNSummary = 0;
					try {
						convertedNameAddressSSNSummary = Convert.ToInt32(tfVerifyResult.Data.LxnxNameAddressSSNSummary);
					} catch { }

					var NASScoreValid = new int[] { 7, 9, 10, 11, 12 }.Contains(convertedNameAddressSSNSummary);
					var RiskIndicatorsValid_SSN = true;
					var RiskIndicatorsValid_DOB = true;

					foreach (var RiskCode in riskIndicatorsList) {
						if (new[] { "2", "3", "6", "79", "CL", "RS" }.Contains(RiskCode)) {
							RiskIndicatorsValid_SSN = false;
						}
						if (new[] { "81", "83" }.Contains(RiskCode)) {
							RiskIndicatorsValid_DOB = false;
						}
					}
					SSNBypassNeeded = !(NASScoreValid && RiskIndicatorsValid_SSN);
					DOBBypassNeeded = !(RiskIndicatorsValid_DOB);

					if (!SSNBypassNeeded && !DOBBypassNeeded) {
						SSNBypassNeeded = true;
						DOBBypassNeeded = true;
					} //If we don't determine that one or the other failed, then we have to default to both failing
				}

				foreach (var RiskCode in riskIndicatorsList) {
					if (new[] { "50", "PO" }.Contains(RiskCode)) {
						var errorMessage = "Lexis Nexis: The address is not a valid address for service";
						ProcessingError error = new ProcessingError(errorMessage, errorMessage, false, true);
						processingResult.Error = error;
						processingResult.Data = returnData;
						return Ok(processingResult);
					}
				}

				if (tfVerifyResult.Data.LxnxId == "" || tfVerifyResult.Data.LxnxNameAddressSSNSummary.ToString() == "" || tfVerifyResult.Data.LxnxTransactionId == "") {
					var errorMessage1 = "SOLIX - Unable to retrieve Lexis Nexis info";
					ProcessingError error = new ProcessingError(errorMessage1, errorMessage1, false, true);
					ExceptionlessClient.Default.CreateException(new Exception("Solix LxNx check result is empty.")).Submit();
					processingResult.Error = error;
					processingResult.Data = returnData;
					return Ok(processingResult);
				}

				if ((!SSNBypassNeeded && !DOBBypassNeeded) && returnData.TFVerifyData.IsBlacklistedSSN != "Y") { //Everything is OK
					tpivCode = tfVerifyResult.Data.LxnxId;
					tpivRiskIndicators = string.Join(",", riskIndicatorsList);
					tpivNasScore = tfVerifyResult.Data.LxnxNameAddressSSNSummary.ToString();
					tpivTransactionID = tfVerifyResult.Data.LxnxTransactionId;
				} else if ((!SSNBypassNeeded || HaveTPIVBypass_SSN) && (!DOBBypassNeeded || HaveTPIVBypass_DOB) && returnData.TFVerifyData.IsBlacklistedSSN != "Y") { //This failure has been bypassed
					tpivCode = tfVerifyResult.Data.LxnxId;
					tpivRiskIndicators = string.Join(",", riskIndicatorsList);
					tpivNasScore = tfVerifyResult.Data.LxnxNameAddressSSNSummary.ToString();
					tpivTransactionID = tfVerifyResult.Data.LxnxTransactionId;
				} else {
					processingResult.IsSuccessful = true;
					var errorMessage = "";
					if (returnData.TFVerifyData.IsBlacklistedSSN == "Y" && !SSNBypassNeeded) {
						SSNBypassNeeded = true;
					}
					if ((SSNBypassNeeded && DOBBypassNeeded)) {
						returnData.TFVerifyData.TpivBypassFailureType = "BOTH";
						errorMessage = "Lexis Nexis: Could not match SSN and DOB";
					} else if (SSNBypassNeeded) {
						errorMessage = "Lexis Nexis: Could not match SSN";
						returnData.TFVerifyData.TpivBypassFailureType = "SSN";
					} else if (DOBBypassNeeded) {
						errorMessage = "Lexis Nexis: Could not match DOB";
						returnData.TFVerifyData.TpivBypassFailureType = "DOB";
					}
					ProcessingError error = new ProcessingError(errorMessage, errorMessage, false, true);
					var applicationUserDataService = new ApplicationUserDataService();
					var getResult = applicationUserDataService.Get(LoggedInUserId);
					if (!getResult.IsSuccessful) {
						//Logger.Error("Failed to get user with Id of " + LoggedInUserId + " during TPIV permission check");
						ExceptionlessClient.Default.CreateLog(typeof(OrderController).FullName, "Failed to get user with Id of " + LoggedInUserId + " during TPIV permission check", "Error").AddTags("Controller Error").Submit();
						returnData.TFVerifyData.TpivBypassAvailable = false;
					} else {
						returnData.TFVerifyData.TpivBypassAvailable = getResult.Data.PermissionsBypassTpiv;
					}
					processingResult.Error = error;
					processingResult.Data = returnData;
					return Ok(processingResult);
				}
			} catch (Exception ex) {
				ex.ToExceptionless()
				.SetMessage("Solix API Error - Function:LexisNexisVerification")
				.MarkAsCritical()
				.Submit();
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("Error performing TracFone validations.", "Error performing TracFone validations.", true, false);
				processingResult.Data = returnData;
				return Ok(processingResult);

			}

			processingResult.Data = returnData;

			return Ok(processingResult);
		}

		



	}

}

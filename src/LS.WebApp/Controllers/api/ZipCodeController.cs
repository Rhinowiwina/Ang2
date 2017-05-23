using System;
using System.Threading.Tasks;
using System.Web.Http;
using LS.ApiBindingModels;
using LS.Core;
using LS.Services;
using LS.Services.Factories;
using LS.WebApp.CustomAttributes;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api {
    //        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/zipCode")]
    public class ZipCodeController : BaseApiController {
        public static readonly string ZipCodeFailedUserMessage = "An error occurred retrieving info based on Zip Code.";

        public async Task<IHttpActionResult> Get(string zipCode, string salesTeamId) {
            var processingResult = new ServiceProcessingResult<ZipCodeResponseBindingModel> { IsSuccessful = true };
            var sqlQueryTime = new SQLQuery();
            var queryString = "SELECT OrderStart, OrderEnd, TimeZone FROM Companies where Id=@Id ";

            SqlParameter[] timeparameters = new SqlParameter[] {
                new SqlParameter("@Id",LoggedInUser.CompanyId)
            };
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

            if (!ModelState.IsValid) {
                var userHelp = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(ZipCodeFailedUserMessage, userHelp, false, true);
                return Ok(processingResult);
            }

            var a = LoggedInUser;
            if (string.IsNullOrEmpty(LoggedInUser.ExternalUserID)) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("External User ID is missing.", "External User ID is missing.", true, false);
                return Ok(processingResult);
            }

            var companyId = GetLoggedInUserCompanyId();

            var appUserService = new ApplicationUserDataService();
            var validateUserIsActiveResult = await appUserService.ValidateUserCanBeginOrder(LoggedInUserId, companyId);
            if (!validateUserIsActiveResult.IsSuccessful || !validateUserIsActiveResult.Data.IsValid) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validateUserIsActiveResult.IsSuccessful
                    ? ErrorValues.CANNOT_BEGIN_ORDER_WITH_INACTIVE_USER_ERROR
                    : validateUserIsActiveResult.Error;
                if (validateUserIsActiveResult.IsFatalFailure()) {
                    //var logMessage =
                    //    String.Format(
                    //        "A fatal error occurred while trying to validate that User with Id: {0} is active.",
                    //        LoggedInUserId);
                    //Logger.Fatal(logMessage);

                    //ExceptionlessClient.Default.CreateLog(typeof(SalesTeamController).FullName, String.Format(
                    //                           "A fatal error occurred while trying to validate that User with Id: {0} is active.",
                    //                           LoggedInUserId), "Error").AddTags("Controller Error").Submit();
                }
                return Ok(processingResult);
            }

            var getUserResult = await appUserService.GetAsync(LoggedInUserId);
            if (!getUserResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return Ok(processingResult);
            }
            var user = getUserResult.Data;
            var loggedInUserName = user.FirstName + " " + user.LastName;

            var salesTeamDataService = new SalesTeamDataService(LoggedInUserId);
            var salesTeamResult = salesTeamDataService.Get(salesTeamId);
            if (!salesTeamResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_TEAM_IN_COMPANY_ERROR;
                return Ok(processingResult);
            }
            var salesTeam = salesTeamResult.Data;

            // ZIP CODE
            var orderDataService = new OrderDataService();
            processingResult = await orderDataService.GetRelevantInfoForOrderAsync(zipCode, companyId, GetLoggedInUserFullName());
            if (!processingResult.IsSuccessful) { return Ok(processingResult); }

            if (processingResult.Data.LifelinePrograms.Count == 0) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.NOLIFELINE_PROGRAMS_AVAILABLE_ERROR;
                return Ok(processingResult);
            }

            // ZIP CODE Coverage Tracfone ZipcodeCoverage
            var sqlQuery = "SELECT PostalCode FROM ZipcodeCoverage WHERE PostalCode=@Zipcode";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Zipcode", zipCode) };

            var sqlQueryService = new SQLQuery();
            var queryResults = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!queryResults.IsSuccessful || queryResults.Data.Rows.Count == 0) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Zipcode does not have coverage (Tracfone).", "The zip code you entered (" + zipCode + ") does not have cell phone coverage in our network.", false);
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }
    }
}

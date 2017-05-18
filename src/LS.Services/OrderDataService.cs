using LS.Domain;
using LS.Core;
using System;
using LS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using LS.ApiBindingModels;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Exceptionless;
namespace LS.Services
{
    public class OrderDataService : BaseDataService<Order, string>
    {
        private static readonly string ProgramProofType = "program";
        private static readonly string IdProofType = "id";
        private static readonly string SubstituteCompetitorsString = "%%COMPETITORS%%";
        private static readonly string AddressBypassProofType = "ADDR";
        private static readonly string SsnTpivProofType = "SSN";
        private static readonly string DobTpivProofType = "DOB";
        private static readonly string AllTpivProofType = "All";

        public override BaseRepository<Order, string> GetDefaultRepository()
        {
            return new OrderRepository();
        }

        public async Task<ServiceProcessingResult<List<Order>>> GetWhereAsync(string UserID)
        {
            var result = new ServiceProcessingResult<List<Order>>();

            result = await GetAllWhereAsync(p => p.Id == UserID);

            return result;
        }

        public async Task<ServiceProcessingResult<List<Order>>> GetOrderByTenantAccountID(string TenantAccountId)
        {
            var result = new ServiceProcessingResult<List<Order>>();

            result = await GetAllWhereAsync(p => p.TenantAccountId == TenantAccountId);

            return result;
        }


        public async Task<ServiceProcessingResult<List<Order>>> GetOrderByID(string OrderID)
        {
            var result = new ServiceProcessingResult<List<Order>>();

            result = await GetAllWhereAsync(p => p.Id == OrderID);

            return result;
        }

        public async Task<ServiceProcessingResult<bool>> TransactionIDExistInOrders(string TransactionID)
        {
            var result = new ServiceProcessingResult<bool> { IsSuccessful = true };

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var sqlString = "SELECT TOP 1 Id FROM Orders (NOLOCK) WHERE TransactionId=@TransactionId AND IsDeleted=0";
            SqlCommand cmd = new SqlCommand(sqlString, connection);
            SqlDataReader rdr = null;

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@TransactionId", TransactionID));

            try {
                connection.Open();
                rdr = cmd.ExecuteReader();
                result.Data = rdr.HasRows;
                connection.Close();
            } catch (Exception ex) {
                result.IsSuccessful = false;
                ex.ToExceptionless()
                     .SetMessage("Error getting with transactionid query TransactionID (" + TransactionID + ")")
                     .MarkAsCritical()
                     .Submit();
                //Logger.Fatal("Error getting with transactionid query TransactionID (" + TransactionID + ")");
                result.Error = new ProcessingError("Error validating order transaction id (" + TransactionID + ").", "Error validating order transaction id (" + TransactionID + ").", true, false);
            }

            return result;
        }


        public async Task<ServiceProcessingResult<Order>> GetOrderBySignatureID(string SignatureID)
        {
            var result = new ServiceProcessingResult<Order>();
            result = await GetWhereAsync(o => o.SigFileName == SignatureID);
            return result;
        }

        public async Task<ServiceProcessingResult<List<CompletedOrder>>> GetOrderForFulfillment(string UserID, string Filter, ICollection<string> locations, DateTime StartDate, DateTime EndDate)
        {
            var processingResult = new ServiceProcessingResult<List<CompletedOrder>> { IsSuccessful = true };
            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    var repository = new OrderRepository();
                    processingResult = repository.GetOrderForFulfillment(UserID, Filter, locations, StartDate, EndDate).ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful) {
                        //Logger.Error("An error occurred while retrieving orders for fulfillment.");
                        ExceptionlessClient.Default.CreateLog(typeof(OrderDataService).FullName, "An error occurred while retrieving orders for fulfillment.", "Error").AddTags("Data Service Error").Submit();
                        return processingResult;
                    }

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_UPDATE_SALES_TEAM_COMMISSIONS_ERROR;
                    ex.ToExceptionless()
                     .SetMessage("An error occurred while retrieving orders for fulfillment.")
                     .AddTags("Fulfillment Error")
                     .MarkAsCritical()
                     .Submit();
                    //Logger.Error("An error occurred while retrieving orders for fulfillment.", ex);
                    return processingResult;
                }
            }

            return processingResult;
        }

        public async Task<ServiceProcessingResult<Order>> ChangeFulfillmentType(Order order)
        {
            var processingResult = new ServiceProcessingResult<Order> { IsSuccessful = true };
            var result = await base.UpdateAsync(order);

            if (!result.IsSuccessful) {
                result.Error = new ProcessingError("Error in updating fulfillment type", "Error trying to change fulfillment type. Please contact support", true, false);
                processingResult.IsSuccessful = false;

                return processingResult;
            }

            processingResult.Data = result.Data;
            return processingResult;
        }
        public async Task<ServiceProcessingResult<string>> AddSolixValidationDetails(SolixValidationDetails model, string orderId)
        {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };
            var sqlQuery = new SQLQuery();
            var queryString = "INSERT INTO SolixValidationDetails (AgentCommission,IsByop,IsRequalification,RequalificationAppId,IsFreePhoneEligible,RequalificationMDN,OrderId) Values(@AgentCommission,@IsByop,@IsRequalification,@RequlificationAppId,@IsFreePhoneEligible,@RequalificationMDN,@OrderId)";
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@AgentCommission",model.AgentCommission),
                new SqlParameter("@IsByop",model.IsBYOP ?? "N"),
                new SqlParameter("@IsRequalification",model.IsRequalification ?? "N"),
                new SqlParameter("@RequlificationAppId",model.RequalificationAppId ?? ""),
                new SqlParameter("@IsFreePhoneEligible",model.IsFreePhoneEligible ?? "N"),
                new SqlParameter("@RequalificationMDN",model.RequalificationMDN ?? ""),
                new SqlParameter("@OrderId",orderId)
            };
            var queryResult = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, queryString, parameters);
            if (!queryResult.IsSuccessful || queryResult.Data == 0) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("There was an error saving solix validation results.", "There was an error saving solix validation results.", true, false);
                processingResult.Data = "sucess";
                return processingResult;
            }
            processingResult.Data = "sucess";
            return processingResult;

        }


        public async Task<ServiceProcessingResult<bool>> OrderExist(string orderId)

        {
            var processingResult = new ServiceProcessingResult<bool> { IsSuccessful = false, Data = false };
            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    var repository = new OrderRepository();
                    processingResult = repository.OrderExist(orderId).ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful) {
                        Logger.Error("An error occurred while checking if order exist.");
                        return processingResult;
                    }

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    ex.ToExceptionless()
                    .SetMessage("An error occurred while checking if order exist.")
                    .AddTags("Order Exist")
                    .MarkAsCritical()
                    .Submit();
                    //Logger.Error("An error occurred while checking if order exist.", ex);
                    return processingResult;
                }
            }

            return processingResult;


        }

        public async Task<ServiceProcessingResult<string>> UpdateTenantAccountInfo(string orderId, string tenantId)
        //return Successful or Failure
        {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = false, Data = "Failure" };
            using (var contextScope = DbContextScopeFactory.Create()) {
                try {
                    var repository = new OrderRepository();
                    processingResult = repository.UpdateTenantAccountInfo(orderId, tenantId).ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful) {
                        //Logger.Error("An error occurred while checking if order exist.");
                        ExceptionlessClient.Default.CreateLog(typeof(OrderDataService).FullName, "An error occurred while checking if order exist.", "Error").AddTags("Data Service Error").Submit();
                        return processingResult;
                    }

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    ex.ToExceptionless()
                     .SetMessage("An error occurred while updating tenant id.")
                     .AddTags("UpdateTenantAccountInfo")
                     .MarkAsCritical()
                     .Submit();
                    //Logger.Error("An error occurred while updating tenant id.", ex);
                    return processingResult;
                }
            }

            return processingResult;


        }
        public async Task<ServiceProcessingResult<ZipCodeResponseBindingModel>> GetRelevantInfoForOrderAsync(string zipCode, string companyId, string LoggedInUserFullName)
        {
            var processingResult = new ServiceProcessingResult<ZipCodeResponseBindingModel> { IsSuccessful = true };
            var returnObject = new ZipCodeResponseBindingModel();

            var zipCodeService = new ZipCodeDataService();
            var zipCodeResult = await zipCodeService.GetExistingZipCodesFromPostalCodeAsync(zipCode);
            if (!zipCodeResult.IsSuccessful) {
                processingResult.IsSuccessful = zipCodeResult.IsSuccessful;
                processingResult.Error = zipCodeResult.Error;
                return processingResult;
            }
            foreach (var record in zipCodeResult.Data) {
                if (record.StateAbbreviation != "CA") {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Postal code must be a California state postal code.", "Postalcode must be a California state postalcode.", true, false);
                    return processingResult;
                }
            }

            returnObject.ZipCodeOptions = zipCodeResult.Data;
            var stateCode = zipCodeResult.Data[0].StateAbbreviation;

            // BASE INCOME LEVELS
            var baseIncomeLevelsService = new BaseIncomeLevelsDataService();
            var baseIncomeLevelsResult = await baseIncomeLevelsService.GetBaseIncomeLevelsByStateCodeAsync(stateCode);
            if (!baseIncomeLevelsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = baseIncomeLevelsResult.Error;
                return processingResult;
            }
            returnObject.IncomeLevelPercentage = baseIncomeLevelsResult.Data.IncomeLevel;
            returnObject.BaseIncomeLevels = baseIncomeLevelsResult.Data;

            // COMPLIANCE STATEMENT
            var complianceStatementService = new ComplianceStatementDataService();
            var complianceStatementResult = await complianceStatementService.GetComplianceStatementByStateCodeAsync(stateCode);
            if (!complianceStatementResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = complianceStatementResult.Error;
                return processingResult;
            }

            returnObject.ComplianceStatement = complianceStatementResult.Data;

            // COMPETITORS
            var competitorService = new CompetitorDataService();
            var competitorsResult = await competitorService.GetCompetitorsByCompanyIdAndStateCode(companyId, stateCode);
            if (!competitorsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = competitorsResult.Error;
                return processingResult;
            }

            returnObject.Competitors = competitorsResult.Data;

            // PROOF DOCUMENT TYPES
            var proofDocumentService = new ProofDocumentTypeDataService();
            var proofDocumentsResult = await proofDocumentService.GetProofDocumentTypesByStateCodeAsync(stateCode);
            if (!proofDocumentsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = proofDocumentsResult.Error;
                return processingResult;
            }

            returnObject.ProgramProofDocumentTypes = proofDocumentsResult.Data.Where(pdt => pdt.ProofType == ProgramProofType).ToList();
            returnObject.IdentityProofDocumentTypes = proofDocumentsResult.Data.Where(pdt => pdt.ProofType == IdProofType).ToList();

            // SSN-DOB
            var tpivProofDocumentService = new TpivProofDocumentTypeDataService();
            var tpivProofDocumentsResult = await tpivProofDocumentService.GetByStateCode(stateCode);
            if (!tpivProofDocumentsResult.IsSuccessful) {
                if (tpivProofDocumentsResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving TPIV Proof Document Types for StateCode: {0}", stateCode);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(OrderDataService).FullName, String.Format("A fatal error occurred while retrieving TPIV Proof Document Types for StateCode: {0}", stateCode), "Error").AddTags("Data Service Error").Submit();
                }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_PROOF_DOCUMENT_TYPES_ERROR;
                return processingResult;
            }

            returnObject.SsnTpivProofDocumentTypes = tpivProofDocumentsResult.Data.Where(tpd => tpd.Type == SsnTpivProofType || tpd.Type == AllTpivProofType).ToList();
            returnObject.DobTpivProofDocumentTypes = tpivProofDocumentsResult.Data.Where(tpd => tpd.Type == DobTpivProofType || tpd.Type == AllTpivProofType).ToList();
            //Address Bypass Docs
            var addressProofDocumentService = new AddressBypassProofDocumentTypeDataService();
            var addressProofDocumentsResult = await addressProofDocumentService.GetByStateCode(stateCode);
            if (!addressProofDocumentsResult.IsSuccessful) {
                if (addressProofDocumentsResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving Address Proof Document Types for StateCode: {0}",stateCode);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(OrderDataService).FullName, String.Format("A fatal error occurred while retrieving Address Proof Document Types for StateCode: {0}", stateCode), "Error").AddTags("Data Service Error").Submit();
                }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_PROOF_DOCUMENT_TYPES_ERROR;
                return processingResult;
            }

            returnObject.AddressBypassProofDocumentTypes = addressProofDocumentsResult.Data.Where(apd => apd.Type == AddressBypassProofType || apd.Type == AllTpivProofType).ToList();
            // LIFELINE PROGRAMS
            var lifelineProgramService = new LifelineProgramDataService();
            var lifelineProgramsResult = await lifelineProgramService.GetLifelineProgramsByStateCodeAsync(stateCode);
            if (!lifelineProgramsResult.IsSuccessful || lifelineProgramsResult.Data == null) {
                if (lifelineProgramsResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving Lifeline Programs for StateCode: {0}", stateCode);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(OrderDataService).FullName, String.Format("A fatal error occurred while retrieving Lifeline Programs for StateCode: {0}", stateCode), "Error").AddTags("Data Service Error").Submit();
                }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_LIFELINE_PROGRAMS_ERROR;
                return processingResult;
            }

            returnObject.LifelinePrograms = lifelineProgramsResult.Data;

            if (lifelineProgramsResult.Data.Any(lp => lp.RequiredStateProgramId != null || lp.RequiredSecondaryStateProgramId != null)) {
                var requiredStateProgramIds = lifelineProgramsResult.Data.Where(lp => lp.RequiredStateProgramId != null).Select(lp => lp.RequiredStateProgramId).ToList();
                var requiredSecondaryStateProgramIds = lifelineProgramsResult.Data.Where(lp => lp.RequiredSecondaryStateProgramId != null).Select(lp => lp.RequiredSecondaryStateProgramId).ToList();
                var stateProgramIds = requiredStateProgramIds.Union(requiredSecondaryStateProgramIds).ToList();

                var stateProgramService = new StateProgramDataService();
                var stateProgramsResult = await stateProgramService.GetStateProgramsByListOfIds(stateProgramIds);
                if (!stateProgramsResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = stateProgramsResult.Error;
                    return processingResult;
                }

                returnObject.StatePrograms = stateProgramsResult.Data;
            }

            // STATE AGREEMENTS
            var stateAgreementService = new StateAgreementDataService();
            var stateAgreementsResult = await stateAgreementService.GetStateAgreementsByStateCodeAsync(companyId, stateCode);
            if (!stateAgreementsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = stateAgreementsResult.Error;
                return processingResult;
            }

            returnObject.AgreementStatements = stateAgreementsResult.Data.OrderByDescending(a => a.StateAgreementParentId).ThenBy(b => b.Agreement).ToList();

            // STATE SETTINGS
            var stateSettingsService = new StateSettingsDataService();
            var stateSettingsResult = await stateSettingsService.GetStateSettingsByStateCodeAsync(stateCode);
            if (!stateSettingsResult.IsSuccessful || stateSettingsResult.Data == null) {
                if (stateSettingsResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving State Settings for StateCode: {0}", stateCode);
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(OrderDataService).FullName, String.Format("A fatal error occurred while retrieving State Settings for StateCode: {0}", stateCode), "Error").AddTags("Data Service Error").Submit();
                }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_STATE_SETTINGS_ERROR;
                return processingResult;
            }

            returnObject.SsnType = stateSettingsResult.Data.SsnType;
            //  returnObject.IncomeLevelPercentage = stateSettingsResult.Data.IncomeLevel;

            // COMPANY OPTIONS
            var companyService = new CompanyDataService();
            var companyResult = companyService.Get(companyId);
            if (!companyResult.IsSuccessful || companyResult.Data == null) {
                if (companyResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving Company with Id: {0}", companyId);
                    ExceptionlessClient.Default.CreateLog(typeof(OrderDataService).FullName, String.Format("A fatal error occurred while retrieving Company with Id: {0}", companyId), "Error").AddTags("Data Service Error").Submit();
                    //Logger.Fatal(logMessage);
                }
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_COMPANY_ERROR;
                return processingResult;
            }

            returnObject.EmailRequired = companyResult.Data.EmailRequiredForOrder;
            returnObject.ContactPhoneRequired = companyResult.Data.ContactPhoneRequiredForOrder;

            processingResult.Data = returnObject;
            return processingResult;
        }
    }
}

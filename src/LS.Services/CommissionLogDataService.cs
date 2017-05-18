using System;
using System.Threading.Tasks;
using LS.Core;
using LS.Core.Interfaces.PayPal;
using LS.Domain;
using LS.Domain.PayPal;
using LS.Repositories;
using LS.Repositories.DBContext;
using LS.Utilities;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LS.PayPal;
using System.Data.Entity;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services {
    public class CommissionLogDataService : BaseDataService<CommissionLog, string> {
        private static readonly string EmailSubject = "Sched Task: Lifeline Services Commission Payment Error";
        private static readonly string EmailToAddress = "errors@spinlifeserv.com";


        public override BaseRepository<CommissionLog, string> GetDefaultRepository() {
            return new CommissionLogRepository();
        }

        public class CommissionResult {
            public string CommissionID { get; set; }
            public decimal Amount { get; set; }
            public string PaypalEmail { get; set; }
        }

        public class CommissionPaymentsResult {
            public string ContentString { get; set; }
            public string PayError { get; set; }
        }


        //public async Task<ServiceProcessingResult> CreateUserCommissionDetailsFor(string salesTeamId, string loggedInUserId, string orderId)
        //{
        //    var result = new ServiceProcessingResult();

        //    var salesTeamDataService = new SalesTeamDataService(loggedInUserId);

        //    var getSalesTeamResult = salesTeamDataService.Get(salesTeamId);
        //    if (!getSalesTeamResult.IsSuccessful)
        //    {
        //        result.IsSuccessful = false;
        //        result.Error = getSalesTeamResult.Error;
        //        return result;
        //    }

        //    var salesTeam = getSalesTeamResult.Data;

        //    if (salesTeam.CommissionLevel1UserId != null)
        //    {
        //        var level1AddResult = await AddAsync(new UserCommissionDetail
        //        {
        //            Amount = salesTeam.CommissionLevel1Amount,
        //            OrderId = orderId,
        //            UserId = salesTeam.CommissionLevel1UserId,
        //            SalesTeamId = salesTeam.Id
        //        });

        //        if (!level1AddResult.IsSuccessful)
        //        {
        //            result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
        //            result.IsSuccessful = false;
        //            return result;
        //        }
        //    }

        //    if (salesTeam.CommissionLevel2UserId != null)
        //    {
        //        var level2AddResult = await AddAsync(new UserCommissionDetail
        //        {
        //            Amount = salesTeam.CommissionLevel2Amount,
        //            OrderId = orderId,
        //            SalesTeamId = salesTeam.Id,
        //            UserId = salesTeam.CommissionLevel2UserId
        //        });

        //        if (!level2AddResult.IsSuccessful)
        //        {
        //            result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
        //            result.IsSuccessful = false;
        //            return result;
        //        }
        //    }

        //    if (salesTeam.CommissionLevel3UserId != null)
        //    {
        //        var level3AddResult = await AddAsync(new UserCommissionDetail
        //        {
        //            Amount = salesTeam.CommissionLevel3Amount,
        //            OrderId = orderId,
        //            UserId = salesTeam.CommissionLevel3UserId,
        //            SalesTeamId = salesTeam.Id
        //        });

        //        if (!level3AddResult.IsSuccessful)
        //        {
        //            result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
        //            result.IsSuccessful = false;
        //            return result;
        //        }
        //    }

        //    if (salesTeam.CommissionManagerUserId != null)
        //    {
        //        var managerAddResult = await AddAsync(new UserCommissionDetail
        //        {
        //            Amount = salesTeam.CommissionManagerAmount,
        //            OrderId = orderId,
        //            UserId = salesTeam.CommissionManagerUserId,
        //            SalesTeamId = salesTeam.Id
        //        });

        //        if (!managerAddResult.IsSuccessful)
        //        {
        //            result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
        //            result.IsSuccessful = false;
        //            return result;
        //        }
        //    }

        //    var salesTeamCommission = new UserCommissionDetail
        //    {
        //        Amount = salesTeam.CommissionAmount,
        //        SalesTeamId = salesTeam.Id,
        //        OrderId = orderId
        //    };

        //    var addResult = await AddAsync(salesTeamCommission);

        //    if (!addResult.IsSuccessful)
        //    {
        //        result.IsSuccessful = false;
        //        result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
        //        return result;
        //    }

        //    var dataService = new ApplicationUserDataService();
        //    var theUser = await dataService.GetAsync(loggedInUserId);

        //    var UserCommission = new UserCommissionDetail
        //    {
        //        Amount = theUser.Data.UserCommission,
        //        SalesTeamId = salesTeam.Id,
        //        OrderId = orderId,
        //        UserId = loggedInUserId
        //    };

        //    var userResult = await AddAsync(UserCommission);

        //    if (!userResult.IsSuccessful)
        //    {
        //        userResult.IsSuccessful = false;
        //        userResult.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
        //        return userResult;
        //    }

        //    result.IsSuccessful = true;
        //    return result;
        //}

        public async Task<ServiceProcessingResult<List<CommissionLog>>> InsertCommissionLog(List<ProductCommissions> ProductCommissions, string OrderID) {
            var result = new ServiceProcessingResult<List<CommissionLog>>();

            foreach (var commission in ProductCommissions) {

                var commissionLogDetails = new CommissionLog {
                    Amount = commission.Amount,
                    SalesTeamId = commission.SalesTeamID,
                    OrderId = OrderID,
                    RecipientUserId = commission.RecipientUserId,
                    RecipientType = commission.RecipientType,
                    OrderType = commission.ProductType
                };

                var commissionLogResult = await AddAsync(commissionLogDetails);

                if (!commissionLogResult.IsSuccessful) {
                    result.IsSuccessful = false;
                    //var errorMessage = commissionLogResult.Error.UserMessage + "RecipientUserId: " + commissionLogDetails.RecipientUserId + "RecipientType: " + commissionLogDetails.RecipientType + "SalesTeamID: " + commissionLogDetails.SalesTeamId + "Amount: " + commissionLogDetails.Amount;
                    result.Error = new ProcessingError("Error inserting commission log", "Error inserting commission log", false, false);

                    ExceptionlessClient.Default.CreateLog(typeof(CommissionLogDataService).FullName,"Error inserting commission log","Error").AddTags("Data Service Error").AddObject(commissionLogDetails).Submit();
                    return result;
                }
            }

            var insertedCommissions = await GetAllWhereAsync(pc => pc.OrderId == OrderID);
            if (!insertedCommissions.IsSuccessful) {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("Error retrieving commission log", "Error retrieving commission log", false, false);

                ExceptionlessClient.Default.CreateLog(typeof(CommissionLogDataService).FullName,"Error retrieving commission log","Error").Submit();
                return result;
            }

            result.IsSuccessful = true;
            result.Data = insertedCommissions.Data;

            return result;
        }
    }
}
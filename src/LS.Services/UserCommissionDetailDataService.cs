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

namespace LS.Services
{
    public class UserCommissionDetailDataService : BaseDataService<UserCommissionDetail, string>
    {
        private static readonly string EmailSubject = "Sched Task: Lifeline Services Commission Payment Error";
        private static readonly string EmailToAddress = "errors@spinlifeserv.com";


        public override BaseRepository<UserCommissionDetail, string> GetDefaultRepository()
        {
            return new UserCommissionDetailRepository();
        }

        public class CommissionResult
        {
            public string CommissionID { get; set; }
            public decimal Amount { get; set; }
            public string PaypalEmail { get; set; }
        }

        public class CommissionPaymentsResult
        {
            public string ContentString { get; set; }
        }

        public async Task<ServiceProcessingResult<CommissionPaymentsResult>> AggregateCommissions()
        {
            var processingResult = new ServiceProcessingResult<CommissionPaymentsResult>() { IsSuccessful = true };
            processingResult.Data = new CommissionPaymentsResult();
            processingResult.Data.ContentString = "";
            var emailHelper = new EmailHelper();
            try
            {
                var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionstring);
                var sqlString = @"
                SELECT C.Amount, T.PaypalEmail, C.Id AS CommissionID, C.OrderID
                FROM UserCommissionDetails C 
                     LEFT JOIN Orders O ON C.OrderID=O.Id 
                     LEFT JOIN SalesTeams T ON C.SalesTeamID=T.Id 
                WHERE O.TenantAccountID IS NOT NULL AND O.IsDeleted=0 
                     AND C.UserId IS NULL 
                     AND C.SalesTeamId IS NOT NULL AND COALESCE(T.PaypalEmail, '')!='' 
                     AND COALESCE(C.Amount, 0)>0 
                     AND C.PaymentID IS NULL
				UNION
					 SELECT C.Amount, U.PaypalEmail, C.Id AS CommissionID, C.OrderID
						 --, * 
					FROM UserCommissionDetails C 
						 LEFT JOIN Orders O ON C.OrderID=O.Id 
						 LEFT JOIN ASPNetUsers U ON C.UserID=U.Id 
					WHERE O.TenantAccountID IS NOT NULL AND O.IsDeleted=0 
						 AND C.UserId IS NOT NULL AND COALESCE(U.PaypalEmail, '')!='' 
						 AND COALESCE(C.Amount, 0)>0 
						 AND C.PaymentID IS NULL 
				ORDER BY OrderID";

                SqlDataReader rdr = null;
                SqlCommand cmd = new SqlCommand(sqlString, connection);
                cmd.Parameters.Clear();

                connection.Open();
                rdr = cmd.ExecuteReader();

                List<CommissionResult> commissionsList = new List<CommissionResult>();
                while (rdr.Read())
                {
                    var commissionResult = new CommissionResult()
                    {
                        CommissionID = rdr["CommissionID"].ToString(),
                        Amount = Convert.ToDecimal(rdr["Amount"]),
                        PaypalEmail = rdr["PaypalEmail"].ToString(),
                    };
                    commissionsList.Add(commissionResult);
                }

                IEnumerable<CommissionResult> result = commissionsList.GroupBy(p => p.PaypalEmail).Select(cl => new CommissionResult { CommissionID = cl.First().CommissionID, PaypalEmail = cl.First().PaypalEmail, Amount = Convert.ToDecimal(cl.Sum(c => c.Amount)) });

                var paymentDataService = new PaymentsDataService();
                processingResult.Data.ContentString += "<strong>Adding Payments</strong><br>";
                if (result.Count() > 0)
                {
                    processingResult.Data.ContentString += "<table><tr><td>ID</td><td>Amount</td><td>Email</td></tr>";
                    foreach (var commission in result)
                    {
                        if (commission.Amount > 0)
                        {
                            var paymentSaveData = new Payment
                            {
                                Id = Guid.NewGuid().ToString(),
                                Amount = commission.Amount,
                                Email = commission.PaypalEmail,
                                DateCreated = DateTime.UtcNow,
                                DateModified = DateTime.UtcNow,
                                IsDeleted = false
                            };
                            try
                            {
                                var paymentResult = paymentDataService.Add(paymentSaveData);
                                processingResult.Data.ContentString += "<tr><td>" + paymentSaveData.Id + "</td><td>" + paymentSaveData.Amount + "</td><td>" + paymentSaveData.Email + "</td></tr>";
                                if (!paymentResult.IsSuccessful)
                                {
                                    processingResult.IsSuccessful = false;
                                    processingResult.Error = new ProcessingError("Error adding payments", "Error adding payments", true, false);
                                    return processingResult;
                                }
                            }
                            catch (Exception ex)
                            {
                                var sendEmail = emailHelper.SendEmail(EmailSubject + " - Error Adding Payments", EmailToAddress, "", "Error adding payments");
                                processingResult.IsSuccessful = false;
                                processingResult.Error = new ProcessingError("Error adding payments.", "Error adding payments.", true, false);
                                return processingResult;
                            }



                            IEnumerable<CommissionResult> ppEmailResult = commissionsList.Where(p => p.PaypalEmail == commission.PaypalEmail);
                            foreach (var payment in ppEmailResult)
                            {
                                var getUserCommission = await GetWhereAsync(uc => uc.Id == payment.CommissionID);

                                var userCommissionUpdate = new UserCommissionDetail
                                {
                                    Id = getUserCommission.Data.Id,
                                    OrderId = getUserCommission.Data.OrderId,
                                    UserId = getUserCommission.Data.UserId,
                                    SalesTeamId = getUserCommission.Data.SalesTeamId,
                                    Amount = getUserCommission.Data.Amount,
                                    PaymentID = paymentSaveData.Id,
                                    DateCreated = getUserCommission.Data.DateCreated,
                                    DateModified = DateTime.UtcNow,
                                    IsDeleted = false
                                };

                                var updateUserCommission = await base.UpdateAsync(userCommissionUpdate);
                                if (!updateUserCommission.IsSuccessful)
                                {
                                    var sendEmail = emailHelper.SendEmail(EmailSubject + " - Error Updating Payment ID", EmailToAddress, "", "Error updating payment id.");
                                    processingResult.IsSuccessful = false;
                                    processingResult.Error = new ProcessingError("Error updating payment id.", "Error updating payment id.", true, false);
                                    return processingResult;
                                }
                            }
                        }
                    }
                    processingResult.Data.ContentString += "</table><br>";
                } else {
                    processingResult.Data.ContentString += "No pending commissions found.<br>";
                }

                //Paypal mass payment
                processingResult.Data.ContentString += "<br><strong>Pending Payments</strong><br>";
                var pendingPayments = await paymentDataService.GetPendingPaymentsForPaypalBatch();
                if (!pendingPayments.IsSuccessful)
                {
                    var sendEmail = emailHelper.SendEmail(EmailSubject + " - Error With Initial Pending Lookup", EmailToAddress, "", "Error getting pending payments.");
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error getting pending payments.", "Error getting pending payments.", true, false);
                    return processingResult;
                }

                int BatchNum = 0;
                while (pendingPayments.Data.Any())
                {
                    BatchNum += 1;
                    processingResult.Data.ContentString += "<i>Batch " + BatchNum + "</i><br><table><tr><td>ID</td><td>Amount</td><td>Email</td></tr>";
                    var paymentList = new List<IMassPaymentRequestItem>();

                    foreach (var payment in pendingPayments.Data)
                    {
                        var massPaymentRequestItem = new MassPaymentRequestItem
                        {
                            Id = payment.Id,
                            Amount = payment.Amount,
                            Email = payment.Email
                        };
                        paymentList.Add(massPaymentRequestItem);
                        processingResult.Data.ContentString += "<tr><td>" + massPaymentRequestItem.Id + "</td><td>" + massPaymentRequestItem.Amount + "</td><td>" + massPaymentRequestItem.Email + "</td></tr>";
                    }
                    processingResult.Data.ContentString += "</table><br>";

                    processingResult.Data.ContentString += "<strong>PayPal Call</strong><br>";
                    var massPaymentRequest = new MassPaymentRequest()
                    {
                        ReceiverType = "EmailAddress",
                        EmailSubject = "Your Commission Payment from Budget Mobile",
                        Payments = paymentList
                    };
                    processingResult.Data.ContentString += "<i>PayPal API Call</i>";
                    var paypalService = new PayPalService();
                    var paypalMassPaymentResult = await paypalService.MassPayment(massPaymentRequest);
                    processingResult.Data.ContentString += "<i>PayPal API Call Finished</i>";
                    if (paypalMassPaymentResult.IsSuccessful)
                    {
                        if (paypalMassPaymentResult.Data.IsPaymentSuccessful)
                        {
                            processingResult.Data.ContentString += "<i>Success</i><br>Updating DatePaid/TransactionID<br>";
                            using (var db = new ApplicationDbContext())
                            {
                                db.Configuration.ValidateOnSaveEnabled = false;
                                try
                                {
                                    foreach (var payment in pendingPayments.Data)
                                    {
                                        var payRow = new Payment { Id = payment.Id, DatePaid = DateTime.UtcNow, TransactionID = paypalMassPaymentResult.Data.CorrelationID };
                                        db.Payment.Attach(payRow);
                                        db.Entry(payRow).Property(p => p.DatePaid).IsModified = true;
                                        db.Entry(payRow).Property(p => p.TransactionID).IsModified = true;
                                        db.SaveChanges();
                                    }
                                }
                                finally { db.Configuration.ValidateOnSaveEnabled = true; }
                            }

                            //Recheck DB to see if there are any more pending payments.  If there is not, it will break out of the while loop
                            pendingPayments = await paymentDataService.GetPendingPaymentsForPaypalBatch();
                            if (!pendingPayments.IsSuccessful)
                            {
                                var sendEmail = emailHelper.SendEmail(EmailSubject + " - Error With Pending Lookup", EmailToAddress, "", "Error getting pending payments.");
                                processingResult.IsSuccessful = false;
                                processingResult.Error = new ProcessingError("Error getting pending payments.", "Error getting pending payments.", true, false);
                                return processingResult;
                            }
                        }
                        else
                        {
                            processingResult.IsSuccessful = false;
                            var sendEmail = emailHelper.SendEmail(EmailSubject + " - PayPal Payment Unsuccessful", EmailToAddress, "", "<h1>Error Message</h1>" + paypalMassPaymentResult.Data.ErrorMessage + "<br /><h1>Error Description</h1>" + paypalMassPaymentResult.Data.ErrorDescription + "<br /><h1>Process Information</h1>" + processingResult.Data.ContentString);
                            processingResult.Error = new ProcessingError(paypalMassPaymentResult.Data.ErrorDescription, paypalMassPaymentResult.Data.ErrorMessage, false, true);
                            return processingResult;
                        }
                    }
                    else
                    {
                        processingResult.IsSuccessful = false;
                        var sendEmail = emailHelper.SendEmail(EmailSubject + " - PayPal Call Unsuccessful", EmailToAddress, "", "<h1>Error Message</h1>" + paypalMassPaymentResult.Error.UserMessage + "<br /><h1>Error Description</h1>" + paypalMassPaymentResult.Error.UserHelp + "<br /><h1>Process Information</h1>" + processingResult.Data.ContentString);
                        processingResult.Error = new ProcessingError(paypalMassPaymentResult.Error.UserMessage, paypalMassPaymentResult.Error.UserHelp, paypalMassPaymentResult.Error.IsFatal, paypalMassPaymentResult.Error.CanBeFixedByUser);
                        return processingResult;
                    }
                }

                processingResult.Data.ContentString += "<strong>DONE</strong>";

                processingResult.IsSuccessful = true;
                return processingResult;
            } catch (Exception ex) {
                //var sendEmail = emailHelper.SendEmail(EmailSubject + " - Exception", EmailToAddress, "", "An error occurred while attempting to pay commissions.");
                string message ="Exception type: " + ex.GetType() + "<br />" + "Exception message: " + ex.Message + "<br />" + "Stack trace: " + ex.StackTrace + "<br />";
                if (ex.InnerException != null) {
                    message += "---BEGIN InnerException--- " + "<br />" +
                               "Exception type: " + ex.InnerException.GetType() + "<br />" +
                               "Exception message: " + ex.InnerException.Message + "<br />" +
                               "Stack trace: " + ex.InnerException.StackTrace + "<br />" +
                               "---END Inner Exception<br />";
                }
                var sendEmail = emailHelper.SendEmail(EmailSubject + " - Exception", EmailToAddress, "", "<h1>Exception</h1>" + message + "<br /><h1>Process Information</h1>" + processingResult.Data.ContentString);
                processingResult.Error = new ProcessingError("An error occurred while attempting to pay commissions", "An error occurred while attempting to pay commissions", true, false);
                processingResult.IsSuccessful = false;
                return processingResult;
            }
        }

        public async Task<ServiceProcessingResult> CreateUserCommissionDetailsFor(string salesTeamId, string loggedInUserId, string orderId)
        {
            var result = new ServiceProcessingResult();

            var salesTeamDataService = new SalesTeamDataService(loggedInUserId);

            var getSalesTeamResult = salesTeamDataService.Get(salesTeamId);
            if (!getSalesTeamResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Error = getSalesTeamResult.Error;
                return result;
            }

            var salesTeam = getSalesTeamResult.Data;

            if (salesTeam.CommissionLevel1UserId != null)
            {
                var level1AddResult = await AddAsync(new UserCommissionDetail
                {
                    Amount = salesTeam.CommissionLevel1Amount,
                    OrderId = orderId,
                    UserId = salesTeam.CommissionLevel1UserId,
                    SalesTeamId = salesTeam.Id,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                });

                if (!level1AddResult.IsSuccessful)
                {
                    result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
                    result.IsSuccessful = false;
                    return result;
                }
            }

            if (salesTeam.CommissionLevel2UserId != null)
            {
                var level2AddResult = await AddAsync(new UserCommissionDetail
                {
                    Amount = salesTeam.CommissionLevel2Amount,
                    OrderId = orderId,
                    SalesTeamId = salesTeam.Id,
                    UserId = salesTeam.CommissionLevel2UserId,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                });

                if (!level2AddResult.IsSuccessful)
                {
                    result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
                    result.IsSuccessful = false;
                    return result;
                }
            }

            if (salesTeam.CommissionLevel3UserId != null)
            {
                var level3AddResult = await AddAsync(new UserCommissionDetail
                {
                    Amount = salesTeam.CommissionLevel3Amount,
                    OrderId = orderId,
                    UserId = salesTeam.CommissionLevel3UserId,
                    SalesTeamId = salesTeam.Id,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                });

                if (!level3AddResult.IsSuccessful)
                {
                    result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
                    result.IsSuccessful = false;
                    return result;
                }
            }

            if (salesTeam.CommissionManagerUserId != null)
            {
                var managerAddResult = await AddAsync(new UserCommissionDetail
                {
                    Amount = salesTeam.CommissionManagerAmount,
                    OrderId = orderId,
                    UserId = salesTeam.CommissionManagerUserId,
                    SalesTeamId = salesTeam.Id,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                });

                if (!managerAddResult.IsSuccessful)
                {
                    result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
                    result.IsSuccessful = false;
                    return result;
                }
            }

            var salesTeamCommission = new UserCommissionDetail
            {
                Amount = salesTeam.CommissionAmount,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                SalesTeamId = salesTeam.Id,
                OrderId = orderId
            };

            var addResult = await AddAsync(salesTeamCommission);

            if (!addResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
                return result;
            }

            var dataService = new ApplicationUserDataService();
            var theUser = await dataService.GetAsync(loggedInUserId);

            var UserCommission = new UserCommissionDetail
            {
                Amount = theUser.Data.UserCommission,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                SalesTeamId = salesTeam.Id,
                OrderId = orderId,
                UserId = loggedInUserId
            };

            var userResult = await AddAsync(UserCommission);

            if (!userResult.IsSuccessful)
            {
                userResult.IsSuccessful = false;
                userResult.Error = ErrorValues.SALES_TEAM_COMMISSION_CREATION_ERROR;
                return userResult;
            }

            result.IsSuccessful = true;
            return result;
        }
    }
}

using System;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LS.ApiBindingModels;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services {
    public class PaymentsDataService : BaseDataService<Payment,string> {
        private static readonly string CheckCommission_StoredProcName = "dbo.usp_CheckCommissionsForErrors";
        private static readonly string CreateBatch_StoredProcName = "dbo.usp_CreateCommissionPaymentBatch";
        public override BaseRepository<Payment,string> GetDefaultRepository() {
            return new PaymentRepository();
            }

        public DataAccessResult<string> CreatePaymentBatch() {
            var result = new DataAccessResult<string>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(CreateBatch_StoredProcName,connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            try
                {
                connection.Open();
                var o = cmd.ExecuteScalar();
                if (o != null)
                    {
                    string vProcessId = o.ToString();
                     result.Data = vProcessId;
                    }
                result.IsSuccessful = true;
                
                }catch(Exception ex)
                {
                ex.ToExceptionless()
                      .SetMessage("Failed to create batch.")
                      .MarkAsCritical()
                      .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Failed to create batch:" + ex.Message);
               
                }
          
            return result;

            }

        public DataAccessResult<List<PaymentViewBindingModel>> PaymentBatches(bool PaidOnly) {
            var result = new DataAccessResult<List<PaymentViewBindingModel>>();
            List<PaymentViewBindingModel> Payments = new List<PaymentViewBindingModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);


            string sqlString = "";
            if (PaidOnly)
                {
                sqlString = "SELECT SUM(Amount) AS TotalAmount, TransactionId, MIN(DatePaid) AS DatePaid, COUNT(Email) AS NumberOfPayments  FROM Payments (NOLOCK) WHERE COALESCE(TransactionID,'') != '' GROUP BY TransactionId ORDER BY MIN(DatePaid) DESC";
                }
            else {
                //unpaid
                sqlString = "SELECT L.ProcessID, SUM(P.Amount) AS TotalAmount, COUNT(P.Email) AS NumberOfPayments FROM Payments P (NOLOCK)    LEFT JOIN (SELECT ProcessID,PaymentID FROM CommissionLogs (NOLOCK) GROUP BY PaymentID,ProcessID) L ON P.ID = L.PaymentID WHERE L.ProcessID IS NOT NULL AND COALESCE(P.DatePaid,'') = '' GROUP BY L.ProcessID";
                }

            SqlCommand cmd = new SqlCommand(sqlString,connection);
            SqlDataReader rdr = null;
            cmd.Parameters.Clear();
            //cmd.Parameters.Add(new SqlParameter("@UserID",userId));
            try {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read()) {

                    var payment = new PaymentViewBindingModel();
                    if (PaidOnly)
                        {
                        payment = new PaymentViewBindingModel()
                            {
                            TotalAmount = rdr.IsDBNull(rdr.GetOrdinal("TotalAmount")) ? 0 : (decimal)rdr["TotalAmount"],
                            DatePaid = rdr.IsDBNull(rdr.GetOrdinal("DatePaid")) ? DateTime.Now : (DateTime)rdr["DatePaid"],
                            TransactionID = rdr["TransactionID"].ToString(),
                            NumberOfPayments = (int)rdr["NumberOfPayments"],
                            };
                        } else
                        {
                        //unpaid
                        payment = new PaymentViewBindingModel()
                            {
                            TotalAmount = rdr.IsDBNull(rdr.GetOrdinal("TotalAmount")) ? 0 : (decimal)rdr["TotalAmount"],
                            NumberOfPayments = (int)rdr["NumberOfPayments"],
                            ProcessID = rdr["ProcessID"].ToString()
                            };
                        }
                    Payments.Add(payment);
                    }
                } catch (Exception ex) {
                ex.ToExceptionless()
                     .SetMessage("Failed to retrieve paid batches")
                     .MarkAsCritical()
                     .Submit();
                result.IsSuccessful = false;
                result.Error = new ProcessingError("Error retrieving payment batchs.","Error retrieving payment batches.",true,false);
                //Logger.Error("Failed to retrieve paid batches",ex);
                return result;
                } 
            result.IsSuccessful = true;
            result.Data = Payments;
            return result;
            }
        public  DataAccessResult<string> MarkBatchPaid(string processID) {
            var result = new DataAccessResult<string>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            
              string  sqlString = "UPDATE Payments SET DatePaid=getdate(), TransactionID=@TransactionID  WHERE ID IN(SELECT PaymentID AS ID FROM CommissionLogs  WHERE ProcessID = @ProcessID)";
                
           
            SqlCommand cmd = new SqlCommand(sqlString,connection);
          
            string transactionID = Guid.NewGuid().ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@TransactionID",transactionID));
            cmd.Parameters.Add(new SqlParameter("@ProcessID",processID));
            try
                {
                connection.Open();
               var updateResult=cmd.ExecuteNonQuery();
               if (updateResult > 0)
                    {
                    result.IsSuccessful = true;
                    result.Data = "PASS";
                     }
                }
            catch (Exception ex)
                {
                ex.ToExceptionless()
                     .SetMessage("Error updating payment batch.")
                     .MarkAsCritical()
                     .Submit();
                result.IsSuccessful = false;
                result.Data = "FAIL";
                result.Error = new ProcessingError("Error updating payment batch.","Error updating payment batch.",true,false);
                //Logger.Error("Error updating payment batch.",ex);

                }
            return result;
            }

        public  DataAccessResult<List<PaymentDetailViewBindingModel>> GetDetailTransaction(string transactionID,string type) {
            var result = new DataAccessResult<List<PaymentDetailViewBindingModel>>();
            List<PaymentDetailViewBindingModel> Payments = new List<PaymentDetailViewBindingModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            string sqlString = "";
            if (type == "batch")
                {
                sqlString = "SELECT SUM(Amount) AS TotalAmount, Email, COUNT(ID) AS NumberOfPayments FROM Payments WHERE TransactionID=@TransactionID GROUP BY Email ORDER BY Email";
                }
            else
                {
                sqlString = "SELECT SUM(Amount) AS TotalAmount, Email, COUNT(Email) AS NumberOfPayments FROM Payments (NOLOCK) WHERE ID IN(SELECT PaymentID AS ID FROM CommissionLogs(NOLOCK) WHERE ProcessID=@ProcessID) GROUP BY Email";
                }


            SqlCommand cmd = new SqlCommand(sqlString,connection);
            SqlDataReader rdr = null;
            cmd.Parameters.Clear();
            if (type == "batch") { cmd.Parameters.Add(new SqlParameter("@TransactionID",transactionID)); }
            else
                { cmd.Parameters.Add(new SqlParameter("@ProcessID",transactionID)); }

            try
                {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    {
                    var payment = new PaymentDetailViewBindingModel()
                        {
                        Email = rdr["Email"].ToString(),
                        TotalAmount = (decimal)rdr["TotalAmount"],
                        NumberOfPayments = (int)rdr["NumberOfPayments"]
                        };
                    Payments.Add(payment);
                    }
                }
            catch (Exception ex)
                {
                ex.ToExceptionless()
                     .SetMessage("Failed to retrieve paid batchs")
                     .MarkAsCritical()
                     .Submit();
                result.IsSuccessful = false;
                result.Error = new ProcessingError("Error retrieving detail payment batches.","Error retrieving detail payment batches.",true,false);
                //Logger.Error("Failed to retrieve paid batchs",ex);
                return result;
                }
            result.IsSuccessful = true;
            result.Data = Payments;
            return result;

            }
   
        public DataAccessResult<bool> CommissionCheck() {
            var result = new DataAccessResult<bool>() {IsSuccessful=true,Data=false };
            SqlDataReader rdr = null;
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(CheckCommission_StoredProcName,connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            try
                {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    {
                   bool CheckPassed = (bool)rdr["CheckPassed"];
                    result.Data = CheckPassed;
                    }
               //result is set return it at end
                }
            catch (Exception ex)
                {
                ex.ToExceptionless()
                     .SetMessage("Commission Check Failure.")
                     .MarkAsCritical()
                     .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Commission Check Failure:" + ex.Message);

                }
          
            return result;
           }
        }
    }

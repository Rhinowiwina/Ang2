using System;
using System.Data.Entity;
using System.Linq;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Exceptionless;
using Exceptionless.Models;

namespace LS.Repositories

    {
    public class LoginMsgRepository : BaseRepository<LoginMsg,string>
        {
        public LoginMsgRepository() : base(new AmbientDbContextLocator())
            {
            }
        public DataAccessResult<List<LoginMsg>> GetActiveMsg()
            {
            var result = new DataAccessResult<List<LoginMsg>>();
            //result.Data = new List<LoginMsg>();
            var messages = new List<LoginMsg>();
            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlDataReader rdr = null;
            string strcmd = "Select * from LoginMsgs  where ( BeginDate BETWEEN BeginDate and @Today) and (ExpirationDate BETWEEN @Today and ExpirationDate) and (Active=@Active)    Order By MsgLevel,BeginDate desc";
            SqlCommand cmd = new SqlCommand(strcmd,connection);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@Today",DateTime.UtcNow));
            cmd.Parameters.Add(new SqlParameter("@Active",true));


            try
                {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    {
                    messages.Add(new LoginMsg()
                        {
                        Id = rdr["Id"].ToString(),
                        Title = rdr["Title"].ToString(),
                        Msg = rdr["Msg"].ToString(),
                        BeginDate = (DateTime)rdr["BeginDate"],
                        ExpirationDate = (DateTime)rdr["ExpirationDate"],
                        MsgLevel = (int)rdr["MsgLevel"],
                        Active=(bool) rdr["Active"],
                        DateCreated = (DateTime)rdr["DateCreated"],
                        DateModified = (DateTime)rdr["DateModified"],
                        IsDeleted = rdr.GetBoolean(rdr.GetOrdinal("IsDeleted"))
                        });
                    }

                result.Data = messages;
                result.IsSuccessful = true;
                }
            catch (Exception ex)
                {
                ex.ToExceptionless()
                   .SetMessage("An error occurred with datareader.")
                   .MarkAsCritical()
                   .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("An error occurred with datareader.",ex);
                }
            finally { connection.Close(); }

            return result;

            }
        public DataAccessResult<List<LoginMsg>> GetAllMsg()
            {
            var result = new DataAccessResult<List<LoginMsg>>();
            //result.Data = new List<LoginMsg>();
            var messages = new List<LoginMsg>();
            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlDataReader rdr = null;
            string strcmd = "Select * from LoginMsgs where (isDeleted=0) Order By BeginDate desc";
            SqlCommand cmd = new SqlCommand(strcmd,connection);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Clear();




            try
                {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    {
                    messages.Add(new LoginMsg()
                        {
                        Id = rdr["Id"].ToString(),
                        Title = rdr["Title"].ToString(),
                        Msg = rdr["Msg"].ToString(),
                        BeginDate = (DateTime)rdr["BeginDate"],
                        ExpirationDate = (DateTime)rdr["ExpirationDate"],
                        MsgLevel = (int)rdr["MsgLevel"],
                        Active = (bool)rdr["Active"],
                        DateCreated = (DateTime)rdr["DateCreated"],
                        DateModified = (DateTime)rdr["DateModified"],
                        IsDeleted = rdr.GetBoolean(rdr.GetOrdinal("IsDeleted"))
                        });
                    }

                result.Data = messages;
                result.IsSuccessful = true;
                }
            catch (Exception ex)
                {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("An error occurred with datareader.",ex);

                ex.ToExceptionless()
                                  .SetMessage("An error occurred with datareader.")
                                  .MarkAsCritical()
                                  .Submit();
                }
            finally { connection.Close(); }

            return result;

            }
        public DataAccessResult<LoginMsg> UpdateMessage(LoginMsg updatedMsg)
            {
            var result = new DataAccessResult<LoginMsg>() { IsSuccessful = false };

            try
                {
                var existingMsg = GetEntity(updatedMsg.Id);
                existingMsg.Id = updatedMsg.Id;
                existingMsg.Title = updatedMsg.Title;
                existingMsg.Msg = updatedMsg.Msg;
                existingMsg.MsgLevel = updatedMsg.MsgLevel;
                existingMsg.BeginDate = updatedMsg.BeginDate;
                existingMsg.Active = updatedMsg.Active;
                existingMsg.ExpirationDate = updatedMsg.ExpirationDate;
                existingMsg.DateModified = DateTime.UtcNow;
                result.IsSuccessful = true;
                result.Data = existingMsg;
                }
            catch (InvalidOperationException)
                {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("Could not find message to update","Could not find message to update",true,false);
                }
            catch (Exception ex)
                {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("Failed to update message.",ex);

                ex.ToExceptionless()
                                  .SetMessage("Failed to update message.")
                                  .MarkAsCritical()
                                  .Submit();
                }

            return result;
            }

        public DataAccessResult DeleteMessage(string messageId)
            {
            var result = new DataAccessResult();

            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);

            string strcmd = "Delete from LoginMsgs where (id=@id)";
            SqlCommand cmd = new SqlCommand(strcmd,connection);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id",messageId);



            try
                {
                connection.Open();
                cmd.ExecuteNonQuery();
                result.IsSuccessful = true;
                }
            catch (Exception ex)
                {

                ex.ToExceptionless()
                                  .SetMessage("An error occurred deleting message.")
                                  .MarkAsCritical()
                                  .Submit();
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Logger.Error("An error occurred deleting message.",ex);
                }
            finally { connection.Close(); }

            return result;

            }
        }
    }

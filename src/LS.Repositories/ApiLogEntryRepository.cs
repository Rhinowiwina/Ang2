using System;
using LS.Core;
using LS.Domain;
using Numero3.EntityFramework.Implementation;
using System.Configuration;
using System.Data.SqlClient;
using Exceptionless;
using Exceptionless.Models;
using System.Data;

namespace LS.Repositories
{
    public class ApiLogEntryRepository : BaseRepository<ApiLogEntry, string>
    {
        public ApiLogEntryRepository() : base(new AmbientDbContextLocator())
        {
        }

        public DataAccessResult<ApiLogEntry> Add(ApiLogEntry logEntry)
        {
            var result = new DataAccessResult<ApiLogEntry> { Data = new ApiLogEntry() };
            //var dbContextScopeFactory = new DbContextScopeFactory();

            if (String.IsNullOrEmpty(logEntry.Input)) { logEntry.Input = ""; }

            if (String.IsNullOrEmpty(logEntry.Response)) { logEntry.Response = ""; }

            logEntry.DateCreated = DateTime.UtcNow;
            logEntry.DateModified = logEntry.DateCreated;
            logEntry.Id = Guid.NewGuid().ToString();
            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);

            SqlCommand cmd = new SqlCommand("INSERT INTO APILogEntries (Id, Api, [Function], Input, Response, DateStarted, DateEnded, DateCreated, DateModified, IsDeleted) VALUES (@Id, @Api, @Function, @Input, @Response, @DateStarted, @DateEnded, @DateCreated, @DateModified, @IsDeleted)", connection);
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@Id", logEntry.Id));
            cmd.Parameters.Add(new SqlParameter("@Api", logEntry.Api));
            cmd.Parameters.Add(new SqlParameter("@Function", logEntry.Function));
            cmd.Parameters.Add(new SqlParameter("@Input", logEntry.Input));
            cmd.Parameters.Add(new SqlParameter("@Response", logEntry.Response));
            cmd.Parameters.Add(new SqlParameter("@DateStarted", logEntry.DateStarted));
            cmd.Parameters.Add(new SqlParameter("@DateCreated", logEntry.DateCreated));
            cmd.Parameters.Add(new SqlParameter("@DateModified", logEntry.DateModified));
            cmd.Parameters.Add(new SqlParameter("@IsDeleted", false));


            var dateEnded = new SqlParameter("@DateEnded", SqlDbType.DateTime);
            dateEnded.Value = (object)logEntry.DateEnded ?? DBNull.Value;
            cmd.Parameters.Add(dateEnded);

            try {
                connection.Open();
                cmd.ExecuteNonQuery();
            } catch (Exception ex) {
                result.IsSuccessful = false;

                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                ex.ToExceptionless()
                     .SetMessage("Error inserting API Log")
                     .MarkAsCritical()
                     .Submit();
                throw;
            }

            result.Data.Id = logEntry.Id;
            return result;
        }

        public DataAccessResult<ApiLogEntry> Update(ApiLogEntry logEntry)
        {
            var result = new DataAccessResult<ApiLogEntry>();
            //var dbContextScopeFactory = new DbContextScopeFactory();
            if (String.IsNullOrEmpty(logEntry.Response)) { logEntry.Response = ""; }

            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);

            SqlCommand cmd = new SqlCommand("UPDATE APILogEntries SET Response=@Response, DateEnded=GETUTCDATE() WHERE Id=@Id", connection);
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@Id", logEntry.Id));
            cmd.Parameters.Add(new SqlParameter("@Response", logEntry.Response));

            try {
                connection.Open();
                cmd.ExecuteNonQuery();
            } catch (Exception ex) {
                result.IsSuccessful = false;

                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                ex.ToExceptionless()
                     .SetMessage("Error updating API Log")
                     .MarkAsCritical()
                     .Submit();
                throw;
            }

            return result;
        }
        //using (var dbContextScope = dbContextScopeFactory.Create())
        //{
        //    try
        //    {
        //        logEntry.DateCreated = DateTime.UtcNow;
        //        logEntry.DateModified = logEntry.DateCreated;
        //        //string isDev = ConfigurationManager.AppSettings["IsDev"];
        //        //if (isDev == "0")
        //        //{
        //        //    logEntry.Response = "";
        //        //    logEntry.Input = ""; 
        //        //}
        //        MainEntity.Add(logEntry);

        //        result.Data = logEntry;
        //        result.IsSuccessful = true;

        //        dbContextScope.SaveChanges();
        //    }
        //    catch (Exception)
        //    {
        //        result.IsSuccessful = false;
        //        result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
        //        throw;
        //    }
        //}

        //return result;

        //}
    }
}

using LS.Domain;
using LS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using LinqKit;
using LS.Core;
using Microsoft.AspNet.Identity;
using LS.ApiBindingModels;
using LS.Repositories.DBContext;
using System.Data;
using System.Data.SqlClient;
namespace LS.Services {
    public class ApiLogEntryDataService : BaseDataService<ApiLogEntry, string> {
        public override BaseRepository<ApiLogEntry, string> GetDefaultRepository() {
            return new ApiLogEntryRepository();
        }
        public async Task<ServiceProcessingResult<List<int>>> ArchiveEntries() {
            var processingResult = new ServiceProcessingResult<List<int>>();
            var sqlQuery = new SQLQuery();
            SqlParameter[] parameter = new SqlParameter[] { };
            //--1) Move rows from the Entries table to the Archive table (gets rid of input and responses).  Do this for aything that is at least 90 days old

            string cmdText1 = @"DECLARE @Logs TABLE(ID nvarchar(128),API varchar(100), [Function] varchar(250),DateStarted datetime,DateEnded datetime)
            INSERT INTO @Logs(ID,API, [Function],DateStarted,DateEnded) SELECT TOP 2000000 ID, API, [Function], DateStarted, DateEnded FROM APILogEntries(NOLOCK) WHERE DateStarted<CONVERT(date,DateAdd(day, -90,getdate())) AND API!='LexisNexis'
            INSERT INTO APILogArchive(ID,API,[Function],DateStarted,DateEnded) SELECT ID, API, [Function], DateStarted, DateEnded FROM @Logs
            DELETE FROM APILogEntries WHERE ID IN(SELECT ID FROM @Logs)";
            //-- 2) Create summaries of API Calls and insert into Stats, then delete the entries from the Archive.Do for anything that is 180 days old
            string cmdText2 = @" INSERT INTO APILogStats(API,[Function],NumCalls,AvgLength,Date) SELECT API, [Function], COUNT(*) AS NumCalls,AVG(DateDiff(ms,DateStarted,DateEnded)) AS AvgLength,DateAdd(day,Day(DateStarted) - 1,DateAdd(month,Month(DateStarted) - 1,DateAdd(Year,Year(DateStarted) - 1900,0))) AS[Date] FROM APILogArchive(NOLOCK) WHERE DateStarted < CONVERT(date,DateAdd(day,-180,getdate())) GROUP BY API, [Function],Year(DateStarted),Month(DateStarted),Day(DateStarted) ORDER BY Year(DateStarted),Month(DateStarted),Day(DateStarted),API, [Function] ";
            string cmdText3 = @"DELETE FROM APILogArchive WHERE DateStarted < CONVERT(date,DateAdd(day,-180,getdate())) ";
            var result = await sqlQuery.ExecuteNonQueryAsync(System.Data.CommandType.Text, cmdText1, 1000, parameter);
            var result2 = await sqlQuery.ExecuteNonQueryAsync(System.Data.CommandType.Text, cmdText2, 1000, parameter);
            var result3 = await sqlQuery.ExecuteNonQueryAsync(System.Data.CommandType.Text, cmdText3, 1000, parameter);
            if (result.IsSuccessful && result2.IsSuccessful && result3.IsSuccessful) {
                processingResult.IsSuccessful = true;
                List<int> vList = new List<int>();
                vList.Add(result.Data / 3);//3 operations on one set of record are done
                vList.Add(result3.Data);// number of records removed from archive                     
                processingResult.Data = vList;

            } else if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("API Log Archive Failed!", "API Log Archive Failed!", true, false);
            }

            return processingResult;

        }
        public async Task<ServiceProcessingResult<List<ApiLogEntry>>> GetUnImportedLogEntries(string API, string Function) {
            var apiLogResult = await GetAllWhereAsync(u => u.Api == API && u.Function.ToUpper() == Function && (u.JsonImported == false));
            if (!apiLogResult.IsSuccessful) {
                return apiLogResult;
            }
            if (apiLogResult.Data == null) {
                apiLogResult.IsSuccessful = false;
                apiLogResult.Error = new ProcessingError("No Data Returned", "No Data Returned", true, false);
            }
            return apiLogResult;
        }

    }
}

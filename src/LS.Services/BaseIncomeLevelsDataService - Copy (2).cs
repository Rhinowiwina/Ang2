using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace LS.Services{
    public class BaseIncomeLevelsDataService : BaseDataService<BaseIncomeLevels, string>{
        public override BaseRepository<BaseIncomeLevels, string> GetDefaultRepository(){
            return new BaseIncomeLevelsRepository();
        }

        public async Task<ServiceProcessingResult<BaseIncomeLevels>> GetBaseIncomeLevelsByStateCodeAsync(string stateCode){
            var processingResult = new ServiceProcessingResult<BaseIncomeLevels>();

            var cmdText = @"
                SELECT TOP 1 * FROM BaseIncomeLevels WHERE StateCode=@StateCode AND getdate() > DateActive AND IsDeleted=0 ORDER BY DateActive DESC
            ";

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@StateCode", stateCode)
            };

            var sqlQuery = new SQLQuery();
            var baseIncomeResult = await sqlQuery.ExecuteReaderAsync<BaseIncomeLevels>(CommandType.Text, cmdText, parameters);
            if (!baseIncomeResult.IsSuccessful || baseIncomeResult.Data == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = baseIncomeResult.Error;
                return processingResult;
            }

            var baseIncomeList = (List<BaseIncomeLevels>)baseIncomeResult.Data;
            processingResult.IsSuccessful = true;
            processingResult.Data = baseIncomeList[0];

            return processingResult;
        }
    }
}

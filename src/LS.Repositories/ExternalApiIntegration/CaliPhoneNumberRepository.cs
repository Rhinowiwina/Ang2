using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using LS.Core;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class CaliPhoneNumberRepository : BaseRepository<CaliPhoneNumber, string>
    {
        private static readonly string StoredProcedureFunction = "exec usp_GetNextValidCaliPhoneNumber @companyId";
        private static readonly string CompanyIdParameterName = "companyId";

        public CaliPhoneNumberRepository() : base(new AmbientDbContextLocator())
        {
        }

        public DataAccessResult<string> GetNextValidCaliPhoneNumber(string companyId)
        {
            var result = new DataAccessResult<string>();

            try
            {
                var companyIdParameter = new SqlParameter(CompanyIdParameterName, companyId);

                var phoneNumber = DbContext.Database.SqlQuery<string>(StoredProcedureFunction, companyIdParameter);

                var number = phoneNumber.First();

                result.Data = number.ToString(CultureInfo.InvariantCulture);
                result.IsSuccessful = true;
                return result;
            }
            catch (Exception exception)
            {
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                //Todo: Logger
                return result;
            }
        }
    }
}

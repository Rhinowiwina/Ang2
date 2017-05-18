using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using LS.Core;
using LS.Domain.ExternalApiIntegration.PuertoRico;
using Numero3.EntityFramework.Implementation;

namespace LS.Repositories
{
    public class PuertoRicoPhoneNumberRepository : BaseRepository<PuertoRicoPhoneNumber, string>
    {
        private static readonly string StoredProcedureFunction = "exec usp_GetNextValidNladPhoneNumber @companyId";
        private static readonly string CompanyIdParameterName = "companyId";

        private static readonly string GetOldestPuertoRicoNumberStoredProcedure = "exec usp_GetOldestInUsePuertoRicoPhoneNumber @companyId";


        public PuertoRicoPhoneNumberRepository() : base(new AmbientDbContextLocator())
        {
        }

        public DataAccessResult<string> GetNextValidPuertoRicoPhoneNumber(string companyId)
        {
            var result = new DataAccessResult<string>();

            try
            {
                var companyIdParameter = new SqlParameter(CompanyIdParameterName, companyId);

                var phoneNumber = DbContext.Database.SqlQuery<Int64>(StoredProcedureFunction, companyIdParameter);

                var number = phoneNumber.FirstOrDefault();

                if (number == default(long))
                {
                    companyIdParameter = new SqlParameter(CompanyIdParameterName, companyId);

                    var recycledPhoneNumber = DbContext.Database.SqlQuery<Int64>(GetOldestPuertoRicoNumberStoredProcedure,
                        companyIdParameter);

                    var recycledNumber = recycledPhoneNumber.First();

                    result.Data = recycledNumber.ToString(CultureInfo.InvariantCulture);
                    result.IsSuccessful = true;
                    return result;
                }

                result.Data = number.ToString(CultureInfo.InvariantCulture);
                result.IsSuccessful = true;
                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                result.IsSuccessful = false;
                result.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                return result;
            }
        }
    }
}

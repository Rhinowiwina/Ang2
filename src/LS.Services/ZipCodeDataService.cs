using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using System.Data.SqlClient;
using System.Configuration;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services
{
    public class ZipCodeDataService : BaseDataService<ZipCode, string>
    {
        public override BaseRepository<ZipCode, string> GetDefaultRepository()
        {
            return new ZipCodeRepository();
        }

        public async Task<ServiceProcessingResult<List<ZipCode>>> GetExistingZipCodesFromPostalCodeAsync(string postalCode) {
           var processingResult = new ServiceProcessingResult<List<ZipCode>> { IsSuccessful = true};
 
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var sqlString = "SELECT Id,PostalCode,State,StateAbbreviation,City FROM Zipcodes (NOLOCK) WHERE PostalCode=@PostalCode AND IsDeleted=0";
            SqlCommand cmd = new SqlCommand(sqlString, connection);
            SqlDataReader rdr = null;

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@PostalCode", postalCode));
            var zipcodeList = new List<ZipCode>();
            try {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    var zipcodeObject = new ZipCode {
                        Id = rdr["Id"].ToString(),
                        PostalCode = rdr["PostalCode"].ToString(),
                        State = rdr["State"].ToString(),
                        StateAbbreviation = rdr["StateAbbreviation"].ToString(),
                        City = rdr["City"].ToString()
                    };

                    zipcodeList.Add(zipcodeObject);
                }
                connection.Close();

                if (zipcodeList.Count == 0) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.ZIP_CODE_NOT_VALID_OR_NOT_FOUND_ERROR;
                } else {
                    processingResult.Data = zipcodeList;
                }
            } catch (Exception ex) {
                    ex.ToExceptionless()
                      .SetMessage("Error with zip code query (" + postalCode + ")")
                      .MarkAsCritical()
                      .Submit();
                connection.Close();
                processingResult.IsSuccessful = false;
                //Logger.Fatal("Error with zip code query (" + postalCode + ")");
                processingResult.Error = ErrorValues.ZIP_CODE_NOT_VALID_OR_NOT_FOUND_ERROR;
            }

            return processingResult;
        }
    }
}

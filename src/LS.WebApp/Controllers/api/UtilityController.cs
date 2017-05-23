using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using System;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using LS.WebApp.CustomAttributes;
using LS.ApiBindingModels;
using LS.Services;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api {

    [SingleSessionAuthorize]
    [RoutePrefix("api/utilities")]
    public class UtilityController : BaseApiController {
        private static string reportingKey = "Rep0rting@utoLog1n-treat";//This string must be 24 character long if not must be hashed. See http://www.codeproject.com/Articles/14150/Encrypt-and-Decrypt-Data-with-C
        [HttpGet]
        [Route("getServerVars")]
        public async Task<IHttpActionResult> GetServerVars() {
            var processingResult = new ServiceProcessingResult<ServerEnvironmentBindingModel> { IsSuccessful = true, Data = new ServerEnvironmentBindingModel() };
            var result = await new UtilityDataService().GetServerVars();
            if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting environment.", "Error getting environment.", true, false);
                return Ok(processingResult);
            }
            try {
                result.Data.LoggedInUser = LoggedInUser;
            } catch {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting logged in user.", "Error getting logged in user.", true, false);
                return Ok(processingResult);
            }
            processingResult.Data = result.Data;

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getEncryptedReportUrl")]
        public async Task<IHttpActionResult> getEncryptedReportUrl(string loggedinUserId) {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true, Data = "" };
            string timestamp = DateTime.UtcNow.ToString("o");
            string baseurl = ConfigurationManager.AppSettings["BaseReportingUrl"].ToString();
            string strToEncrypt = loggedinUserId + "|" + timestamp;
            string encryptedString = this.Encrypt(strToEncrypt, reportingKey);
            if (encryptedString != "Failed") {
                encryptedString = HttpContext.Current.Server.UrlEncode(encryptedString);
                string retval = baseurl + encryptedString;
                processingResult.IsSuccessful = true;
                processingResult.Data = retval;
                return Ok(processingResult);
            } else {
                processingResult.IsSuccessful = false;
                processingResult.Data = null;
                processingResult.Error = new ProcessingError("Failed to encrypt url.", "Failed to encrypt url.", true, false);
                return Ok(processingResult);
            }
        }

        private string Encrypt(string data, string key) {
            UTF8Encoding utf8_encoder = new UTF8Encoding();
            ASCIIEncoding ascii_encoder = new ASCIIEncoding();

            string key_value = key;
            string init_vector = "@vector@";

            byte[] key_array = utf8_encoder.GetBytes(key_value);
            byte[] iv_array = ascii_encoder.GetBytes(init_vector);

            string to_encrypt = data;
            byte[] to_encrypt_array = utf8_encoder.GetBytes(to_encrypt);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            try { tdes.Key = key_array; } catch (Exception ex) {
                ex.ToExceptionless()
                    .SetMessage("Failed to encrypt report Url.")
                    .MarkAsCritical()
                    .Submit();
               
                return "Failed";
            };

            tdes.IV = iv_array;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            var the_encryptor = tdes.CreateEncryptor();
            byte[] result_array = the_encryptor.TransformFinalBlock(to_encrypt_array, 0, to_encrypt_array.Length);

            string the_encrypted_string = Convert.ToBase64String(result_array);

            return the_encrypted_string;
        }

        private string Decrypt(string dataBase64Str, string key) {
            UTF8Encoding utf8_encoder = new UTF8Encoding();
            ASCIIEncoding ascii_encoder = new ASCIIEncoding();

            string key_value = key;
            string init_vector = "@vector@";

            byte[] key_array = utf8_encoder.GetBytes(key_value);
            byte[] iv_array = ascii_encoder.GetBytes(init_vector);

            //byte[] result_array = Convert.FromBase64String( dataBase64Str );

            //string to_encrypt = dataBase64Str;
            byte[] to_encrypt_array = Convert.FromBase64String(dataBase64Str); // utf8_encoder.GetBytes( to_encrypt );

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = key_array;
            tdes.IV = iv_array;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            var the_encryptor = tdes.CreateDecryptor();
            byte[] result_array = the_encryptor.TransformFinalBlock(to_encrypt_array, 0, to_encrypt_array.Length);

            //string the_encrypted_string = Convert.ToBase64String( result_array );
            string the_encrypted_string = UTF8Encoding.UTF8.GetString(result_array);

            return the_encrypted_string;
        }

    }
}


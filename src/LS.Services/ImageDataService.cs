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
using System.Data;

namespace LS.Services {
    public class ImageDataService {
        public async Task<ServiceProcessingResult<string>> UploadImageAsync(byte[] image, string imageFileName, string type, string companyID) {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };

            var parameters = new SqlParameter[] { new SqlParameter("@CompanyId", companyID), new SqlParameter("@Type", type) };
            var sqlQuery = "SELECT AccessKey, Path, SecretKey,MaxImageSize FROM ExternalStorageCredentials WHERE CompanyId=@CompanyId AND Type=@Type AND IsDeleted=0";
            var sqlQueryService = new SQLQuery();
            var externalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!externalStorageCredentialsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred looking up external storage credentials.", "An error occurred looking up external storage credentials.", false);
                return processingResult;
            }

            var storageCreds = new ExternalStorageCredentials();
            foreach (DataRow row in externalStorageCredentialsResult.Data.Rows) {
                storageCreds.AccessKey = row["AccessKey"].ToString();
                storageCreds.Path = row["Path"].ToString();
                storageCreds.SecretKey = row["SecretKey"].ToString();
                storageCreds.MaxImageSize =(int) row["MaxImageSize"];
            }

            var externalStorageService = new ExternalStorageService(storageCreds);
            var saveResult = externalStorageService.SaveImage(image, imageFileName, "jpg");
            if (!saveResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred saving image to external storage.", "An error occurred while trying to save image. Please try again.", false);
                return processingResult;
            }

            processingResult.IsSuccessful = saveResult.IsSuccessful;

            return processingResult;
            }
        }
}
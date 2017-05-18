using System;
using System.Data.Entity.Infrastructure.Annotations;
using System.Threading.Tasks;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.Utilities;
using System.Data.SqlClient;
using System.Configuration;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services {
    public class ImageUploadDataService : BaseDataService<ImageUpload, string> {
        private static readonly string ProofImageType = "Proof";
        public static readonly int LengthOfImageCode = 6;

        public override BaseRepository<ImageUpload, string> GetDefaultRepository() {
            return new ImageUploadRepository();
        }

        public async Task<ServiceProcessingResult<VerifyImageUploadResponseBindingModel>> VerifyImageUploadAsync(string imageCode) {
            var processingResult = new ServiceProcessingResult<VerifyImageUploadResponseBindingModel> { IsSuccessful = true };

            var getImageUploadResult = await GetImageUploadWithCompanyByImageCode(imageCode);
            if (!getImageUploadResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error =
                    new ProcessingError("An error occurred while looking up image upload by image code.",
                        "An error occurred while looking up image code. Please try again. If problem persists, please contact support.",
                        true);
                return processingResult;
            }
            var imageUpload = getImageUploadResult.Data;

            if (imageUpload == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Image code invalid.",
                    "The image code you provided was invalid. Please try again.", false, true);
                return processingResult;
            }

            if (imageUpload.HasBeenUploaded) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Image code has already been used.",
                    "The image relating to this code has already been uploaded.", false);
                return processingResult;
            }

            if (imageUpload.Company == null) {
                var companyService = new CompanyDataService();
                var getCompanyResult = companyService.Get(imageUpload.CompanyId);
                if (!getCompanyResult.IsSuccessful || getCompanyResult.Data == null) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("An error occurred while retrieving company.",
                        "An error occurred while retrieving company information. If the problem persists, please try again.",
                        true);
                    return processingResult;
                }
                imageUpload.Company = getCompanyResult.Data;
            }

            var responseObject = new VerifyImageUploadResponseBindingModel {
                CompanyName = imageUpload.Company.Name,
                CompanyLogoUrl = imageUpload.Company.CompanyLogoUrl,
                PrimaryColorHex = imageUpload.Company.PrimaryColorHex,
                SecondaryColorHex = imageUpload.Company.SecondaryColorHex,
                MaxImageSize = imageUpload.MaxImageSize,
                UploadId = imageUpload.Id
            };
            processingResult.Data = responseObject;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ImageUpload>> VerifyImageHasBeenUploadedAsync(string imageCode) {
            var processingResult = new ServiceProcessingResult<ImageUpload>();

            var imageUploadResult = await GetWhereAsync(iu => iu.ImageCode == imageCode);
            if (!imageUploadResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving image code status", "Error retrieving image code status", true, false);
                return processingResult;
            }

            if (!imageUploadResult.Data.HasBeenUploaded) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.IMAGE_HAS_NOT_YET_BEEN_UPLOADED_ERROR;
                return processingResult;
            }

            var credentialsService = new ExternalStorageCredentialsDataService();
            var getCredentialsResult = await credentialsService.GetProofImageStorageCredentialsFromCompanyId(imageUploadResult.Data.CompanyId, "Proof");
            if (!getCredentialsResult.IsSuccessful || getCredentialsResult.Data == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while retrieving storage credentials.", "An error occurred while retrieving storage credentials. If the problem presists, please contact support.", true);
                return processingResult;
            }

            var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
            if (!externalStorageService.DoesFileExist(imageUploadResult.Data.Id + ".png")) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.IMAGE_HAS_NOT_YET_BEEN_UPLOADED_ERROR;
                return processingResult;
            }

            return imageUploadResult;
        }

        public async Task<ServiceProcessingResult> UploadImageAsync(byte[] image, string deviceDetails, string uploadId, string fileType) {
            var processingResult = new ServiceProcessingResult();

            var getImageUploadResult = await GetImageUploadWithStorageCredentialsAsync(uploadId);
            if (!getImageUploadResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error =
                    new ProcessingError("An error occurred while retrieving information for this upload.",
                        "An error occurred while retrieving upload information. Please try again. If the problem persists, contact support.",
                        true);
                return processingResult;
            }
            if (getImageUploadResult.Data == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("This image upload is not valid.",
                    "Information provided is not valid. Please try again.", false);
                return processingResult;
            }
            if (getImageUploadResult.Data.HasBeenUploaded) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("This image upload has already been processed.",
                    "This image upload has already been processed.", false);
                return processingResult;
            }

            var imageUploadObject = getImageUploadResult.Data;
            if (imageUploadObject.StorageCredentials == null) {
                var credentialsService = new ExternalStorageCredentialsDataService();
                var getCredentialsResult = credentialsService.Get(imageUploadObject.StorageCredentialsId);
                if (!getCredentialsResult.IsSuccessful || getCredentialsResult.Data == null) {
                    processingResult.IsSuccessful = false;
                    // TODO Revisit error
                    processingResult.Error =
                        new ProcessingError("An error occurred while retrieving company's storage credentials.",
                            "An error occurred while retrieving storage credentials. If the problem presists, please contact support.",
                            true);
                    return processingResult;
                }
                imageUploadObject.StorageCredentials = getCredentialsResult.Data;
            }

            var externalStorageService = new ExternalStorageService(imageUploadObject.StorageCredentials);
            var saveResult = externalStorageService.SaveImage(image, imageUploadObject.Id, fileType);
            if (!saveResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred saving image to external storage.",
                    "An error occurred while trying to save image. Please try again.", false);
                return processingResult;
            }

            imageUploadObject.DateUploaded = DateTime.UtcNow;
            imageUploadObject.HasBeenUploaded = true;
            imageUploadObject.DeviceDetails = deviceDetails;

            var updateResult = await UpdateAsync(imageUploadObject);
            if (!updateResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = updateResult.Error;
                return processingResult;
            }

            processingResult.IsSuccessful = true;

            return processingResult;
        }

        public async Task<ServiceProcessingResult<ImageUpload>> GenerateImageUploadForProofImageAsync(string companyId, string userId, string uploadType) {
            var processingResult = new ServiceProcessingResult<ImageUpload>();

            var imageCodeResult = await GetNewImageCodeAsync();
            if (!imageCodeResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                //TODO: Pull into error values
                processingResult.Error = new ProcessingError("An error occurred while generating image upload code.",
                    "An error occurred while generating image upload code. If the problem persists, please contact support.",
                    false);
                return processingResult;
            }
            var imageCode = imageCodeResult.Data;

            var storageCredentialsService = new ExternalStorageCredentialsDataService();
            var credentialsResult = await storageCredentialsService.GetProofImageStorageCredentialsFromCompanyId(companyId, ProofImageType);
            if (!credentialsResult.IsSuccessful || credentialsResult.Data == null) {
                processingResult.IsSuccessful = false;
                // TODO Pull into error values
                processingResult.Error =
                    new ProcessingError(
                        "An error occurred while retrieving company's proof image storage credentials.",
                        "An error occurred retrieving storage credentials. If the problem persists, please contact support.",
                        false);
                return processingResult;
            }
            var credentials = credentialsResult.Data;

            var imageUpload = new ImageUpload {
                ImageCode = imageCode,
                CompanyId = companyId,
                UploadType = uploadType,
                HasBeenUploaded = false,
                IsDeleted = false,
                UserId = userId,
                StorageCredentialsId = credentials.Id,
                MaxImageSize = credentials.MaxImageSize
            };

            return await AddAsync(imageUpload);
        }

        public async Task<ServiceProcessingResult<ImageUpload>> GetImageUploadWithCompanyByImageCode(string imageCode) {
            return await GetWhereAsync(iu => iu.ImageCode == imageCode, new[] { "Company" });
        }

        public async Task<ServiceProcessingResult<ImageUpload>> GetImageUploadWithStorageCredentialsAsync(string id) {
            return await GetWhereAsync(iu => iu.Id == id, new[] { "StorageCredentials" });
        }

        private async Task<ServiceProcessingResult<string>> GetNewImageCodeAsync() {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };

            var imageCode = String.Empty;
            var imageCodeExists = true;

            while (imageCodeExists) {
                imageCode = AlphanumericKeyGenerator.GenerateAlphanumericKeyOfLength(LengthOfImageCode);
                var imageCodeExistsResult = await ImageCodeExistsAsync(imageCode);
                if (!imageCodeExistsResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error =
                        new ProcessingError("An error occurred while generating the image code for upload.",
                            "An error occurred while generating the image code for your upload.", true);
                    return processingResult;
                }

                imageCodeExists = imageCodeExistsResult.Data;
            }

            processingResult.Data = imageCode;
            return processingResult;
        }

        private async Task<ServiceProcessingResult<bool>> ImageCodeExistsAsync(string imageCode) {
            var processingResult = new ServiceProcessingResult<bool> { IsSuccessful = true };

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var sqlString = "SELECT TOP 1 Id FROM ImageUploads (NOLOCK) WHERE ImageCode=@ImageCode AND IsDeleted=0";
            SqlCommand cmd = new SqlCommand(sqlString, connection);
            SqlDataReader rdr = null;

            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@ImageCode", imageCode));

            try {
                connection.Open();
                rdr = cmd.ExecuteReader();
                processingResult.Data = rdr.HasRows;
                connection.Close();
                return processingResult;
            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                      ex.ToExceptionless()
                     .SetMessage("Error getting image code(" + imageCode + ")")
                     .MarkAsCritical()
                     .Submit();
                //Logger.Fatal("Error getting image code(" + imageCode + ")");
                processingResult.Error = new ProcessingError("Error getting image code (" + imageCode + ").", "Error getting image code (" + imageCode + ").", true, false);
                return processingResult;
                }
           

            //var getExternalImageUploadResult = await GetWhereAsync(i => i.ImageCode == imageCode);
            //if (!getExternalImageUploadResult.IsSuccessful)
            //{
            //    processingResult.IsSuccessful = false;
            //    // TODO: does this error need to be more specific than the generic fatal backend error this will likely be?
            //    processingResult.Error = getExternalImageUploadResult.Error;
            //    return processingResult;
            //}

            //processingResult.Data = getExternalImageUploadResult.Data != null;
            //return processingResult;
            }
    }
}
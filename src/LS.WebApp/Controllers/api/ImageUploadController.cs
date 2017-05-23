using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LS.ApiBindingModels;
using LS.Core;
using LS.Services;
using LS.Utilities;
using LS.WebApp.Utilities;
using LS.Domain;
using LS.WebApp.CustomAttributes;

namespace LS.WebApp.Controllers.api {
    [SingleSessionAuthorize]
    [RoutePrefix("api/imageupload")]
    public class ImageUploadController : ApiController {
        private static readonly string BasicAuthType = "Basic";
        private static readonly string AuthHeader = "Authorization";
        //set the type of document being searched, values are Proof, Docs. Function is used in another controller with Docs value
        private static readonly string ProofImageType = "Proof";
        [Route("verify")]
        [HttpGet]
        public async Task<IHttpActionResult> Verify(string imageCode) {
            var processingResult = new ServiceProcessingResult<VerifyImageUploadResponseBindingModel> { IsSuccessful = true };

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            var imageUploadService = new ImageUploadDataService();
            processingResult = await imageUploadService.VerifyImageUploadAsync(imageCode);

            return Ok(processingResult);
        }

        private bool RequestIsAuthorized() {
            var authHeader = HttpContext.Current.Request.Headers[AuthHeader];
            if (authHeader == null || !authHeader.StartsWith(BasicAuthType)) {
                return false;
            }
            var encodedUsernamePassword = authHeader.Substring((BasicAuthType + " ").Length).Trim();
            var usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var credsArray = usernamePassword.Split(':');
            var username = credsArray[0];
            var password = credsArray[1];

            var basicAuthIsValid = username == ApplicationConfig.MobileImageUploadUsername &&
                                   password == ApplicationConfig.MobileImageUploadPassword;

            return basicAuthIsValid;
        }

        [HttpPost]
        [Route("uploadImage")]
        public async Task<IHttpActionResult> UploadImage() {
            var processingResult = new ServiceProcessingResult();

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }

            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);
            var uploadId = provider.FormData.GetValues("UploadId");
            var deviceDetails = provider.FormData.GetValues("DeviceDetails");
            if (uploadId == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Missing parameter UploadId", "Missing parameter UploadId", true);
                return Ok(processingResult);
            }
            if (deviceDetails == null) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Missing parameter DeviceDetails", "Missing parameter DeviceDetails", true);
                return Ok(processingResult);
            }
            foreach (MultipartFileData file in provider.FileData) {
                var image = File.ReadAllBytes(file.LocalFileName);
                var imageUploadService = new ImageUploadDataService();
                processingResult = await imageUploadService.UploadImageAsync(image, deviceDetails.FirstOrDefault(), uploadId.FirstOrDefault(),"jpg");
                try {
                    var rootFiles = new DirectoryInfo(root);
                    foreach (FileInfo deletefile in rootFiles.GetFiles()) {
                        if (deletefile.LastAccessTime < DateTime.UtcNow.AddDays(-1)) {
                            deletefile.Delete();
                        }
                    }
                } catch (Exception ex) {

                }
                return Ok(processingResult);
            }
            return Ok(processingResult);
        }

        [HttpGet]
        [Route("generateImageUploadCodeForTesting")]
        // TODO: Remove this method when no longer needed for mobile app testing
        public async Task<IHttpActionResult> GenerateImageUploadCodeForTesting() {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };

            var imageUploadDataService = new ImageUploadDataService();
            var imageCodeResult =
                await
                    imageUploadDataService.GenerateImageUploadForProofImageAsync("65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c",
                        "0c0bbb64-2861-48d9-a185-b9cd7133c5da","app");
            if (!imageCodeResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = imageCodeResult.Error;
                return Ok(processingResult);
            }
            var imageCode = imageCodeResult.Data.ImageCode;

            processingResult.Data = imageCode;
            return Ok(processingResult);
        }

        [Route("getImageUrl")]
        [HttpGet]
        [ExceptionHandling("Could not retrieve Image URL")]
        public async Task<IHttpActionResult> GetImageUrl(string imageId) {
            return await GetImageUrl(imageId, "Proof"); ;
        }

        [Route("getImageUrl")]
        [HttpGet]
        [ExceptionHandling("Could not retrieve Image URL")]
        public async Task<IHttpActionResult> GetImageUrl(string imageId, string imageType) {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };

            if (!RequestIsAuthorized()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Not authorized.", "Not authorized.", false);
                return Ok(processingResult);
            }
            var imageResult = new ServiceProcessingResult<ImageUpload> { IsSuccessful = false, Data = new ImageUpload() };

            if (imageType == "Signatures") {
                // Go find order based on the signature ID
                var orderDataService = new OrderDataService();
                var orderLookup = await orderDataService.GetOrderBySignatureID(imageId);
                if (orderLookup.IsSuccessful) {
                    imageResult.IsSuccessful = true;
                    imageResult.Data.CompanyId = orderLookup.Data.CompanyId;
                }
            } else {
                var imageUploadDataService = new ImageUploadDataService();
                imageResult = imageUploadDataService.Get(imageId);
            }
        
            if (imageResult.IsSuccessful && imageResult.Data!=null) {
                var credentialsService = new ExternalStorageCredentialsDataService();
                var getCredentialsResult = await credentialsService.GetProofImageStorageCredentialsFromCompanyId(imageResult.Data.CompanyId, imageType);
                if (!getCredentialsResult.IsSuccessful || getCredentialsResult.Data == null) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("An error occurred while retrieving storage credentials.", "An error occurred while retrieving storage credentials. If the problem presists, please contact support.", true);
                }

                var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
                var fileName = imageId + ".png";

                if (!externalStorageService.DoesFileExist(fileName)) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("File does not exist", "File does not exist. If the problem presists, please contact support.", true);
                }

                var urlResult = externalStorageService.GeneratePreSignedUrl(fileName);
                if (!urlResult.IsSuccessful || urlResult.Data == null) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("An error occurred while generating the file URL.", "An error occurred while generating the file URL. If the problem presists, please contact support.", true);
                }
                processingResult.Data = urlResult.Data;
            } else {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while retrieving image details (Image Type: "+imageType+").", "An error occurred while retrieving image details. If the problem presists, please contact support.", true);
            }
            return Ok(processingResult);
        }
    }
}

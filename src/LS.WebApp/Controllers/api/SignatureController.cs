using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.WebApp.CustomAttributes;
using LS.Utilities;
using Topaz;
using LS.WebApp.Models;
using NLog.Internal;
using System.Drawing;
using System.IO;

namespace LS.WebApp.Controllers.api
{

    //        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/signature")]
    public class SignatureController : BaseApiController
    {
        private static readonly string ProofImageType = "Signatures"; //Value may change, using for testing purposes

        [Route("createTopazImageFile")]
        public async Task<IHttpActionResult> CreateTopazImageFile(ESignature data)
        {

            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = false,Data=null, Error = new ProcessingError("Failed to create image file.", "Failed to create image file.", true, false) };
            Image sigImage;
            try
            {
                SigPlusNET sigPlusNet = new SigPlusNET();
                sigPlusNet.SetSigCompressionMode(1);
                sigPlusNet.SetSigString(data.SigString);
                if (sigPlusNet.NumberOfTabletPoints() > 0)
                {
                    sigPlusNet.SetImageFileFormat(1);
                    sigPlusNet.SetImageXSize(500);
                    sigPlusNet.SetImageYSize(165);
                    sigPlusNet.SetImagePenWidth(8);
                    sigPlusNet.SetJustifyMode(5);
                    //string root = HttpContext.Current.Server.MapPath("/app/main/support/SigTest");
                    //string path = root + "\\signature.png";
                    //sigPlusNet.GetSigImage().Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    //string jpgpath = root + "\\signature.png";
                    //sigPlusNet.GetSigImage().Save(jpgpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    sigImage = sigPlusNet.GetSigImage();
                }
                else
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Signature is not detected.", "Signature is not detected.", true, false);
                    return Ok(processingResult);
                }
            }
            catch (Exception ex)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(ex.Message, ex.Message, true, false);
                return Ok(processingResult);
            }

            var credentialService = new ExternalStorageCredentialsDataService();
            var getCredentialsResult = await credentialService.GetProofImageStorageCredentialsFromCompanyId(LoggedInUser.CompanyId, ProofImageType);
            if (!getCredentialsResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.STORAGE_CREDENTIAL_ERROR;
                return Ok(processingResult);
            }
            var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
            string filename = "TopazTestSignature" + DateTime.Now.Ticks.ToString();
            var result = externalStorageService.SaveImage(sigImage, filename, "png");
          
            if (!result.IsSuccessful)
            {
                processingResult.Error = result.Error;
                processingResult.IsSuccessful = result.IsSuccessful;
                return Ok(processingResult);
            }


            return Ok(externalStorageService.GeneratePreSignedUrl(filename+".png"));
         
        }
        [Route("createTabletImageFile")]
        public async Task<IHttpActionResult> CreateTabletImageFile(ESignature data)
        {

            var processingResult = new ServiceProcessingResult() { IsSuccessful = false, Error = new ProcessingError("Failed to create image file.", "Failed to create image file.", true, false) };
            var sigToImg = new SignatureToImage();
            Image sigImage = sigToImg.SigJsonToImage(data.SigString,new Size(320, 125));
            //sigImage.Save("c:\\Temp\\test.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);

            var credentialService = new ExternalStorageCredentialsDataService();
            var getCredentialsResult = await credentialService.GetProofImageStorageCredentialsFromCompanyId(LoggedInUser.CompanyId, ProofImageType);
            if (!getCredentialsResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.STORAGE_CREDENTIAL_ERROR;
                return Ok(processingResult);
            }
            var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
            string filename = "TabletTestSignature" + DateTime.Now.Ticks.ToString()+".png";
            var result = externalStorageService.SaveImage(sigImage,filename, "png");
            if (!result.IsSuccessful)
            {
                processingResult.Error = result.Error;
                processingResult.IsSuccessful = result.IsSuccessful;
                return Ok(processingResult);
            }
            processingResult.IsSuccessful = true;
            var retval = externalStorageService.GeneratePreSignedUrl(filename);
            return Ok(retval);
        }

    }
}

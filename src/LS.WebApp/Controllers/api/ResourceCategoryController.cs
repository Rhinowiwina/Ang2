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
using LS.WebApp.Models;
using NLog.Internal;
using LS.Utilities;
using LS.WebApp.Utilities;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api
{

    //        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/resource")]
    public class ResourceCategoryController : BaseApiController
    {
        //set the type of document being searched, values are Proof, Docs. Function is used in another controller with Proofs value
        private static readonly string ProofImageType = "Docs";

        [HttpGet]
        [Route("getResources")]
        public async Task<IHttpActionResult> GetResources()
        {
            var processingResult = new ServiceProcessingResult<List<ResourceCategory>>();
            var companyid = LoggedInUser.CompanyId;
            var DataService = new ResourceCategoryDataService();
            var dataResult = await DataService.GetAllCategoriessAsync(companyid);

            if (!dataResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = dataResult.Error;
                if (processingResult.IsFatalFailure())
                {
                    //Logger.Fatal("A fatal error occurred while getting company resources.");
                    ExceptionlessClient.Default.CreateLog(typeof(ResourceCategoryController).FullName,"A fatal error occurred while getting company resources.","Error").AddTags("Controller Error").Submit();
                    }
                return Ok(processingResult);
            }
            foreach (var category in dataResult.Data)
            {
                var SortedResources = category.Resources.OrderBy(x => x.Name).ToList();
                category.Resources.Clear();
                category.Resources = SortedResources;
            }
            var data = dataResult.Data.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();

            processingResult.Data = data;

            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getResourceURL")]
        public async Task<IHttpActionResult> GetResourceURL(string filename)
        {
            //"Customer-Order-Confirmation-Sheet.pdf";
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };
            var companyId = LoggedInUser.CompanyId;
            var credentialService = new ExternalStorageCredentialsDataService();
            var getCredentialsResult = await credentialService.GetProofImageStorageCredentialsFromCompanyId(companyId, ProofImageType);
            if (!getCredentialsResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.STORAGE_CREDENTIAL_ERROR;
                return Ok(processingResult);
            }
            
            var externalStorageService = new ExternalStorageService(getCredentialsResult.Data);
            return Ok(externalStorageService.GeneratePreSignedUrl(filename));
        }

    }
}

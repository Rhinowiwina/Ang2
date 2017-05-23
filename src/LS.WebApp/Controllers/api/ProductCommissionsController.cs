using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using System;
using LS.Domain;
using LS.Services;
using LS.ApiBindingModels;
using Exceptionless;
using Exceptionless.Models;
using LS.WebApp.CustomAttributes;
namespace LS.WebApp.Controllers.api {
    [SingleSessionAuthorize]

    [RoutePrefix("api/productcommissions")]
    public class ProductCommissionsController : BaseApiController {
        private static readonly string ProductCommissionCreationFailedSystemMessage = "An error occurred during Product Commission creation.";

        private static readonly string ProductCommissionEditFailedsystemMessage = "An error occurred while editing the Product Commission.";

        private static readonly string EditedProductCommissionDoesntExistSystemHelp = "The Product Commission you are attempting to edit no longer exists.  Someone may have deleted this record while you were editting.  Please refresh the page and try again.";

        private static readonly string ProductCommissionValidationFailedSystemMessage = "Product Commission validation failed.";

        [HttpPost]
        [Route("getCommissionsBySalesTeam")]
        public async Task<IHttpActionResult> GetCommissionsBySalesTeam(string SalesTeamID) {
            var processingResult = new ServiceProcessingResult<List<ProductCommissions>> { IsSuccessful = true };

            var productCommissionsService = new ProductCommissionsDataService();
            var productCommissionsResult = await productCommissionsService.GetCommissionsBySalesTeam(SalesTeamID);
            if (!productCommissionsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving product commissions by sales team,", "Error retrieving product commissions by sales team", true, false);
                return Ok(processingResult);
            }
            processingResult.Data = productCommissionsResult.Data;

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("getCommissionsByUserID")]
        public async Task<IHttpActionResult> GetCommissionsByUserID(string UserID) {
            var processingResult = new ServiceProcessingResult<List<ProductCommissions>> { IsSuccessful = true };

            var productCommissionsService = new ProductCommissionsDataService();
            var productCommissionsResult = await productCommissionsService.GetCommissionsByUserID(UserID);
            if (!productCommissionsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving product commissions by sales team,", "Error retrieving product commissions by sales team", true, false);
                return Ok(processingResult);
            }
            processingResult.Data = productCommissionsResult.Data;

            return Ok(processingResult);
        }


        [Route("getCommissionForUpdate")]
        public async Task<IHttpActionResult> GetCommissionForUpdate(string CommissionID) {
            var processingResult = new ServiceProcessingResult<ProductCommissions>();

            var productCommissionsDataService = new ProductCommissionsDataService();
            var getCommissionForUpdateResult = await productCommissionsDataService.GetCommissionForUpdate(CommissionID);
            if (!getCommissionForUpdateResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving Commission for update", "Error retrieving Commission for update", false, false);
                if (getCommissionForUpdateResult.IsFatalFailure()) {
                    //var logMessage = String.Format("A fatal error occurred while retrieving Commissions");
                    //Logger.Fatal(logMessage);
                    ExceptionlessClient.Default.CreateLog(typeof(ProductCommissionsController).FullName,"A fatal error occurred while retrieving Commissions","Error").AddTags("Controller Error").Submit();
                    }
                return Ok(processingResult);
            }

            processingResult.Data = getCommissionForUpdateResult.Data;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("createProductCommission")]
        public async Task<IHttpActionResult> CreateProductCommission(ProductCommissionsCreateBindingModel model) {
            var processingResult = new ServiceProcessingResult() { IsSuccessful = true };
            if (!ModelState.IsValid) {
                var systemHelp = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(ProductCommissionValidationFailedSystemMessage, systemHelp, false, true);
                return Ok(processingResult);
            }
            if (!LoggedInUser.Role.IsAdmin() && !LoggedInUser.Role.IsSuperAdmin()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }

            var productCommissionsDataService = new ProductCommissionsDataService();

            model.Id = Guid.NewGuid().ToString();

            var validCommission = await productCommissionsDataService.ValidateCommissionAvailable(model.Id, model.ProductType, model.RecipientType, model.SalesTeamID, model.RecipientUserId);
            if (!validCommission.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validCommission.Error;
                return Ok(processingResult);
            }
            if (!validCommission.Data) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("There is already a commission setup for the product/recipient type you have selected.", "There is already a commission setup for the product/recipient type you have selected.", false, false);
                return Ok(processingResult);
            }

            var newProductCommission = model.ToProductCommission();
            processingResult = await productCommissionsDataService.AddAsync(newProductCommission);
            if (!processingResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error creating new Product Commission", "Error creating new Product Commission", false, false);
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("editProductCommission")]
        public async Task<IHttpActionResult> EditProductCommission(ProductCommissionsUpdateBindingModel model) {
            var processingResult = new ServiceProcessingResult() { IsSuccessful = true };
            if (!LoggedInUser.Role.IsAdmin() && !LoggedInUser.Role.IsSuperAdmin()) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.PERMISSION_ERROR;
                return Ok(processingResult);
            }
            if (!ModelState.IsValid) {
                var systemHelp = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(ProductCommissionValidationFailedSystemMessage, systemHelp, false, true);
                return Ok(processingResult);
            }
            var productCommissionsDataService = new ProductCommissionsDataService();

            var validCommission = await productCommissionsDataService.ValidateCommissionAvailable(model.Id, model.ProductType, model.RecipientType, model.SalesTeamID, model.RecipientUserId);
            if (!validCommission.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = validCommission.Error;
                return Ok(processingResult);
            }
            if (!validCommission.Data) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("There is already a commission setup for the product/recipient type you have selected.", "There is already a commission setup for the product/recipient type you have selected.", false, false);
                return Ok(processingResult);
            }

            var updatedProductCommission = model.ToProductCommission();

            processingResult = await productCommissionsDataService.UpdateAsync(updatedProductCommission);
            if (!processingResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error editing Product Commission", "Error editing Product Commission", false, false);
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }
    }
}

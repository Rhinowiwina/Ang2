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
using LS.WebApp.Controllers;
using LS.WebApp.Models;
//using NLog.Internal;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api
{

    //        [Authorize]
    //[SingleSessionAuthorize]
    [RoutePrefix("api/company")]
    public class CompanyController : BaseAPIController
	{
        private static readonly string CompanyValidationFailedSystemMessage ="Company validation failed.";

        //[HttpGet]
        //[Route("getAllCompanies")]
        //public async Task<IHttpActionResult> GetAllCompanies() {
        //    var processingResult = new ServiceProcessingResult<List<Company>>();
        //    //var companyId = GetLoggedInUserCompanyId();*/

        //    var dataService = new ApplicationUserDataService();
        //    var getLoggedInUserResult = await dataService.GetAsync(LoggedInUserId);
        //    if (!getLoggedInUserResult.IsSuccessful) {
        //        processingResult.IsSuccessful = false;
        //        processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
        //        return Ok(processingResult);
        //    }
        //    var loggedInUser = getLoggedInUserResult.Data;
        //    var companyDataService = new CompanyDataService();

        //    var getCompaniesResult = await companyDataService.GetAllCompaniesAsync(loggedInUser);

        //    if (!getCompaniesResult.IsSuccessful) {
        //        processingResult.IsSuccessful = false;
        //        processingResult.Error = getCompaniesResult.Error;
        //        if (processingResult.IsFatalFailure()) {
        //            ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName,"A fatal error occurred while getting all companies.","Error").AddTags("Controller Error").Submit();
        //            //Logger.Fatal("A fatal error occurred while getting all companies.");
        //        }
        //        return Ok(processingResult);
        //    }

        //    var companies = getCompaniesResult.Data;
        //    //var data = users.Select(user => user.ToUserViewBindingModel()).ToList();

        //    processingResult.IsSuccessful = true;
        //    processingResult.Data = companies.OrderBy(a => a.Name).ToList();

        //    return Ok(processingResult);
        //}

        //[HttpGet]
        //[Route("getEnvironment")]
        //public async Task<IHttpActionResult> GetEnvironment() {
        //    var processingResult = new ServiceProcessingResult<String>();
        //    string result = ConfigurationSettings.AppSettings["Environment"];
        //    processingResult.IsSuccessful = true;
        //    processingResult.Data = result;

        //    return Ok(processingResult);
        //}

        [HttpGet]
        [Route("getCompany")]
        public async Task<IHttpActionResult> GetCompany(string companyId) {
            var processingResult = new ServiceProcessingResult<Company>();

            var dataService = new CompanyDataService();
            var getCompanyResult = await dataService.GetCompanyAsync(companyId);

			if (!getCompanyResult.IsSuccessful)
			{
				processingResult.IsSuccessful = false;
				processingResult.Error = new ProcessingError("test", "test", true, false);// getCompanyResult.Error;
				if (getCompanyResult.IsFatalFailure())
				{
					//var logMessage =
					//    String.Format("A fatal error occurred while getting Company with Id: {0}", companyId);

					//Logger.Fatal(logMessage);
					ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, String.Format("A fatal error occurred while getting Company with Id: {0}", companyId), "Error").AddTags("Controller Error").Submit();
				}
				return Ok(processingResult);
			}

			var company = getCompanyResult.Data;

			processingResult.IsSuccessful = true;
			processingResult.Data = company;

            return Ok(processingResult);
        }

		//[HttpPost]
		//[Route("editCompany")]
		//public async Task<IHttpActionResult> EditCompany(Company model) {
		//    var processingResult = new ServiceProcessingResult() { IsSuccessful = true };
		//    if (!ModelState.IsValid) {
		//        var companyHelp = GetModelStateErrorsAsString(ModelState);
		//        processingResult.IsSuccessful = false;
		//        processingResult.Error = new ProcessingError(CompanyValidationFailedSystemMessage, companyHelp, false, true);
		//        return Ok(processingResult);
		//    }
		//    var companydataService = new CompanyDataService();
		//    var upDatedCompany = model;
		//    processingResult = await companydataService.UpdateCompany(upDatedCompany);
		//    if (!processingResult.IsSuccessful) {
		//        processingResult.IsSuccessful = false;
		//        processingResult.Error = ErrorValues.CANNOT_GET_SYTEMS;
		//        return Ok(processingResult);
		//    }

		//    return Ok(processingResult);
		//}
		//[HttpPost]
		//[Route("createCompany")]
		//public async Task<IHttpActionResult> CreateCompany(Company model)
		//{
		//	var processingResult = new ServiceProcessingResult() { IsSuccessful = true };
		//	if (!ModelState.IsValid)
		//	{
		//		var companyHelp = GetModelStateErrorsAsString(ModelState);
		//		processingResult.IsSuccessful = false;
		//		processingResult.Error = new ProcessingError(CompanyValidationFailedSystemMessage, companyHelp, false, true);
		//		return Ok(processingResult);
		//	}
		//	var companydataService = new CompanyDataService();
		//	var newCompany = model;
		//	processingResult = await companydataService.CreateCompany(newCompany);
		//	if (!processingResult.IsSuccessful)
		//	{
		//		processingResult.IsSuccessful = false;
		//		processingResult.Error = ErrorValues.GENERIC_COMPANY_CREATE_ERROR;
		//		return Ok(processingResult);
		//	}

		//	return Ok(processingResult);
		//}

		//public async Task<IHttpActionResult> Delete(string companyId) {

		//    var companyService = new CompanyDataService();
		//    var processingResult = await companyService.MarkCompanyAsDeletedAsync(companyId);

		//    if (processingResult.IsFatalFailure()) {
		//        //var logMessage = String.Format("A fatal error occurred while deleting System with Id: {0}",
		//        //    companyId);
		//        //Logger.Fatal(logMessage);
		//        ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName,String.Format("A fatal error occurred while deleting System with Id: {0}",
		//            companyId),"Error").AddTags("Controller Error").Submit();
		//        }

		//    return Ok(processingResult);
		//}

	}
}

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.Services;
using Microsoft.AspNet.Identity;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services {
    public class CompanyDataService : BaseDataService<Company, string> {
        public override BaseRepository<Company, string> GetDefaultRepository() {
            return new CompanyRepository();
        }

        public ServiceProcessingResult<Company> GetWithSacEntries(string companyId) {
            return GetWhere(c => c.Id == companyId, new[] { "SacEntries"});
        }
        public async Task<ServiceProcessingResult<List<Company>>> GetAllCompaniesAsync(ApplicationUser loggedInUser) {
            var processingResult = new ServiceProcessingResult<List<Company>> { IsSuccessful = true };

            var companies = new List<Company>();

            if (loggedInUser.Role.IsSuperAdmin()) {
                var getAllCompaniesResult = await GetAllWhereAsync(u => u.Name != null);
                if (!getAllCompaniesResult.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USERS_ERROR;
                    return processingResult;
                }

                companies = getAllCompaniesResult.Data;
            }


            processingResult.Data = companies;
            return processingResult;
        }
        public async Task<ServiceProcessingResult<Company>> GetCompanyAsync(string companyId) {
           

            var result = await GetWhereAsync(u => u.Id == companyId);

            if (result.Data == null) {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("No company found", "No company found", true, false);
            }
            //remove systems marked as deleted. Can not do this in includes.
          
            return result;
        }

        public async Task<ServiceProcessingResult<Company>> GetCompanyByNameAsync(string companyName) {
            var result = await GetWhereAsync(c => c.Name == companyName);
            if (result.Data == null) {
                result.IsSuccessful = false;
                result.Error = new ProcessingError("No company found", "No company found", true, false);
            }

            return result;
        }

        public async Task<ServiceProcessingResult> UpdateCompany(Company upDatedCompany) {
            var result = new ServiceProcessingResult<Company>();
            result = await base.UpdateAsync(upDatedCompany);
            if (!result.IsSuccessful) {
                result.Error = ErrorValues.GENERIC_UPDATE_Company_FAILED_ERROR;
            }

            return result;
        }

      

        public async Task<ServiceProcessingResult> CreateCompany(Company newCompany) {
            if (newCompany.Id == null) {
                newCompany.Id = Guid.NewGuid().ToString();
            }
            var processingResult = new ServiceProcessingResult();
            processingResult = await base.AddAsync(newCompany);

            if (!processingResult.IsSuccessful) {
                processingResult.Error = ErrorValues.GENERIC_COMPANY_CREATE_ERROR;
            }

            return processingResult;

        }

        public async Task<ServiceProcessingResult> MarkCompanyAsDeletedAsync(string systemId) {
            var processingResult = new ServiceProcessingResult();

            if (String.IsNullOrWhiteSpace(systemId)) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.DELETE_Company_WITHOUT_ID_ERROR;
                return processingResult;
            }

            using (var contextScope = DbContextScopeFactory.Create()) {

                try {
                    processingResult = GetRepository().Delete(systemId)
                        .ToServiceProcessingResult(ErrorValues.GENERIC_DELETE_COMPANY_ERROR);
                    if (!processingResult.IsSuccessful) {
                        var logMessage = String.Format("An error occurred while deleting SYstem with Id: {0}",
                            systemId);
                        ExceptionlessClient.Default.CreateLog(typeof(CompanyDataService).FullName,String.Format("An error occurred while deleting SYstem with Id: {0}",
                            systemId),"Error").Submit();
                        //Logger.Error(logMessage);
                    }

                    await contextScope.SaveChangesAsync();
                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    ex.ToExceptionless()
                     .SetMessage(String.Format("An error occurred while deleting System with Id: {0}",
                        systemId))
                     .AddTags("Delete Company Error")
                     .MarkAsCritical()
                     .Submit();
                    processingResult.Error = ErrorValues.GENERIC_DELETE_COMPANY_ERROR;
                    //var logMessage = String.Format("An error occurred while deleting System with Id: {0}",
                    //    systemId);
                    //Logger.Error(logMessage, ex);
                }
            }

            return processingResult;
        }
    }
}

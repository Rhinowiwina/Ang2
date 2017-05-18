using System.Threading.Tasks;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Repositories;

namespace LS.Services
{
    public class TempOrderDataService : BaseDataService<TempOrder, string>
    {
        public override BaseRepository<TempOrder, string> GetDefaultRepository()
        {
            return new TempOrderRepository();
        }

        public async Task<ServiceProcessingResult<ValidationResult>> ValidateTempOrderAsync(SubmitOrderBindingModel model, string userId, string companyId)
        {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult();

            var appUserDataService = new ApplicationUserDataService();
            var userResult = await appUserDataService.GetAsync(userId);
            if (!userResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_COULD_NOT_FIND_USER_ERROR;
                return processingResult;
            }

            var user = userResult.Data;

            // Verify user is active
            if (!user.IsActive)
            {
                validationResult.Errors.Add(ErrorValues.CannotTakeOrderWithInactiveUserUserMessage);
            }

            // Verify user has access to company and company is active
            if (user.CompanyId != companyId || user.Company.IsDeleted)
            {
                validationResult.Errors.Add(ErrorValues.CannotTakeOrderOutsideOfCompanyUserMessage);
            }

            if (validationResult.Errors.Count > 0)
            {
                validationResult.IsValid = false;
                processingResult.Data = validationResult;
                return processingResult;
            }

            // Validate all data elements present
            // TODO: does every single element need to be validated? if so what are the rules? just to fit into the DB?
          


            // Validate Compliance Questions
            if (model.HouseholdReceivesLifelineBenefits != model.CustomerReceivesLifelineBenefits)
            {
                validationResult.Errors.Add(ErrorValues.ComplianceQuestionResponsesInvalidUserMessage);
            }

            // QB middle initial must be one letter
            if (!string.IsNullOrEmpty(model.MiddleInitial) && model.MiddleInitial.Length > 1)
            {
                validationResult.Errors.Add(ErrorValues.MiddleInitialMustBeOnlyOneLetterUserMessage);
            }



            // Lifeline Program Id must be valid
            // If lifeline program has stateProgramPrimary requirement, that field must be present
            var lifelineProgramService = new LifelineProgramDataService();
            var lifelineProgramResult = lifelineProgramService.Get(model.LifelineProgramId);
            if (!lifelineProgramResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_LIFELINE_PROGRAMS_ERROR;
                return processingResult;
            }
            var lifelineProgram = lifelineProgramResult.Data;

            if (lifelineProgram == null)
            {
                validationResult.Errors.Add(ErrorValues.InvalidLifelineProgramIdUserMessage);
                validationResult.IsValid = false;
                processingResult.Data = validationResult;
                return processingResult;
            }

            // State program numbers must be there if they're required
            if ((lifelineProgram.RequiredStateProgramId != null && model.StateProgramNumber == null)
                || (lifelineProgram.RequiredSecondaryStateProgramId != null && model.SecondaryStateProgramNumber == null))
            {
                validationResult.Errors.Add(ErrorValues.StateProgramNumberRequiredButMissingUserMessage);
            }

            // based on company, email and contactPhone must be present
            var companyService = new CompanyDataService();
            var companyResult = companyService.Get(companyId);
            if (!companyResult.IsSuccessful || companyResult.Data == null)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.GENERIC_GET_COMPANY_ERROR;
                return processingResult;
            }
            var company = companyResult.Data;

           

            // sales team id must be valid
            var salesTeamService = new SalesTeamDataService(userId);
            var salesTeamResult = salesTeamService.Get(model.SalesTeamId);
            if (!salesTeamResult.IsSuccessful || salesTeamResult.Data == null)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.COULD_NOT_FIND_SALES_TEAM_IN_COMPANY_ERROR;
                return processingResult;
            }
            var salesTeam = salesTeamResult.Data;

            if (!salesTeam.IsActive)
            {
                validationResult.Errors.Add(ErrorValues.SalesTeamForOrderIsInactiveUserMessage);
            }


            validationResult.IsValid = validationResult.Errors.Count == 0;
            processingResult.Data = validationResult;
            return processingResult;

            // todo Address zip codes must match up with city provided??? 
            // todo how do HOH fields from worksheet work if certain questions aren't required??

            // Validate required data is present

        }
    }
}

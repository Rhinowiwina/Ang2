using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Common.Logging;
using LS.Core;
using LS.Core.Interfaces;
using LS.Domain;
using LS.Utilities;
using LS.BudgetMobile.Logging;
using LS.BudgetMobile.BudgetMobileTestApi;
using System.Text.RegularExpressions;
using Exceptionless;
using Exceptionless.Models;
namespace LS.BudgetMobile {
    public class CompanyProviderOptions : ICompanyProviderOptions {
        private static readonly string ApplicationId = "LA1953";
        private static readonly string ApplicationPassword = "uxwri5ph";
        private readonly string _employeeName;
        private readonly int _locationId = 32342;

        private ILog Logger { get; set; }

        public CompanyProviderOptions(string employeeName) {
            Logger = LoggerFactory.GetLogger(GetType());

            //can only send 10 characters
            if (employeeName.Length > 10) {
                _employeeName = employeeName.Substring(0, 10);
            } else { _employeeName = employeeName; }
        }

        public CompanyProviderOptions(string employeeName, string LocationID) {
            Logger = LoggerFactory.GetLogger(GetType());
            //can only send 10 characters
            if (employeeName.Length > 10) {
                _employeeName = employeeName.Substring(0, 10);
            } else { _employeeName = employeeName; }

            _locationId = Int32.Parse(LocationID);
        }

        public ServiceProcessingResult<IDuplicateCheckResult> DuplicateCheck(string FirstName, string LastName, string DOB, string SSN, string LexID) {
            var result = new ServiceProcessingResult<IDuplicateCheckResult> { IsSuccessful = false};
            try {
                var service = new LifelineServicesSoapClient();
                service.Endpoint.Behaviors.Add(new CAMsDuplicateCheckInspectorBehavior());
                var convertedDate = Convert.ToDateTime(DOB);
                var duplicateCheck = service.DuplicateCheck(new Credentials_DuplicateChecks {
                    ApplicationID = ApplicationId,
                    ApplicationPassword = ApplicationPassword,
                    LocationID = _locationId,
                    EmployeeName = _employeeName,
                    FirstName = FirstName,
                    LastName = LastName,
                    DOB = String.Format("{0:MM/dd/yyyy}", convertedDate),
                    SSN = SSN,
                    LexID = LexID
                });

                if (duplicateCheck.IsError) {
                    result.Error = new ProcessingError("CAMs - " + duplicateCheck.Errors.ErrorMessage, "CAMs - " + duplicateCheck.Errors.ErrorMessage, false, false);
                    result.IsSuccessful = false;
                    return result;
                }

                result.IsSuccessful = true;
                result.Data =  new DuplicateCheckResult { IsDuplicate = duplicateCheck.IsDuplicate};

            } catch (Exception ex) {
                ex.ToExceptionless()
                   .SetMessage("Failed to CAMs duplicate check")
                   .MarkAsCritical()
                   .Submit();
                var failureText = "Failed to CAMs duplicate check";
                result.Error = new ProcessingError(failureText, failureText, true);
                Logger.Error(failureText, ex);
            }
            return result;
        }
    }
}

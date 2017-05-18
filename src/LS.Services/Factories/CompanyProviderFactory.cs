using LS.BudgetMobile;
using LS.Core.Interfaces;

namespace LS.Services.Factories
{
    public static class CompanyProviderFactory
    {
        private static ICompanyProviderOptions _providerOptions;
        private static ICompanyProviderValidation _providerValidation;

        public static ICompanyProviderValidation GetValidation(string companyId)
        {
            if (_providerValidation == null)
            {
                _providerValidation = new CompanyProviderValidation();
            }
            return _providerValidation;
        }

        public static ICompanyProviderOptions GetOptions(string companyId, string employeeName)
        {
            if (_providerOptions == null)
            {
                _providerOptions = new CompanyProviderOptions(employeeName);
            }
            return _providerOptions;
        }

        public static ICompanyProviderOptions GetOptions(string companyId, string employeeName, string LocationID)
        {
            _providerOptions = new CompanyProviderOptions(employeeName, LocationID);
            return _providerOptions;
        }
    }
}

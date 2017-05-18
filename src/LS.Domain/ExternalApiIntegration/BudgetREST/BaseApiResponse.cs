using System.Collections.Generic;
using LS.Domain.ExternalApiIntegration.Interfaces;

namespace LS.Domain.ExternalApiIntegration.BudgetREST
{
    public class BaseApiResponse
    {
        public bool IsError { get; set; }
        public string ErrorMessage{ get; set; }
        public string ErrorCode { get; set; }
    }
}

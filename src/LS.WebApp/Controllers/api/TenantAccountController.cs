using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using System;
using LS.Services.Factories;
using LS.Core.Interfaces;
using LS.WebApp.CustomAttributes;
namespace LS.WebApp.Controllers.api
{

    [SingleSessionAuthorize]
    [RoutePrefix("api/tenantaccount")]
    public class TenantAccountController : TenantController
    {
        
    }
}

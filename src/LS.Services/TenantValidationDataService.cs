using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using LS.Core;
using LS.Core.Interfaces;
using LS.Domain;
using LS.Services.Factories;
using LS.ApiBindingModels;
using Common.Logging;
using LS.Utilities;
using System.Configuration;
using LS.Repositories;
using Exceptionless;
using Exceptionless.Models;
namespace LS.Services
{
    public class TenantValidationDataService {

        protected ILog Logger { get; set; }

        public TenantValidationDataService()
        {
            Logger = LoggerFactory.GetLogger(GetType());
        }
    }
}


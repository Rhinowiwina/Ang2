using LS.Domain;
using LS.Repositories;
using LS.Core;
using System.Threading.Tasks;
using LS.ApiBindingModels;
using System.Configuration;
using System;

namespace LS.Services {
    public class UtilityDataService {
        public async Task<ServiceProcessingResult<ServerEnvironmentBindingModel>> GetServerVars() {
            var processingResult = new ServiceProcessingResult<ServerEnvironmentBindingModel> { IsSuccessful = true };
            ServerEnvironmentBindingModel model = new ServerEnvironmentBindingModel();
            try {
                var environment = ConfigurationManager.AppSettings["Environment"].ToString();
                var machine = ConfigurationManager.AppSettings["IsDeveloperMachine"].ToString();
                var version = ConfigurationManager.AppSettings["AppVersion"].ToString();
                var vKey = ConfigurationManager.AppSettings["JSExceptionlessKey"].ToString();

                switch (environment) {
                    case "DEV":
                        model.IsDev = true;
                        break;
                    case "Staging":
                        model.IsStaging = true;
                        break;
                    case "PROD":
                        model.IsProd = true;
                        break;
                }
                model.Environment = environment;
                model.IsDeveloperMachine = Convert.ToBoolean(machine);
                model.Version = version;
                model.JSExceptionlessKey = vKey;
                processingResult.Data = model;
            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(ex.Message, ex.Message, true, false);
                return processingResult;

            }
            return processingResult;
        }
    }
}

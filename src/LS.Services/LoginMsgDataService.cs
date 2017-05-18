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
    public class LoginMsgDataService : BaseDataService<LoginMsg, string> {
        public override BaseRepository<LoginMsg, string> GetDefaultRepository() {
            return new LoginMsgRepository();
        }

       
        public async Task<ServiceProcessingResult<List<LoginMsg>>> GetActiveMsg() {
            var processingResult = new ServiceProcessingResult<List<LoginMsg>> { IsSuccessful = true };

            var messages = new List<LoginMsg>();
         
            using (var contextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    var repository = new LoginMsgRepository();
                    processingResult = repository.GetActiveMsg().ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(processingResult.Error.UserMessage);
                        return processingResult;
                    }

                    await contextScope.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error("An error occurred while retrieving login messages.", ex);
                    ExceptionlessClient.Default.CreateLog(typeof(LoginMsgDataService).FullName,"An error occurred while retrieving login messages.","Error").AddTags("Data Service Error").Submit();
                    return processingResult;
                }
            }
            return processingResult;
        }
       
       public async Task<ServiceProcessingResult<List<LoginMsg>>> GetAllMsg()
        {
            var processingResult = new ServiceProcessingResult<List<LoginMsg>> { IsSuccessful = true };

            var messages = new List<LoginMsg>();

            using (var contextScope = DbContextScopeFactory.Create())
            {
                try
                {
                    var repository = new LoginMsgRepository();
                    processingResult = repository.GetAllMsg().ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);

                    if (!processingResult.IsSuccessful)
                    {
                        Logger.Error(processingResult.Error.UserMessage);
                        return processingResult;
                    }

                    await contextScope.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error("An error occurred while retrieving login messages.", ex);
                    return processingResult;
                }
            }
            return processingResult;
        }
        public async Task<ServiceProcessingResult<LoginMsg>> UpdateMessage(LoginMsg message)
            {
            var processingResult = new ServiceProcessingResult<LoginMsg> { IsSuccessful = true };

       

            using (var contextScope = DbContextScopeFactory.Create())
                {
                try
                    {
                    var repository = new LoginMsgRepository();
                    processingResult = repository.UpdateMessage(message).ToServiceProcessingResult(ErrorValues.GENERIC_FATAL_BACKEND_ERROR);
                    await contextScope.SaveChangesAsync();
                    if (!processingResult.IsSuccessful)
                        {
                        Logger.Error(processingResult.Error.UserMessage);
                        return processingResult;
                        }
                                     
                    }
                catch (Exception ex)
                    {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error("An error occurred while updating messages.",ex);
                    return processingResult;
                    }
                }
            return processingResult;
            }
        public async Task<ServiceProcessingResult> DeleteMessage(string messageId)
            {
            var processingResult = new ServiceProcessingResult() { IsSuccessful = true };

          

            using (var contextScope = DbContextScopeFactory.Create())
                {
                try
                    {
                    var repository = new LoginMsgRepository();
                    var result = repository.DeleteMessage(messageId);
                     

                    if (!result.IsSuccessful)
                        {
                        processingResult.IsSuccessful = false;
                        Logger.Error(result.Error.UserMessage);
                        return processingResult;
                        }

                    await contextScope.SaveChangesAsync();
                    }
                catch (Exception ex)
                    {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = ErrorValues.GENERIC_FATAL_BACKEND_ERROR;
                    Logger.Error("An error occurred while deleting login messages.",ex);
                    return processingResult;
                    }
                }
            
            return processingResult;
            }


        }
}

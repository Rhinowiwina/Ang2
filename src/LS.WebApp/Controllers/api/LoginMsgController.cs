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
using LS.WebApp.Models;
//using NLog.Internal;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp.Controllers.api
{

    //        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/loginMsg")]
    public class LoginMsgController : BaseAPIController
    {


        [HttpGet]
        [Route("getActiveMessages")]
        public async Task<IHttpActionResult> GetActiveMessages()
        {

            var processingResult = new ServiceProcessingResult<List<LoginMsg>> { IsSuccessful = true };
            var dataService = new LoginMsgDataService();
            var getMessagersResult = await dataService.GetActiveMsg();
            if (!getMessagersResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(getMessagersResult.Error.UserMessage, getMessagersResult.Error.UserMessage, true, false);
                return Ok(processingResult);
            }
            processingResult.Data = getMessagersResult.Data;
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }
        [HttpGet]
        [Route("getAllMessages")]
        public async Task<IHttpActionResult> GetAllMessages()
        {

            var processingResult = new ServiceProcessingResult<List<LoginMsg>> { IsSuccessful = true };
            var dataService = new LoginMsgDataService();
            var getMessagersResult = await dataService.GetAllMsg();
            if (!getMessagersResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(getMessagersResult.Error.UserMessage, getMessagersResult.Error.UserMessage, true, false);
                return Ok(processingResult);
            }
            processingResult.Data = getMessagersResult.Data;
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }
        [HttpGet]
        [Route("getMsgToEdit")]
        public async Task<IHttpActionResult> GetMsgToEdit(string messageId)
        {

            var processingResult = new ServiceProcessingResult<LoginMsg> { IsSuccessful = true };
            var dataService = new LoginMsgDataService();
            var getMessagesResult =  dataService.Get(messageId);
            if (!getMessagesResult.IsSuccessful)
            {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(getMessagesResult.Error.UserMessage, getMessagesResult.Error.UserMessage, true, false);
                return Ok(processingResult);
            }
            processingResult.Data = getMessagesResult.Data;
            processingResult.IsSuccessful = true;
            return Ok(processingResult);
        }
        [HttpPost]
        [Route("editMessage")]
        public async Task<IHttpActionResult> EditMessage(LoginMsgBindingModel model)
            {
            var processingResult = new ServiceProcessingResult<LoginMsg>() { IsSuccessful = true };
            var dataService = new LoginMsgDataService();
           if (!ModelState.IsValid)
                {
                var messageHelp = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Validation of message failed.",messageHelp,false,true);
                return Ok(processingResult);
                }

            var updatedMessage = model.ToLoginMsg();
        
            processingResult = await dataService.UpdateMessage(updatedMessage);

            if (!processingResult.IsSuccessful)
                {
                //var logMessage =
                //String.Format("A fatal error occurred while editing message with Id: {0}.",model.Id);
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(LoginMsgController).FullName,String.Format("A fatal error occurred while editing message with Id: {0}.",model.Id),"Error").AddTags("Controller Error").Submit();
                return Ok(processingResult);
                }
           

               return Ok(processingResult);
            }

        [HttpPost]
        [Route("createMessage")]
        public async Task<IHttpActionResult> CreateMessage(LoginMsgBindingModel model)
            {
            var processingResult = new ServiceProcessingResult<LoginMsg>() { IsSuccessful = true };
            var dataService = new LoginMsgDataService();
            if (!ModelState.IsValid)
                {
                var messageHelp = GetModelStateErrorsAsString(ModelState);
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Validation of message failed.",messageHelp,false,true);
                return Ok(processingResult);
                }

            var newMessage = model.ToNewLoginMsg();

            processingResult = await dataService.AddAsync(newMessage);

            if (!processingResult.IsSuccessful)
                {
                ExceptionlessClient.Default.CreateLog(typeof(LoginMsgController).FullName,"A fatal error occurred while inserting message.","Error").AddTags("Controller Error").Submit();
                //Logger.Fatal("A fatal error occurred while inserting message.");
                return Ok(processingResult);
                }


            return Ok(processingResult);
            }


        public async Task<IHttpActionResult> Delete(string messageId)//Permanently removes record.
            {
            var processingResult = new ServiceProcessingResult();
            var messageService = new LoginMsgDataService();
             processingResult = await messageService.DeleteMessage(messageId);

            if (processingResult.IsFatalFailure())
                {
                var logMessage = String.Format("A fatal error occurred while deleting Message with Id: {0}",
                    messageId);
              
                ExceptionlessClient.Default.CreateLog(typeof(LoginMsgController).FullName,String.Format("A fatal error occurred while deleting Message with Id: {0}",messageId),"Error").AddTags("Controller Error").Submit();
                }

            return Ok(processingResult);
            }

        }
}

       
      
       



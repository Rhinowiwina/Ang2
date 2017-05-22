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
using NLog.Internal;
using Exceptionless;
using Exceptionless.Models;
using System.Data.SqlClient;
using System.Data;
using LS.Utilities;
using LS.Domain.ExternalApiIntegration;
namespace LS.WebApp.Controllers.api {

    //        [Authorize]
    [RoutePrefix("api/addressValidation")]
    public class AddressValidationController : BaseApiController {
        [HttpGet]
        [Route("getAllAddresses")]
        public async Task<IHttpActionResult> GetAllAddresses() {
            var processingResult = new ServiceProcessingResult<List<AddressValidation>>();
            var cmdText = @"
               SELECT * FROM AddressValidations
            ";

            var sqlQueryHelper = new SQLQuery();
            var getAddresses = await sqlQueryHelper.ExecuteReaderAsync<AddressValidation>(CommandType.Text, cmdText);
            if (!getAddresses.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while getting whitelisted addresses.", "An error occurred while getting whitelisted addresses.", false, false);
                ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, "An error occurred while getting whitelisted addresses.", "Error").AddTags("Controller Error").Submit();
                return Ok(processingResult);
            }

            processingResult.Data = (List<AddressValidation>)getAddresses.Data;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getAddressForEdit")]
        public async Task<IHttpActionResult> GetAddressForEdit(string addressId) {
            var processingResult = new ServiceProcessingResult<AddressValidation>();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id",addressId)
            };

            var strQuery = @"
               SELECT * FROM AddressValidations WHERE Id=@Id
            ";

            var sqlQueryHelper = new SQLQuery();
            var getAddress = await sqlQueryHelper.ExecuteReaderAsync<AddressValidation>(CommandType.Text, strQuery, parameters);
            if (!getAddress.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while getting the whitelisted address.", "An error occurred while getting the whitelisted address.", false, false);
                ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, "An error occurred while getting the whitelisted address.", "Error").AddTags("Controller Error").Submit();
                return Ok(processingResult);
            }
            var theAddress = (List<AddressValidation>)getAddress.Data;
            processingResult.Data = theAddress != null ? theAddress[0] : null;//only one record should be returned,check if null
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("createAddress")]
        public async Task<IHttpActionResult> CreateAddress(AddressValidation model) {
            var processingResult = new ServiceProcessingResult<bool>() { IsSuccessful = true };
            var validateAddressService = new AddressValidationDataService();
            var addressToValidate = new AddressStandardizeRequest {
                Address = model.Street1,
                Address2 = model.Street2,
                City = model.City,
                State = model.State,
                Zip = model.Zipcode
            };
            var validateAddressResult = await validateAddressService.Standardize(addressToValidate);//Hard coded california api, arrow is in CA only.
            ////Check validateAddressResult.IsSuccessful.  If success, set model address fields to returned values.  
            if (!validateAddressResult.IsSuccessful) {
                processingResult.IsSuccessful = validateAddressResult.IsSuccessful;
                processingResult.Error = validateAddressResult.Error;
                return Ok(processingResult);
            }


            if (!validateAddressResult.IsSuccessful) {
                processingResult.IsSuccessful = validateAddressResult.IsSuccessful;
                processingResult.Error = validateAddressResult.Error;
                return Ok(processingResult);
            }

            if (!validateAddressResult.Data.IsValid) {
                processingResult.IsSuccessful = false;

                var fullerror = "";
                foreach (var error in validateAddressResult.Data.ValidationRejections) {
                    fullerror += error + "<br>";
                }
                processingResult.Error = new ProcessingError("", fullerror, false, true);
                return Ok(processingResult);
            }


            var scrubbedAddress = validateAddressResult.Data;


            var newId = Guid.NewGuid().ToString();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", newId),
                new SqlParameter("@Street1",scrubbedAddress.Address1),
                new SqlParameter("@Reason",model.Reason),
                new SqlParameter("@Street2",scrubbedAddress.Address2),
                new SqlParameter("@City",scrubbedAddress.City),
                new SqlParameter("@State",scrubbedAddress.State),
                new SqlParameter("@Zipcode",scrubbedAddress.Zip),
                new SqlParameter("@IsShelter",model.IsShelter),
                new SqlParameter("@DateCreated",DateTime.Now),
                new SqlParameter("@DateModified",DateTime.Now),
                new SqlParameter("@ModifiedByUserID",LoggedInUser.Id),
                };
            var strQuery = @"
               INSERT INTO ADDRESSVALIDATIONS (Id,Street1,Street2,City,Reason,State,Zipcode,IsShelter,DateCreated,DateModified,ModifiedByUserID) VALUES (@Id,@Street1,@Street2,@City,@Reason,@State,@Zipcode,@IsShelter,@DateCreated,@DateModified,@ModifiedByUserID)
            ";

            var sqlQueryHelper = new SQLQuery();
            var result = await sqlQueryHelper.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
            if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while adding the whitelisted address.", "An error occurred while adding the whitelisted address.", false, false);
                ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, "An error occurred while adding the whitelisted address.", "Error").AddTags("Controller Error").Submit();
                return Ok(processingResult);
            }
            if (result.Data < 1) {
                processingResult.Data = false;
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to add Address to white list.", "Failed to add Address to white list.", true, false);
                return Ok(processingResult);

            }
            processingResult.Data = true;
            processingResult.IsSuccessful = true;
            //Insert Log if it fails to insert we go on but notify exceptionless
            var vReason = new ReasonAuditField() { Reason = "" };//first record so there is no previous data
            SqlParameter[] parameters1 = new SqlParameter[] {
                new SqlParameter("ModifiedByUserID", LoggedInUser.Id),
                new SqlParameter("TableName", "AddressValidations"),
                new SqlParameter("TableRowID",newId),
                new SqlParameter("TablePreviousData", Utils.ToJSON(vReason)),
                new SqlParameter("Reason", model.Reason)
            };
            var sqlText = @"
                INSERT INTO AuditLogs (
                    Id,ModifiedByUserID,DateCreated,
                    TableName,TableRowID,TablePreviousData,
                    Reason
                ) VALUES(
                    newid(), @ModifiedByUserID, getdate(),
                    @TableName, @TableRowID, @TablePreviousData,
                    @Reason
                )
            ";
            var sqlQuery1 = new SQLQuery();
            var insertAuditLogResult = await sqlQuery1.ExecuteNonQueryAsync(CommandType.Text, sqlText, parameters1);
            if (!insertAuditLogResult.IsSuccessful) {
                ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, "Failed to insert AuditLog for AddressValidation", "Error").AddTags("Controller Error").AddObject(parameters).Submit();
            }
            return Ok(processingResult);
        }

        [HttpPost]
        [Route("editAddress")]
        public async Task<IHttpActionResult> EditAddress(AddressValidation model) {
            var processingResult = new ServiceProcessingResult<bool>() { IsSuccessful = true };

            var validateAddressService = new AddressValidationDataService();
            var addressToValidate = new AddressStandardizeRequest {
                Address = model.Street1,
                Address2 = model.Street2,
                City = model.City,
                State = model.State,
                Zip = model.Zipcode
            };

            var validateAddressResult = await validateAddressService.Standardize(addressToValidate);//Hard coded california api, arrow is in CA only.
            ////Check validateAddressResult.IsSuccessful.  If success, set model address fields to returned values.  
            if (!validateAddressResult.IsSuccessful) {
                processingResult.IsSuccessful = validateAddressResult.IsSuccessful;
                processingResult.Error = validateAddressResult.Error;
                return Ok(processingResult);
            }

            if (!validateAddressResult.IsSuccessful) {
                processingResult.IsSuccessful = validateAddressResult.IsSuccessful;
                processingResult.Error = validateAddressResult.Error;
                return Ok(processingResult);
            }

            if (!validateAddressResult.Data.IsValid) {
                processingResult.IsSuccessful = false;

                var fullerror = "";
                foreach (var error in validateAddressResult.Data.ValidationRejections) {
                    fullerror += error + "<br>";
                }
                processingResult.Error = new ProcessingError("", fullerror, false, true);
                return Ok(processingResult);
            }

            var scrubbedAddress = validateAddressResult.Data;

            var sqlQuery1 = new SQLQuery();

            SqlParameter[] parameters1 = new SqlParameter[] {
                new SqlParameter("Id", model.Id)
            };

            var sqlText = @"SELECT Reason FROM ADDRESSVALIDATIONS WHERE Id=@Id";

            var getCurrentAddressVaidationReason = await sqlQuery1.ExecuteReaderAsync<ReasonAuditField>(CommandType.Text, sqlText, parameters1);
            if (!getCurrentAddressVaidationReason.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting audit table data from AddressValidation.", "Error getting audit table data from getCurrentAddressVaidationReason.", false, false);
                return Ok(processingResult);
            }

            var auditTableList = (List<ReasonAuditField>)getCurrentAddressVaidationReason.Data;
            var tableDataJSON = auditTableList[0];
            SqlParameter[] parameters2 = new SqlParameter[] {
                new SqlParameter("ModifiedByUserID", LoggedInUser.Id),
                new SqlParameter("TableName", "AddressValidations"),
                new SqlParameter("TableRowID",model.Id),
                new SqlParameter("TablePreviousData", Utils.ToJSON(tableDataJSON)),
                new SqlParameter("Reason", model.Reason)
            };

            var sqlText2 = @"
                INSERT INTO AuditLogs (
                    Id,ModifiedByUserID,DateCreated,
                    TableName,TableRowID,TablePreviousData,
                    Reason
                ) VALUES(
                    newid(), @ModifiedByUserID, getdate(),
                    @TableName, @TableRowID, @TablePreviousData,
                    @Reason
                )
            ";

            var sqlQuery2 = new SQLQuery();
            var insertAuditLogResult = await sqlQuery2.ExecuteNonQueryAsync(CommandType.Text, sqlText2, parameters2);
            if (!insertAuditLogResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to insert audit log.", "Failed to insert audit log.", true, false);
                ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, "Failed to insert AuditLog for AddressValidation", "Error").AddTags("Controller Error").AddObject(parameters2).Submit();
                return Ok(processingResult);
            }

            //edit record
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@Reason",model.Reason),
                new SqlParameter("@Street1",scrubbedAddress.Address1),
                new SqlParameter("@Street2",scrubbedAddress.Address2),
                new SqlParameter("@City",scrubbedAddress.City),
                new SqlParameter("@State",scrubbedAddress.State),
                new SqlParameter("@Zipcode",scrubbedAddress.Zip),
                new SqlParameter("@IsShelter",model.IsShelter),
                new SqlParameter("@DateModified",DateTime.Now),
                new SqlParameter("@ModifiedByUserID",LoggedInUser.Id),
                };
            var strQuery = @"
               UPDATE ADDRESSVALIDATIONS Set Street1=@Street1,Street2=@Street2,City=@City,State=@State,ZipCode=@Zipcode,IsShelter=@IsShelter,DateModified=@DateModified,Reason=@Reason,ModifiedbyUserID=@ModifiedByUserID WHERE Id=@Id";

            var sqlQueryHelper = new SQLQuery();
            var result = await sqlQueryHelper.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
            if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while editing the whitelisted address.", "An error occurred while editing the whitelisted address.", false, false);
                ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, "An error occurred while editing the whitelisted address.", "Error").AddTags("Controller Error").Submit();
                return Ok(processingResult);
            }
            if (result.Data < 1) {
                processingResult.Data = false;
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to edit Address.", "Failed to edit Address.", true, false);
                return Ok(processingResult);

            }
            processingResult.Data = true;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("deleteAddress")]
        public async Task<IHttpActionResult> DeleteAddress(string addressid) {
            var processingResult = new ServiceProcessingResult<bool>() { IsSuccessful = true };
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", addressid),
            };

            var strQuery = @"DELETE FROM ADDRESSVALIDATIONS WHERE Id=@Id";

            var sqlQueryHelper = new SQLQuery();
            var result = await sqlQueryHelper.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
            if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred while deleting the whitelisted address.", "An error occurred while deleting the whitelisted address.", false, false);
                ExceptionlessClient.Default.CreateLog(typeof(CompanyController).FullName, "An error occurred while deleting the whitelisted address.", "Error").AddTags("Controller Error").Submit();
                return Ok(processingResult);
            }
            if (result.Data < 1) {
                processingResult.Data = false;
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to delete Address.", "Failed to delete Address.", true, false);
                return Ok(processingResult);

            }
            processingResult.Data = true;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

    }
    public class ReasonAuditField {
        public string Reason { get; set; }
    }
}

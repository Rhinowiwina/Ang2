using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity.Owin;
using LS.ApiBindingModels;
using LS.Core;
using LS.Domain;
using LS.Services;
using LS.WebApp.CustomAttributes;
using LS.WebApp.Models;
using System.Net;
using System.Net.Mail;
using LS.Utilities;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Exceptionless;
using Exceptionless.Models;
using OfficeOpenXml;
using System.Configuration;

namespace LS.WebApp.Controllers.api {
    //        [Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/deviceOrder")]
    public class DeviceOrderController : BaseApiController {

        [Route("getDeviceOrderForEdit")]
        public async Task<IHttpActionResult> getDeviceOrderForEdit(string deviceOrderID) {
            var processingResult = new ServiceProcessingResult<DeviceOrderResponse>() { IsSuccessful = true };
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", deviceOrderID),
            };
            var sqlQueryHelper = new SQLQuery();
            var cmdText = @"
                SELECT DO.*, L1.Name AS Level1SalesGroupName
                FROM DeviceOrders DO
	                LEFT JOIN Level1SalesGroup L1 ON L1.Id=DO.Level1SalesGroupID
                WHERE DO.Id=@Id
            ";
            var getDeviceOrderResult = await sqlQueryHelper.ExecuteReaderAsync<DeviceOrderResponse>(CommandType.Text, cmdText, parameters);
            if (!getDeviceOrderResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.CANNOT_GET_ORDERS;
                return Ok(processingResult);
            }

            var deviceOrder = (List<DeviceOrderResponse>)getDeviceOrderResult.Data;
            processingResult.Data = deviceOrder[0];
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }
        
        [Route("createDeviceOrder")]
        public async Task<IHttpActionResult> createDeviceOrder(DeviceOrderCreationBindingModel model) {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", Guid.NewGuid().ToString()),
                new SqlParameter("@PONumber", model.PONumber),
                new SqlParameter("@InvoiceNumber", model.InvoiceNumber),
                new SqlParameter("@OrderDate", model.OrderDate),
                new SqlParameter("@ShipDate", model.ShipDate),
                new SqlParameter("@AgentDueDate", model.AgentDueDate),
                new SqlParameter("@ASGDueDate", model.ASGDueDate),
                new SqlParameter("@Level1SalesGroupID", model.Level1SalesGroupID),
                new SqlParameter("@OrderReturned", model.OrderReturnedIndicator),
            };
            var sqlQueryHelper = new SQLQuery();
            var cmdText = @"
                INSERT INTO DeviceOrders (Id, PONumber, InvoiceNumber, OrderDate, ShipDate, AgentDueDate, ASGDueDate, Level1SalesGroupID, IsReturned) VALUES (@Id,@PONumber, @InvoiceNumber, @OrderDate, @ShipDate, @AgentDueDate, @ASGDueDate, @Level1SalesGroupID, @OrderReturned)
            ";
            var insertDeviceOrder = await sqlQueryHelper.ExecuteNonQueryAsync(CommandType.Text, cmdText, parameters);
            if (!insertDeviceOrder.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error creating new handset order", "Error creating new handset order", false, false);
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }

        [Route("editDeviceOrder")]
        public async Task<IHttpActionResult> editDeviceOrder(DeviceOrderUpdateBindingModel model) {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@PONumber", model.PONumber),
                new SqlParameter("@InvoiceNumber", model.InvoiceNumber),
                new SqlParameter("@OrderDate", model.OrderDate),
                new SqlParameter("@ShipDate", model.ShipDate),
                new SqlParameter("@AgentDueDate", model.AgentDueDate),
                new SqlParameter("@ASGDueDate", model.ASGDueDate),
                new SqlParameter("@Level1SalesGroupID", model.Level1SalesGroupID),
                new SqlParameter("@OrderReturned", model.OrderReturnedIndicator),
            };
            var sqlQueryHelper = new SQLQuery();
            var cmdText = @"
                UPDATE DeviceOrders SET PONumber=@PONumber, InvoiceNumber=@InvoiceNumber, OrderDate=@OrderDate, ShipDate=@ShipDate, AgentDueDate=@AgentDueDate, ASGDueDate=@ASGDueDate, Level1SalesGroupID=@Level1SalesGroupID, IsReturned=@OrderReturned WHERE Id=@Id
            ";
            var updateDeviceOrder = await sqlQueryHelper.ExecuteNonQueryAsync(CommandType.Text, cmdText, parameters);
            if (!updateDeviceOrder.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error editing handset order", "Error editing handset order", false, false);
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }

		[Route("getAllDeviceOrders")]
		public async Task<IHttpActionResult> getAllDeviceOrders(string startDate, string endDate)
		{
			var processingResult = new ServiceProcessingResult<List<DeviceOrderResponse>>() { IsSuccessful = true };
			DateTime vstartDate;
			DateTime vendDate;
			var isStartDateValid = DateTime.TryParse(startDate, out vstartDate);
			var isEndDateValid = DateTime.TryParse(endDate, out vendDate);
			if (isStartDateValid && isEndDateValid)
			{
				if (vendDate < vstartDate)
				{
					processingResult.IsSuccessful = false;
					processingResult.Error = new ProcessingError("Start Date is greater than End Date.", "Start Date is greater than End Date.", true, false);
					return Ok(processingResult);
				}
			}
			var sqlQueryHelper = new SQLQuery();
			var parametersList = new List<SqlParameter>();

			if (isStartDateValid)
				{
				parametersList.Add(new SqlParameter("@StartDate", vstartDate));
				}

			if (isEndDateValid)
			{
				parametersList.Add(new SqlParameter("@EndDate", vendDate));
			}
			var parameters = parametersList.ToArray();

			var selectText = @"
                SELECT DO.*, L1.Name AS Level1SalesGroupName
                FROM DeviceOrders DO
                 LEFT JOIN Level1SalesGroup L1 ON L1.Id=DO.Level1SalesGroupID
	            ";
			string whereText = "WHERE 1=1";
			if (isStartDateValid)
			{
				whereText += @" AND DO.OrderDate >= @StartDate";
			}
			if (isEndDateValid)
			{
				whereText += @" AND DO.OrderDate <= @EndDate";
			}
			var orderByText = @" ORDER BY DO.PONumber DESC";

			var cmdText = selectText + whereText + orderByText;

			var getDeviceOrdersResult = await sqlQueryHelper.ExecuteReaderAsync<DeviceOrderResponse>(CommandType.Text, cmdText,parameters);
            if (!getDeviceOrdersResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.CANNOT_GET_ORDERS;
                return Ok(processingResult);
            }
            processingResult.Data = (List<DeviceOrderResponse>)getDeviceOrdersResult.Data;
            processingResult.IsSuccessful = true;

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getLevel1Groups")]
        public async Task<IHttpActionResult> GetLevel1Groups()
        {
            var processingResult = new ServiceProcessingResult<List<GroupSimpleViewBindingModel>> { IsSuccessful = true };

            var sqlQuery = new SQLQuery();

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@CompanyID", LoggedInUser.CompanyId)
            };

            var sqlText = @"
                SELECT Id, Name
                FROM Level1SalesGroup 
                WHERE CompanyID=@CompanyID AND IsDeleted=0
            ";

            var getLevel1Result = await sqlQuery.ExecuteReaderAsync<GroupSimpleViewBindingModel>(CommandType.Text, sqlText, parameters);
            if (!getLevel1Result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting level 1 groups", "Error getting level 1 groups", false, false);
                return Ok(processingResult);
            }

            processingResult.Data = (List<GroupSimpleViewBindingModel>)getLevel1Result.Data;

            return Ok(processingResult);
        }

        [Route("uploadDevices")]
        [HttpPost]
        public async Task<IHttpActionResult> uploadDevices()
        {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };

            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data"))) {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/App_Data"));
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            var data = await Request.Content.ReadAsMultipartAsync(provider);

            var model = new UploadDevicesBindingModel();

            // Show all the key-value pairs.
            foreach (var key in provider.FormData.AllKeys) {
                foreach (var val in provider.FormData.GetValues(key)) {
                    Utils.SetObjectProperty(key, val, model);
                }
            }

            try {
                Stream excelStream = null;

                foreach (MultipartFileData fileData in provider.FileData) {
                    excelStream = new MemoryStream(File.ReadAllBytes(fileData.LocalFileName));

                    var excel = new ExcelPackage(excelStream);
                    var dt = Utils.ExcelToDataTable(excel, "Data");
                    dt.Columns.Add("OrderID", typeof(System.String));
                    foreach (DataRow row in dt.Rows) {row["OrderID"] = model.OrderID;}

                    var destTable = "DeviceOrderDevices";

                    var sqlQuery = new SQLQuery();

                    if (Convert.ToBoolean(model.ClearOrders)) {
                        var strQuery = "DELETE FROM DeviceOrderDevices WHERE OrderID=@OrderID";
                        SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@OrderID", model.OrderID) };
                        var truncateResult = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
                    }

                    var sqlConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                    using (var conn = new SqlConnection(sqlConnectionString)) {
                        var sqlBulkCopy = new SqlBulkCopy(conn);
                        sqlBulkCopy.DestinationTableName = destTable;
                        conn.Open();

                        var schema = conn.GetSchema("Columns", new[] { null, null, destTable, null });

                        foreach (DataColumn sourceColumn in dt.Columns) {
                            foreach (DataRow row in schema.Rows) {
                                if (string.Equals(sourceColumn.ColumnName, (string)row["COLUMN_NAME"], StringComparison.OrdinalIgnoreCase)) {
                                    sqlBulkCopy.ColumnMappings.Add(sourceColumn.ColumnName, (string)row["COLUMN_NAME"]);
                                    break;
                                }
                            }
                        }
                       
                        sqlBulkCopy.WriteToServer(dt);
                        conn.Close();
                    }
                }
            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error uploading devices", "Error uploading devices", true, false);
                ex.ToExceptionless().AddTags("HandsetImport").Submit();
                return Ok(processingResult);
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = "Successful";
            return Ok(processingResult);
        }
    }
}
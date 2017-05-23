using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using System;
using LS.Services;
using LS.ApiBindingModels;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.IO;
using LS.Utilities;
using Ionic.Zip;
using Newtonsoft.Json;
using Exceptionless;
using Exceptionless.Models;
using System.Configuration;
using System.Drawing;
using System.Data.OleDb;
using OfficeOpenXml;
using LS.WebApp.CustomAttributes;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace LS.WebApp.Controllers.api {
    //[Authorize]
    [SingleSessionAuthorize]
    [RoutePrefix("api/importData")]
    public class ImportDataController : BaseApiController
	{

        [Route("uploadTrueUpData")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadTrueUpData() {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };

            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data"))) {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/App_Data"));
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            var data = await Request.Content.ReadAsMultipartAsync(provider);


            DataTable NumList = new DataTable();
            NumList.Columns.Add("Item", typeof(String));

            if (provider.FileData != null && provider.FileData.Count > 0) {
                try {
                    Stream excelStream = null;

                    foreach (MultipartFileData fileData in provider.FileData) {
						excelStream = new MemoryStream(File.ReadAllBytes(fileData.LocalFileName));
						//excelStream = new MemoryStream(File.ReadAllBytes("F:\\Book1.xlsx"));
						
						var excel = new ExcelPackage(excelStream);
                        var dt = Utils.ExcelToDataTable(excel, "Data");

                        foreach (DataRow enrollmentNumber in dt.Rows) {
                            NumList.Rows.Add(enrollmentNumber["EnrollmentNumber"]);
                        }
                    }

                } catch (Exception ex) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error uploading true up data", "Error uploading true up data", true, false);
                    ex.ToExceptionless().AddTags("TrueUp").Submit();
                    return Ok(processingResult);
                }
            } else {
                var model = new TrueUpRequest();

                // Show all the key-value pairs.
                foreach (var key in provider.FormData.AllKeys) {
                    foreach (var val in provider.FormData.GetValues(key)) {
                        Utils.SetObjectProperty(key, val, model);
                    }
                }
                List<string> enrollmentNumbers = model.EnrollmentNumbers.Split(',', '\n', '\r').ToList();
                var filteredEnrollmentNumbers = enrollmentNumbers.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                if (filteredEnrollmentNumbers.Count() > 0) {
                    foreach (string number in filteredEnrollmentNumbers) {
                        NumList.Rows.Add(number);
                    }
                } else {
                    NumList.Rows.Add("");
                }
            }

            var sqlQuery = new SQLQuery();

            var queryString = @"
                INSERT INTO ExternalDataTrueUp (CompanyId,ENROLLMENTNUMBER)
                SELECT @CompanyId, Item FROM @EnrollmentList
            ";

			SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@EnrollmentList", SqlDbType.Structured) {
					TypeName = "ItemList",
					Value = NumList
				},
				new SqlParameter("@CompanyId",LoggedInUser.CompanyId )
		};

            var result = await sqlQuery.ExecuteScalarAsync(CommandType.Text, queryString, parameters);
            if (!result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Failed to insert list of enrollment codes", "Failed to insert list of enrollment codes", true, false);
                ExceptionlessClient.Default.CreateLog("Failed to insert list of enrollment codes", "Error").Submit();
                return Ok(processingResult);
            };

            processingResult.IsSuccessful = true;
            processingResult.Data = "Successfull";
            return Ok(processingResult);
        }

        [Route("uploadDuplicateData")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadDuplicateData() {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };

            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data"))) {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/App_Data"));
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            var data = await Request.Content.ReadAsMultipartAsync(provider);

            try {
                Stream excelStream = null;

                foreach (MultipartFileData fileData in provider.FileData) {
                    excelStream = new MemoryStream(File.ReadAllBytes(fileData.LocalFileName));

                    var excel = new ExcelPackage(excelStream);
                    var dt = Utils.ExcelToDataTable(excel, "90 day rule duplicates");
					dt.Columns.Add("CompanyID", typeof(System.String));
					foreach (DataRow row in dt.Rows) { row["CompanyID"] = LoggedInUser.CompanyId; }
					var destTable = "ExternalDuplicateData";

                    var sqlQuery = new SQLQuery();

                    var strQuery = "TRUNCATE TABLE ExternalDuplicateData";
                    var truncateResult = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery);

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
                        //sqlBulkCopy.ColumnMappings.Add("GEOLOCATION", "GEOLOCATION");  //Only needed if we have column names that don't match or if we need to change types                     
                        sqlBulkCopy.WriteToServer(dt);
                        conn.Close();
                    }

                    strQuery = @"
                        UPDATE O SET O.IsDuplicate=1, O.DateMarkedDuplicate=getdate()
                        FROM Orders (NOLOCK) O 
                            LEFT JOIN ExternalDuplicateData (NOLOCK) EDD ON O.TenantAccountId=EDD.NEW_ENROLLMENT AND O.CompanyID=EDD.CompanyID
                        WHERE 1=1
                            AND EDD.NEW_ENROLLMENT IS NOT NULL
                            AND O.IsDuplicate != 1
                    ";
                    var updateDuplicateOrdersResult = await sqlQuery.ExecuteReaderAsync(CommandType.Text, strQuery);
                    if (!updateDuplicateOrdersResult.IsSuccessful) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = new ProcessingError("Error finding duplicate orders in the order table.", "Error finding duplicate orders in the order table.", true, false);
                        ExceptionlessClient.Default.CreateException(new Exception("Error finding duplicate orders in the order table.")).AddTags("Duplicate Data").Submit();
                        return Ok(processingResult);
                    }

                    //foreach (DataRow row in dupOrderResult.Data.Rows) {
                    //    SqlParameter[] parameters = new SqlParameter[] {new SqlParameter("@OrderID", row["OrderID"].ToString())};

                    //    strQuery = @"UPDATE Orders SET IsDuplicate=1, DateMarkedDuplicate=getdate() WHERE Id=@OrderID";

                    //    var updateOrderResult = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery, parameters);
                    //    if (!updateOrderResult.IsSuccessful) {
                    //        processingResult.IsSuccessful = false;
                    //        processingResult.Error = new ProcessingError("Error updating duplicate order in the order table.", "Error updating duplicate order in the order table.", true, false);
                    //        ExceptionlessClient.Default.CreateException(new Exception("Error updating duplicate order in the order table.")).AddTags("Duplicate Data").Submit();
                    //        return Ok(processingResult);
                    //    }
                    //}
                }

            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error uploading duplicate data", "Error uploading duplicate data", true, false);
                ex.ToExceptionless().AddTags("Duplicate Data").Submit();
                return Ok(processingResult);
            }

            processingResult.IsSuccessful = true;
            processingResult.Data = "Successful";
            return Ok(processingResult);

        }
    }
}


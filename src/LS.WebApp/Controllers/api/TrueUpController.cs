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
using LS.WebApp.CustomAttributes;
using OfficeOpenXml;
using LS.WebApp.CustomAttributes;
namespace LS.WebApp.Controllers.api {

    [SingleSessionAuthorize]
    [RoutePrefix("api/trueUp")]
    public class TrueUpController : BaseApiController {

        [Route("uploadData")]
        public async Task<IHttpActionResult> UploadData() {
            var processingResult = new ServiceProcessingResult<string>() { IsSuccessful = true };

            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data"))) {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/App_Data"));

            }


            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            var data = await Request.Content.ReadAsMultipartAsync(provider);

            var fullFilePath = root + "/" + Utils.RandomGuidString() + ".xlsx";

            try {
                Stream excelStream = null;

                foreach (MultipartFileData fileData in provider.FileData) {
                    excelStream = new MemoryStream(File.ReadAllBytes(fileData.LocalFileName));

                    File.Move(fileData.LocalFileName, fullFilePath);

                    var excel = new ExcelPackage(excelStream);
                    var dt = Utils.ExcelToDataTable(excel, "Data");

                    var destTable = "ExternalDataTrueUp";

                    var sqlQuery = new SQLQuery();

                    var strQuery = "TRUNCATE TABLE ExternalDataTrueUp";

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

                    strQuery = "DELETE FROM ExternalDataTrueUp WHERE ENROLLMENTNUMBER='' OR ENROLLMENTNUMBER IS NULL OR CAST(ENROLLMENTNUMBER as bigint)<100000000";
                    var deleteResult = await sqlQuery.ExecuteNonQueryAsync(CommandType.Text, strQuery);

                }

            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error uploading true up data", "Error uploading true up data", true, false);
                ex.ToExceptionless().AddTags("TrueUp").Submit();
                return Ok(processingResult);
            }


            processingResult.IsSuccessful = true;
            processingResult.Data = "Successfull";
            return Ok(processingResult);


        }
    }
}

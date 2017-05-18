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
using System.Web;
using Microsoft.AspNet.Identity;
using System.IO;
using Common.Logging;
using LS.Utilities;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System.Data.SqlClient;
using System.Data;
using ApiBindingModels;
using Exceptionless;
using LS.ApiBindingModels;
using System.Text;
using Exceptionless.Models;
namespace LS.Services {
    public class SchedTasksDataService {
        private static string companyID = "65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c";//Arrow
        public async Task<ServiceProcessingResult<string>> DetailReport() {

            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };

            var sqlQuery = "";
            var sqlSelect = "";
            var sqlFrom = "";
            var sqlWhere = "";
            var sqlDateType = "";
            var sqlGroup = "";
            var sqlOrderBy = "";
            int numHours = -72;
            var UtilitiesDataService = new UtilityDataService();
            var utilityResult = await UtilitiesDataService.GetServerVars();
            if (utilityResult.IsSuccessful) {
                if (utilityResult.Data.IsDev) {
                    numHours = -300;
                }
            }

            sqlDateType = @" AND O.DateCreated <= DateAdd(hour, @NumHours, getdate()) ";
            sqlOrderBy = @" ORDER BY O.DateCreated DESC";
            var parameters = new SqlParameter[] {
                new SqlParameter("@NumHours", numHours)
            };

            sqlSelect = @"
                    SELECT O.Id,O.OrderCode, O.DateCreated, O.DateExternalAccountCreated AS AccountDate, COALESCE(UT.Level1Name, 'Unknown') AS Level1Name, UT.ExternalDisplayName AS AgentID, UT.Name AS TeamName, U.FirstName + ' ' + U.LastName AS EmployeeName, U.ExternalUserID AS PromoCode, O.ServiceAddressState, S.StatusCode, S.Name, O.DeviceIdentifier, COALESCE(FC.FirstName + ' ' + FC.LastName, O.ActivationUserID) AS FulfillmentName, O.DeviceIdentifier, O.ActivationDate, FC.ExternalUserID AS FulfillmentPromoCode
                ";
            sqlFrom = @"
                    FROM Orders O (NOLOCK)
                        LEFT JOIN AspNetUsers U (NOLOCK) ON O.UserID=U.Id
                        LEFT JOIN OrderStatuses S (NOLOCK) ON COALESCE(O.StatusID, 0)=S.StatusCode
                        LEFT JOIN v_UserTeams UT (NOLOCK) ON O.SalesTeamID=UT.Id AND UT.UserID=(SELECT Id FROM dbo.AspNetUsers WHERE(UserName ='SA'))
                        LEFT JOIN AspNetUsers FC (NOLOCK) ON O.ActivationUserID=FC.Id
                ";
            sqlWhere = @"
                    WHERE 1=1 
                        AND ((IsExported IS NULL) OR(IsExported = 0))
                        AND O.IsDeleted=0
                        AND UT.Id IS NOT NULL";
            sqlQuery = sqlSelect + sqlFrom + sqlWhere + sqlDateType + sqlGroup + sqlOrderBy;
            var sqlQueryService = new SQLQuery();
            var queryResults = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (queryResults.IsSuccessful) {
                if (queryResults.Data.Rows.Count < 1) {
                    processingResult.IsSuccessful = true;
                    processingResult.Error = new ProcessingError("No Records were returned. ", "No Records were returned.", false, false);

                    return processingResult;
                }
                DataTable OrderIDs = new DataTable();
                DataTable dt = new DataTable();
                dt = queryResults.Data;

                OrderIDs.Columns.Add("Item", typeof(String));
                foreach (DataRow dr in dt.Rows) {
                    OrderIDs.Rows.Add(dr[0].ToString()); //Add OrderID to ItemList table for later updating
                }

                var csvString = Utils.DataTableToCSV(dt, true);
                var exportResult = this.ExportDataFiles(csvString, "OrderDetail_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.TimeOfDay.ToString("hhmm") + ".csv");
                if (!exportResult.Result.IsSuccessful) {
                    processingResult.IsSuccessful = false;
                    processingResult.Error = new ProcessingError("Error saving to storage. ", "Error saving to storage.", false, false);
                    ExceptionlessClient.Default.CreateLog("ScheduledTasksDataService", "Error saving file to storage. ", "Error").AddTags("Controller Error")
                      .AddTags("ExternalDataCtrl")
                      .Submit();
                    processingResult.IsSuccessful = false;
                    return processingResult;
                }

                //every thing is good to here,now mark as exported

                var updateResult = await this.SetRecordExported(OrderIDs);
                if (!updateResult.IsSuccessful) {
                    //email 305 to say update failed
                    var emailHelper = new Utilities.EmailHelper();
                    string body = "Unable to set order ids to exported in DB.";
                    //string body = "The following Order ID's were exported but were not updated to ISEXPORTED=TRUE in database.<br/>";
                    //foreach (DataRow row in OrderIDs.Rows) {
                    //    body = body + row[0].ToString() + "<br/>";
                    //}
                    await emailHelper.SendEmail("Update to isExported Failed", "jarrett@305spin.com,kevin@305spin.com", "", body);
                }

                processingResult.Data = "Success";
                processingResult.IsSuccessful = true;
                return processingResult;
            } else {
                processingResult.IsSuccessful = false;
                ExceptionlessClient.Default.CreateLog("ScheduledTasksDataService", "Error getting report search results. | " + queryResults.Error.UserHelp.ToString(), "Error").AddTags("Controller Error")
                .AddTags("ExternalDataCtrl")
                .Submit();
                processingResult.Error = new ProcessingError("Error getting report search results. ", "Error getting report search results.", false, false);
                return processingResult;
            }
        }

        public async Task<ServiceProcessingResult<string>> SetRecordExported(DataTable OrderIds) {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };
            var parameters = new SqlParameter[] {
                new SqlParameter("@OrderIDs", SqlDbType.Structured) {
                    TypeName = "ItemList",
                    Value =OrderIds
                }
            };

            var sqlQuery = @"
                UPDATE Orders SET 
                    IsExported=1,
                    DateExported=GetDate()
                WHERE Id IN (SELECT Item AS Id FROM @OrderIDs)
            ";

            var sqlQueryService = new SQLQuery();
            var queryResult = await sqlQueryService.ExecuteNonQueryAsync(CommandType.Text, sqlQuery, 20, parameters);
            if (!queryResult.IsSuccessful) {
                ExceptionlessClient.Default.CreateLog("ScheduledTasksDataService", "Error Setting records to exported. ", "Error").AddTags("Service Error")
                  .AddTags("SchedTasksDataService")
                  .Submit();
                processingResult.Error = new ProcessingError("Error Setting records to exported.", "Error Setting records to exported.", true, false);
                processingResult.IsSuccessful = false;
                return processingResult;
            }
            processingResult.IsSuccessful = true;
            processingResult.Data = "Update succeded.";

            return processingResult;
        }

        public async Task<ServiceProcessingResult<string>> ExportDataFiles(string csvFile, string fileName) {
            var processingResult = new ServiceProcessingResult<string> { IsSuccessful = true };
            var parameters = new SqlParameter[] { new SqlParameter("@CompanyId", companyID), new SqlParameter("@Type", "DataExport") };
            var sqlQuery = "SELECT AccessKey, Path, SecretKey,MaxImageSize FROM ExternalStorageCredentials WHERE CompanyId=@CompanyId AND Type=@Type AND IsDeleted=0";
            var sqlQueryService = new SQLQuery();
            var externalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!externalStorageCredentialsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred looking up external storage credentials.", "An error occurred looking up external storage credentials.", false);
                return processingResult;
            }

            var storageCreds = new ExternalStorageCredentials();
            foreach (DataRow row in externalStorageCredentialsResult.Data.Rows) {
                storageCreds.AccessKey = row["AccessKey"].ToString();
                storageCreds.Path = row["Path"].ToString();
                storageCreds.SecretKey = row["SecretKey"].ToString();
                storageCreds.MaxImageSize = (int)row["MaxImageSize"];
            }
            var externalStorageService = new ExternalStorageService(storageCreds);
            byte[] vFile = Encoding.ASCII.GetBytes(csvFile);
            var saveResult = externalStorageService.SaveFile(vFile, fileName);
            if (!saveResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("An error occurred saving file to external storage.", "An error occurred while trying to save file to external storage.", false);
                return processingResult;
            }
            processingResult.IsSuccessful = saveResult.IsSuccessful;

            return processingResult;
        }
        public async Task<ServiceProcessingResult<SchedTaskReturnModel>> MoveDataFiles() {
            var processingResult = new ServiceProcessingResult<SchedTaskReturnModel> { IsSuccessful = true, Data = new SchedTaskReturnModel() };

            var host = "ftp.tracfone.com";
            var username = "budgetp";
            var password = "Tms@3ptd";
            var localPath = HttpContext.Current.Server.MapPath("~/App_Data/").Trim();
            var remoteDirectory = "Salesforce_Integration/";
            var contentString = "";
            var sendEmail = false;
            int fileCount = 0;

            try {
                //http://stackoverflow.com/questions/23703040/download-files-from-sftp-with-ssh-net-library
                using (var sftp = new SftpClient(host, username, password)) {
                    sftp.Connect();
                    contentString += "<br>SFTP Connected<br>";
                    var files = sftp.ListDirectory(remoteDirectory);
                    contentString += "<br>Looping Files in FTP<br>";

                    fileCount = files.Count();
                    foreach (var file in files.OrderBy(file => file.Name)) { //TODO: We need to put in the Company DataImportFilePrefix loop here...
                        if (file.Name.EndsWith("txt") && file.Name.StartsWith("SLArrow") && file.Name.Contains("CA201")) {
                            var remoteFileName = file.Name;
                            contentString += "<br>Writing temp file " + remoteFileName + "<br>";
                            var localDestinationFile = File.OpenWrite(localPath + file.Name);
                            //Stream file1 = File.OpenRead(remoteDirectory + file.Name);
                            sftp.DownloadFile(remoteDirectory + file.Name, localDestinationFile);
                            localDestinationFile.Close();
                            contentString += "<br>Closed temp file " + remoteFileName + "<br>";
                            sftp.DeleteFile(remoteDirectory + file.Name);
                            contentString += "<br>Deleted FTP file " + remoteFileName + "<br>";
                            sendEmail = true;
                        }
                    }

                    sftp.Disconnect();
                    contentString += "<br>SFTP Disconnected<br>";
                }
            } catch (Exception ex) {
                sendEmail = true;
                contentString += "<br>Failure SFTP<br>";
                ex.ToExceptionless()
                .SetMessage("(SFTP ERROR) Error downloading files from tracfone CA sftp.")
                .MarkAsCritical()
                .Submit();
            }

            SqlParameter[] parameters = { };
            var sqlQuery = "SELECT Id AS CompanyId, DataImportFilePrefix FROM Companies WHERE IsDeleted=0";
            var sqlQueryService = new SQLQuery();
            var CompanyFilePrefixesResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!CompanyFilePrefixesResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = CompanyFilePrefixesResult.Error;

                ExceptionlessClient.Default.SubmitLog("Error doing query to get company prefixes", "Error");
                return processingResult;
            }

            try {
                DirectoryInfo d = new DirectoryInfo(localPath);
                contentString += "<br>Getting Temp files to loop<br>";
                foreach (var file in d.GetFiles("*.txt")) {
                    sendEmail = true;
                    contentString += "<br>Running Query to find company<br>";
                    var myCompanyID = "";
                    foreach (DataRow row in CompanyFilePrefixesResult.Data.Rows) {
                        if (file.Name.StartsWith(row["DataImportFilePrefix"].ToString())) {
                            myCompanyID = row["CompanyId"].ToString();
                        }
                    }

                    if (myCompanyID != "") {
                        contentString += "<br>Getting ExternalStorageCredentials<br>";
                        parameters = new SqlParameter[] { new SqlParameter("@CompanyId", myCompanyID), new SqlParameter("@Type", "DataImport") };
                        sqlQuery = "SELECT AccessKey, Path, SecretKey FROM ExternalStorageCredentials WHERE CompanyId=@CompanyId AND Type=@Type AND IsDeleted=0";
                        var externalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                        if (externalStorageCredentialsResult.IsSuccessful) {
                            var storageCreds = new ExternalStorageCredentials();
                            foreach (DataRow row in externalStorageCredentialsResult.Data.Rows) {
                                storageCreds.AccessKey = row["AccessKey"].ToString();
                                storageCreds.Path = row["Path"].ToString();
                                storageCreds.SecretKey = row["SecretKey"].ToString();
                            }

                            var externalStorageService = new ExternalStorageService(storageCreds);

                            FileStream fs = new FileStream(localPath + file.Name, FileMode.Open, FileAccess.Read);
                            contentString += "<br>Uploading file " + file.Name + " to S3 Storage<br>";
                            var saveFileResult = externalStorageService.Save(new BinaryReader(fs).ReadBytes((int)new FileInfo(localPath + file.Name).Length), file.Name);
                            if (saveFileResult.IsSuccessful) {
                                fs.Close();
                                contentString += "<br>Closed Stream from File to S3<br>";
                                file.Delete();
                                contentString += "<br>Deleted Temp File <br>";
                            } else {
                                ExceptionlessClient.Default.SubmitLog("(AWS ERROR) Error saving file to AWS.", "Error");
                                contentString += "<br>(AWS ERROR) Error saving file to AWS.<br>";
                            }

                        } else {
                            ExceptionlessClient.Default.SubmitLog("Error getting storage credentials for CompanyID (" + myCompanyID + ")", "Error");
                            contentString += "<br>Error getting storage credentials for CompanyID (" + myCompanyID + ")<br>";
                        }
                    } else {
                        ExceptionlessClient.Default.SubmitLog("Unable to get CompanyID from prefix", "Error");
                        contentString += "<br>Unable to get CompanyID from prefix<br>";
                    }
                }

            } catch (Exception ex) {
                sendEmail = true;
                contentString += "<br>Error moving local temp files to S3 storage<br>";

                ex.ToExceptionless()
                .SetMessage("(AWS ERROR) Error moving files from local drive to AWS.")
                .MarkAsCritical()
                .Submit();
            }

            processingResult.Data.Content = contentString;
            processingResult.Data.SendEmail = sendEmail;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<SchedTaskReturnModel>> ImportDataFilesCheck() {
            var processingResult = new ServiceProcessingResult<SchedTaskReturnModel> { IsSuccessful = true, Data = new SchedTaskReturnModel() };
            var contentString = "";
            var sendEmail = false;
            contentString += "Looking up ExternalStorageCredentials for each Company || " + DateTime.Now.ToLongTimeString() + "<br>";
            SqlParameter[] parameters = { new SqlParameter("@Type", "DataImport") };
            var sqlQuery = "SELECT E.AccessKey, E.Path, E.SecretKey, E.CompanyId, C.DataImportFilePrefix FROM ExternalStorageCredentials E (NOLOCK) LEFT JOIN Companies C (NOLOCK) ON E.CompanyId=C.Id WHERE E.Type=@Type AND E.IsDeleted=0 AND COALESCE(C.DataImportFilePrefix, '')!=''";
            var sqlQueryService = new SQLQuery();
            var externalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!externalStorageCredentialsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = externalStorageCredentialsResult.Error;
                ExceptionlessClient.Default.SubmitLog("Error getting storage credentials for Companies", "Error");
                return processingResult;
            }
            contentString += "<br>Looping each companies data import in ExternalStorageCredentials<br>";
            foreach (DataRow row in externalStorageCredentialsResult.Data.Rows) {
                var storageCreds = new ExternalStorageCredentials {
                    AccessKey = row["AccessKey"].ToString(),
                    Path = row["Path"].ToString(),
                    SecretKey = row["SecretKey"].ToString()
                };

                var externalStorageService = new ExternalStorageService(storageCreds);
                contentString += "<br>Getting files from S3 Storage <br>";
                var getS3Files = externalStorageService.GetDirectoryList("", "/");

                if (getS3Files.IsSuccessful) {
                    if (getS3Files.Data.Count() > 0) {
                        sendEmail = true;
                        contentString += "<br>Files Count:" + getS3Files.Data.Count() + " <br>";
                    }
                } else {
                    sendEmail = true;
                    ExceptionlessClient.Default.SubmitLog("(AWS ERROR) Error getting file list from AWS.", "Error");
                    contentString += "<br>(AWS ERROR) Error getting file list from AWS.<br>";
                }
            }

            contentString += "<br>Success || " + DateTime.Now.ToLongTimeString() + "UTC <br>";
            processingResult.Data.Content = contentString;
            processingResult.Data.SendEmail = sendEmail;
            return processingResult;
        }

        public async Task<ServiceProcessingResult<SchedTaskReturnModel>> ImportDataFiles() {
            var processingResult = new ServiceProcessingResult<SchedTaskReturnModel> { IsSuccessful = true, Data = new SchedTaskReturnModel() };
            var contentString = "";
            var sendEmail = false;
            contentString += "Looking up ExternalStorageCredentials for each Company || " + DateTime.Now.ToLongTimeString() + "<br>";
            SqlParameter[] parameters = { new SqlParameter("@Type", "DataImport") };
            var sqlQuery = "SELECT E.AccessKey, E.Path, E.SecretKey, E.CompanyId, C.DataImportFilePrefix FROM ExternalStorageCredentials E (NOLOCK) LEFT JOIN Companies C (NOLOCK) ON E.CompanyId=C.Id WHERE E.Type=@Type AND E.IsDeleted=0 AND COALESCE(C.DataImportFilePrefix, '')!=''";
            var sqlQueryService = new SQLQuery();
            var externalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!externalStorageCredentialsResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = externalStorageCredentialsResult.Error;
                ExceptionlessClient.Default.SubmitLog("Error getting storage credentials for Companies", "Error");
                return processingResult;
            }
            contentString += "<br>Looping each companies data import in ExternalStorageCredentials<br>";
            foreach (DataRow row in externalStorageCredentialsResult.Data.Rows) {
                var storageCreds = new ExternalStorageCredentials {
                    AccessKey = row["AccessKey"].ToString(),
                    Path = row["Path"].ToString(),
                    SecretKey = row["SecretKey"].ToString()
                };

                var externalStorageService = new ExternalStorageService(storageCreds);
                contentString += "<br>Getting files from S3 Storage <br>";
                var getS3Files = externalStorageService.GetDirectoryList("", "/");

                if (getS3Files.IsSuccessful) {
                    if (getS3Files.Data.Count() > 0) { sendEmail = true; }
                    // Does line 171 work? we need to order by the oldest file
                    foreach (var s3filename in getS3Files.Data.OrderBy(file => file.Key)) {
                        contentString += "<br><strong>Current File: " + s3filename.Key + " || " + DateTime.Now.ToLongTimeString() + "<br>";
                        string line;
                        string insertTable = "Unknown";
                        if (s3filename.Key.StartsWith(row["DataImportFilePrefix"].ToString() + "Activations") && s3filename.Key.Contains("CA201")) {
                            insertTable = "Activations";
                        }

                        if (insertTable != "Unknown") {
                            contentString += "<br>Get S3 File Stream Object for " + insertTable + "<br>";
                            var s3Stream = externalStorageService.Get(s3filename.Key);
                            StreamReader file = new StreamReader(s3Stream.Data);
                            int count = 0;
                            bool anyInserted = false;
                            while ((line = file.ReadLine()) != null) {
                                count += 1;
                                try {
                                    if (count != 1) { //Skip the header row
                                        if (insertTable == "Activations") {
                                            var lineColumns = new ActivationsRow(line);
                                            var randomGuid = Guid.NewGuid().ToString();
                                            parameters = new SqlParameter[] {
                                                new SqlParameter("@Id", randomGuid),
                                                new SqlParameter("@PromoCode", lineColumns.PromoCode),
                                                new SqlParameter("@CompanyId", row["CompanyId"].ToString()),
                                                new SqlParameter("@ESN", lineColumns.ESN),
                                                new SqlParameter("@EnrollmentNumber", lineColumns.EnrollmentNumber),
                                                new SqlParameter("@ActivationDateTime", lineColumns.ActivationDateTime),
                                                new SqlParameter("@DeviceType", lineColumns.DeviceType ?? (object)DBNull.Value),
                                                new SqlParameter("@ImportFilename", s3filename.Key),
                                                new SqlParameter("@DateCreated", DateTime.Now),
                                                new SqlParameter("@DateModified", DateTime.Now),
                                                new SqlParameter("@IsDeleted", false)
                                            };
                                            sqlQuery = @"
                                                INSERT INTO Activations (
                                                    Id, CompanyID, PromoCode, ESN, EnrollmentNumber, ActivationDate, DeviceType,
                                                    ImportFilename, DateCreated, DateModified, IsDeleted
                                                ) VALUES (
                                                    @Id, @CompanyId, @PromoCode, @ESN, @EnrollmentNumber, @ActivationDateTime, @DeviceType,
                                                    @ImportFilename, @DateCreated, @DateModified, @IsDeleted
                                                )
                                            ";

                                            var insertResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
                                            if (!insertResult.IsSuccessful) {
                                                contentString += "<br><strong>Failed inserting row " + count + "</strong><br>";
                                                ExceptionlessClient.Default.SubmitLog("Failure adding row to data table (" + insertTable + ").  File:" + s3filename.Key + " Row Number:" + count, "Error");
                                            } else {
                                                contentString += "<br>-Inserted row " + count + "<br>";
                                                anyInserted = true;
                                            }
                                        }
                                    }
                                } catch (Exception ex) {
                                    contentString += "<br><strong>Failed inserting block </strong><br>";

                                    ex.ToExceptionless()
                                    .SetMessage("Failure adding row to data table(" + insertTable + ").File:" + s3filename.Key + " Row Number: " + count)
                                    .MarkAsCritical()
                                    .Submit();
                                }
                            }

                            file.Close();
                            contentString += "<br>Closed File<br>";

                            if (anyInserted || count <= 1) { //We inserted at least one row from the file, or the file was empty, or the file only had the header row
                                externalStorageService.RenameFile(s3filename.Key, "imported/" + s3filename.Key);
                                contentString += "<br>Moved file to imported folder<br>";
                            } else {
                                contentString += "<br>Unable to import any rows from file.  Leaving file in storage.<br>";
                            }

                        } else {
                            ExceptionlessClient.Default.SubmitLog("Unable to determine destination table from filename", "Error");
                            contentString += "<br>Unable to determine destination table from filename<br>";
                        }
                    }
                } else {
                    sendEmail = true;
                    ExceptionlessClient.Default.SubmitLog("(AWS ERROR) Error getting file list from AWS.", "Error");
                    contentString += "<br>(AWS ERROR) Error getting file list from AWS.<br>";
                }
            }

            contentString += "<br>Success || " + DateTime.Now.ToLongTimeString() + "<br>";
            processingResult.Data.Content = contentString;
            processingResult.Data.SendEmail = sendEmail;
            return processingResult;
        }
        public async Task<ServiceProcessingResult<SchedTaskReturnModel>> ProcessDataCheck() {
            var processingResult = new ServiceProcessingResult<SchedTaskReturnModel> { IsSuccessful = true, Data = new SchedTaskReturnModel() };
            var sendEmail = false;
            var sqlQueryService = new SQLQuery();

            var processActivationsCheckResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, "SELECT COUNT(*) AS NumRows FROM Activations WHERE DateProcessed IS NULL", 60);
            if (!processActivationsCheckResult.IsSuccessful) {
                sendEmail = true;
                processingResult.IsSuccessful = false;
                processingResult.Error = processActivationsCheckResult.Error;
                return processingResult;
            } else {
                var processActivationsRows = Convert.ToInt32(processActivationsCheckResult.Data.Rows[0]["NumRows"].ToString());
                processingResult.Data.Content += "Activation Rows: " + processActivationsRows.ToString() + "<br>";
                if (processActivationsRows > 0) {
                    sendEmail = true;
                }
            }

            processingResult.Data.SendEmail = sendEmail;

            return processingResult;
        }
        public async Task<ServiceProcessingResult<SchedTaskReturnModel>> ProcessData() {
            var processingResult = new ServiceProcessingResult<SchedTaskReturnModel> { IsSuccessful = true, Data = new SchedTaskReturnModel() };
            var sendEmail = false;
            processingResult.Data.Content = "Running SPs<br>";

            var sqlQueryService = new SQLQuery();

            var processActivationsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.StoredProcedure, "usp_processActivations", 60);
            if (!processActivationsResult.IsSuccessful) {
                sendEmail = true;
                processingResult.IsSuccessful = false;
                processingResult.Error = processActivationsResult.Error;
                return processingResult;
            } else {
                processingResult.Data.Content += "Finished Activations<br>";
                processingResult.Data.Content = "Success";
            }

            var handsetSqlText = @"
				INSERT INTO DeviceOrderActivations(ESN)
				SELECT DISTINCT E.ESN
				FROM [EXTTBL_LifelineSalesActivations] (NOLOCK) E
					LEFT JOIN DeviceOrderActivations (NOLOCK) A ON E.ESN=A.ESN
				WHERE A.ESN IS NULL

				INSERT INTO DeviceOrderActivations(ESN)
				SELECT DISTINCT E.ESN
				FROM [Activations] (NOLOCK) E
					LEFT JOIN DeviceOrderActivations (NOLOCK) A ON E.ESN=A.ESN
				WHERE A.ESN IS NULL
            ";

            var processHandsetActivationsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, handsetSqlText, 120);
            if (!processHandsetActivationsResult.IsSuccessful) {
                sendEmail = true;
                processingResult.IsSuccessful = false;
                processingResult.Error = processActivationsResult.Error;
                return processingResult;
            } else {
                processingResult.Data.Content += "Finished Handset Activations<br>";
                processingResult.Data.Content = "Success";
            }

            processingResult.Data.SendEmail = sendEmail;

            return processingResult;
        }
        public async Task<ServiceProcessingResult<SchedTaskReturnModel>> TempFileCleanup() {
            var processingResult = new ServiceProcessingResult<SchedTaskReturnModel> { IsSuccessful = true, Data = new SchedTaskReturnModel() };
            var contentString = "";
            var sendEmail = false;

            SqlParameter[] parameters = { };
            var sqlQuery = "SELECT Id FROM Companies WHERE IsDeleted=0";
            var sqlQueryService = new SQLQuery();
            var companyFilePrefixesResult = await sqlQueryService.ExecuteReaderAsync<SchedTaskCompanyBindingModel>(CommandType.Text, sqlQuery, parameters);
            if (!companyFilePrefixesResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = companyFilePrefixesResult.Error;
                ExceptionlessClient.Default.SubmitLog("Error doing query to get company prefixes", "Error");
                return processingResult;
            }

            var companyList = (List<SchedTaskCompanyBindingModel>)companyFilePrefixesResult.Data;

            var myCompanyID = companyList[0].Id;

            //Get old signatures
            parameters = new SqlParameter[] { new SqlParameter("@CompanyId", myCompanyID), new SqlParameter("@Type", "Signatures") };
            sqlQuery = "SELECT AccessKey, Path, SecretKey FROM ExternalStorageCredentials WHERE CompanyId=@CompanyId AND Type=@Type AND IsDeleted=0";
            var externalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (externalStorageCredentialsResult.IsSuccessful) {
                var storageCreds = new ExternalStorageCredentials();
                foreach (DataRow row in externalStorageCredentialsResult.Data.Rows) {
                    storageCreds.AccessKey = row["AccessKey"].ToString();
                    storageCreds.Path = row["Path"].ToString();
                    storageCreds.SecretKey = row["SecretKey"].ToString();
                }

                var externalStorageService = new ExternalStorageService(storageCreds);
                var getS3Files = externalStorageService.GetDirectoryList("", "/");

                if (getS3Files.IsSuccessful) {
                    if (getS3Files.Data.Count() > 0) { sendEmail = true; }

                    var listToDelete = new List<string>();
                    foreach (var s3filename in getS3Files.Data) {
                        if (s3filename.LastModified < DateTime.Now.AddHours(-24)) {
                            listToDelete.Add(s3filename.Key);
                        }
                    }

                    //Delete files script here

                    contentString += "<br>Signature files cleanup success || " + DateTime.Now.ToLongTimeString() + "UTC <br>";
                }
            }

            //Get old proof files
            parameters = new SqlParameter[] { new SqlParameter("@CompanyId", myCompanyID), new SqlParameter("@Type", "Proof") };
            sqlQuery = "SELECT AccessKey, Path, SecretKey FROM ExternalStorageCredentials WHERE CompanyId=@CompanyId AND Type=@Type AND IsDeleted=0";
            externalStorageCredentialsResult = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (externalStorageCredentialsResult.IsSuccessful) {
                var storageCreds = new ExternalStorageCredentials();
                foreach (DataRow row in externalStorageCredentialsResult.Data.Rows) {
                    storageCreds.AccessKey = row["AccessKey"].ToString();
                    storageCreds.Path = row["Path"].ToString();
                    storageCreds.SecretKey = row["SecretKey"].ToString();
                }

                var externalStorageService = new ExternalStorageService(storageCreds);
                var getS3Files = externalStorageService.GetDirectoryList("", "/");

                if (getS3Files.IsSuccessful) {
                    if (getS3Files.Data.Count() > 0) { sendEmail = true; }

                    var listToDelete = new List<string>();
                    foreach (var s3filename in getS3Files.Data) {
                        if (s3filename.LastModified < DateTime.Now.AddHours(-24)) {
                            listToDelete.Add(s3filename.Key);
                        }
                    }

                    //Delete files script here

                    contentString += "<br>Proof files cleanup Success || " + DateTime.Now.ToLongTimeString() + "UTC <br>";
                }
            }

            processingResult.Data.Content = contentString;
            processingResult.Data.SendEmail = sendEmail;
            return processingResult;
        }
        public async Task<ServiceProcessingResult<SchedTaskReturnModel>> ClearOrders() {
            var processingResult = new ServiceProcessingResult<SchedTaskReturnModel> { IsSuccessful = true, Data = new SchedTaskReturnModel() };
            var contentString = "";
            var sendEmail = false;
            contentString += "Cleared orders and temp orders data table || " + DateTime.Now.ToLongTimeString() + "<br>";
            SqlParameter[] parameters = {
                new SqlParameter("@tableName","Orders"),
                new SqlParameter("@fromDays", "-4"),
                new SqlParameter("@toDays","-1")
            };

            var sqlQueryService = new SQLQuery();
            //timeout in seconds 300=5 minutes
            var clearResult = await sqlQueryService.ExecuteNonQueryAsync(CommandType.StoredProcedure, "usp_Clear_Order_Data", 300, parameters);
            if (!clearResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = clearResult.Error;
                ExceptionlessClient.Default.SubmitLog("Error clearing data", "Error");
                return processingResult;
            }

            SqlParameter[] parameters1 = {
                new SqlParameter("@tableName","TempOrders"),
                new SqlParameter("@fromDays", "-4"),
                new SqlParameter("@toDays","-1")
            };

            var sqlQueryService1 = new SQLQuery();

            var clearResult1 = await sqlQueryService.ExecuteNonQueryAsync(CommandType.StoredProcedure, "usp_Clear_Order_Data", 300, parameters1);
            if (!clearResult1.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = clearResult.Error;
                ExceptionlessClient.Default.SubmitLog("Error clearing data", "Error");
                return processingResult;
            }

            contentString += "<br>Orders and Temp Orders data cleared.<br>";
            sendEmail = true;

            contentString += "<br>Success || " + DateTime.Now.ToLongTimeString() + "UTC <br>";
            processingResult.Data.Content = contentString;
            processingResult.Data.SendEmail = sendEmail;
            return processingResult;
        }
    }
}

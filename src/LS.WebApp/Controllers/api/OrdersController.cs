using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LS.Core;
using System;
using LS.Domain;
using LS.Services;
using LS.ApiBindingModels;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using Exceptionless;
using Exceptionless.Models;
using LS.WebApp.CustomAttributes;


namespace LS.WebApp.Controllers.api
{

    [SingleSessionAuthorize]

    [RoutePrefix("api/orders")]
    public class OrdersController : BaseApiController
    {
        [HttpPost]
        [Route("getOrdersForFulfillment")]
        public async Task<IHttpActionResult> GetOrders(OrderSearhItems PostData)
        {
            var processingResult = new ServiceProcessingResult<List<OrderStatusBindingModel>> { IsSuccessful = true, Data = new List<OrderStatusBindingModel>() };

            var convertedDateStart = DateTime.Parse(PostData.StartDate, null, DateTimeStyles.RoundtripKind);
            var convertedDateEnd = DateTime.Parse(PostData.EndDate, null, DateTimeStyles.RoundtripKind);
            var filterString = "";

            if (!String.IsNullOrEmpty(PostData.Filter)) {
                filterString += "AND O.FirstName LIKE @Filter AND O.LastName LIKE @Filter AND O.FirstName + ' ' + O.LastName LIKE @Filter";
            }

            if (LoggedInUser.Role.Rank > 4) {
                filterString += " AND O.UserId=@UserID";
            }

            var cmdText = @"
                SELECT 
	                O.Id, 
	                O.FirstName,
	                O.LastName,
	                U.FirstName AS EmployeeFirstName,
	                U.LastName AS EmployeeLastName,
	                S.Name AS SalesTeamName,
	                S.ExternalDisplayName AS SalesTeamExternalDisplayName,
	                O.DeviceIdentifier,
                    O.DeviceId,
	                O.SimIdentifier,
	                O.FulfillmentType,
	                O.TenantAccountID,
                    COALESCE(O.ActivationDate, null) AS ActivationDate,
	                COALESCE(OS.StatusCode,0) AS StatusCode,
	                COALESCE(OS.Name,'Pending RTR') AS Status,
                    O.RTR_Name,
                    O.RTR_Date,
                    O.RTR_RejectCode,
                    O.RTR_Notes,
	                O.DateCreated,
					CASE WHEN LOF.OrderId IS NOT NULL THEN 1 ELSE 0 END AS ActivationAttempted
                FROM Orders O
	                LEFT JOIN SalesTeams S ON O.SalesTeamId=S.Id
	                LEFT JOIN AspNetUsers U ON U.Id=O.userId
                    LEFT JOIN LogOrderFulfillment LOF ON O.Id=LOF.OrderID
	                LEFT JOIN OrderStatuses OS ON O.StatusID=OS.StatusCode
                WHERE 1=1
	                AND O.IsDeleted = 0
                    AND O.DateCreated >= @DateStart 
                    AND O.DateCreated < DATEADD(day, 1, @DateEnd)
                    AND OS.StatusCode = 100
	                AND O.SalesTeamID IN (SELECT Item FROM @Locations) " + filterString + @"
					 GROUP BY 
                        O.Id,
                        O.FirstName,
	                    O.LastName,
	                    U.FirstName,
	                    U.LastName,
	                    S.Name,
	                    S.ExternalDisplayName,
	                    O.DeviceIdentifier,
                        O.DeviceId,
	                    O.SimIdentifier,
	                    O.FulfillmentType,
	                    O.TenantAccountID,
                        O.ActivationDate,
	                    OS.StatusCode,
	                    OS.Name,
                        O.RTR_Name,
                        O.RTR_Date,
                        O.RTR_RejectCode,
                        O.RTR_Notes,
	                    O.DateCreated,
					    LOF.OrderId
                ";

            DataTable LocList = new DataTable();
            LocList.Columns.Add("Item", typeof(String));
            if (PostData.SalesTeamList.Count() > 0) {
                foreach (string Id in PostData.SalesTeamList) {
                    LocList.Rows.Add(Id);
                }
            } else {
                LocList.Rows.Add("");
            }

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@DateStart", convertedDateStart),
                new SqlParameter("@DateEnd", convertedDateEnd),
                new SqlParameter("@Filter", "%"+PostData.Filter+"%"),
                new SqlParameter("@UserID", LoggedInUser.Id),

                new SqlParameter("@Locations", SqlDbType.Structured) {
                    TypeName = "ItemList",
                    Value = LocList
                },
            };

            var sqlQueryHelper = new SQLQuery();
            var getOrdersForFulfillment = await sqlQueryHelper.ExecuteReaderAsync<OrderStatusBindingModel>(CommandType.Text, cmdText, parameters);
            if (!getOrdersForFulfillment.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.CANNOT_GET_ORDERS;
                return Ok(processingResult);
            }
            var orderData = (List<OrderStatusBindingModel>)getOrdersForFulfillment.Data;
            if (orderData != null && orderData.Count > 0) {
                foreach (var order in orderData) {
                    order.CustFullName = order.FirstName + " " + order.LastName;
                    order.EmployeeFullName = order.EmployeeFirstName + " " + order.EmployeeLastName;
                    order.Location = order.SalesTeamExternalDisplayName + "/" + order.SalesTeamName;
                }

            }
            processingResult.Data = orderData;

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("checkLogOrderFulfillment")]
        public async Task<IHttpActionResult> CheckLogOrderFulfillment(string oid)
        {
            var processingResult = new ServiceProcessingResult<bool> { IsSuccessful = true, Data = false };

            var sqlQuery = "SELECT * FROM LogOrderFulfillment WHERE OrderID=@OrderID";
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@OrderID", oid),
            };

            var query = new SQLQuery();
            var logOrderFulfillmentResult = await query.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!logOrderFulfillmentResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting log order fulfillment.", "Error getting log order fulfillment.", false, false);
                return Ok(processingResult);
            }

            if (logOrderFulfillmentResult.Data.Rows.Count > 0) {
                processingResult.Data = true;
            }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("insertLogOrderFulfillment")]
        public async Task<IHttpActionResult> insertLogOrderFulfillment(string oid)
        {
            var processingResult = new ServiceProcessingResult<bool> { IsSuccessful = true, Data = false };

            var sqlQuery = "INSERT INTO LogOrderFulfillment (OrderID, UserID, DateInitiated) VALUES (@OrderID, @LoggedInUserID, getdate())";
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@OrderID", oid),
                new SqlParameter("@LoggedInUserID", LoggedInUser.Id),
            };

            var query = new SQLQuery();
            var insertLogOrderFulfillmentResult = await query.ExecuteNonQueryAsync(CommandType.Text, sqlQuery, parameters);
            if (!insertLogOrderFulfillmentResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error inserting log order fulfillment.", "Error inserting log order fulfillment.", false, false);
                return Ok(processingResult);
            }

            return Ok(processingResult);
        }

        [HttpPost]
        [Route("getOrdersForStatus")]
        public async Task<IHttpActionResult> GetOrdersForStatus(OrderSearhItems PostData)
        {
            var processingResult = new ServiceProcessingResult<List<OrderStatusBindingModel>> { IsSuccessful = true, Data = new List<OrderStatusBindingModel>() };

            var convertedDateStart = DateTime.Parse(PostData.StartDate, null, DateTimeStyles.RoundtripKind);
            var convertedDateEnd = DateTime.Parse(PostData.EndDate, null, DateTimeStyles.RoundtripKind);
            var filterString = "";

            if (!String.IsNullOrEmpty(PostData.Filter)) {
                filterString = "AND (O.FirstName LIKE @Filter OR O.LastName LIKE @Filter OR O.FirstName + ' ' + O.LastName LIKE @Filter)";
            }

            var cmdText = @"
                SELECT 
	                O.Id, 
	                O.FirstName,
	                O.LastName,
	                U.FirstName AS EmployeeFirstName,
	                U.LastName AS EmployeeLastName,
	                S.Name AS SalesTeamName,
	                S.ExternalDisplayName AS SalesTeamExternalDisplayName,
	                O.DeviceIdentifier,
	                O.SimIdentifier,
	                O.FulfillmentType,
	                O.TenantAccountID,
	                COALESCE(OS.StatusCode,0) AS StatusCode,
	                COALESCE(OS.Name,'Pending RTR') AS Status,
	                O.DateCreated,
                    O.RTR_Name,
                    O.RTR_Date,
                    O.RTR_RejectCode,
                    O.RTR_Notes
                FROM Orders O
	                LEFT JOIN SalesTeams S ON O.SalesTeamId=S.Id
	                LEFT JOIN AspNetUsers U ON U.Id=O.userId
	                LEFT JOIN OrderStatuses OS ON O.StatusID=OS.StatusCode
                WHERE 1=1
	                AND O.IsDeleted = 0
                    AND O.DateCreated >= @DateStart 
                    AND O.DateCreated < DATEADD(day, 1, @DateEnd)
	                AND O.SalesTeamID IN (SELECT Item FROM @Locations) " + filterString;

            DataTable LocList = new DataTable();
            LocList.Columns.Add("Item", typeof(String));
            if (PostData.SalesTeamList.Count() > 0) {
                foreach (string Id in PostData.SalesTeamList) {
                    LocList.Rows.Add(Id);
                }
            } else {
                LocList.Rows.Add("");
            }

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@DateStart", convertedDateStart),
                new SqlParameter("@DateEnd", convertedDateEnd),
                new SqlParameter("@Filter", "%"+PostData.Filter+"%"),
                new SqlParameter("@Locations", SqlDbType.Structured) {
                    TypeName = "ItemList",
                    Value = LocList
                },
            };


            var sqlQueryHelper = new SQLQuery();
            var getOrdersForFulfillment = await sqlQueryHelper.ExecuteReaderAsync(CommandType.Text, cmdText, parameters);
            if (!getOrdersForFulfillment.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = ErrorValues.CANNOT_GET_ORDERS;
                return Ok(processingResult);
            }
            try {
                var orderList = new List<OrderStatusBindingModel>();
                foreach (DataRow row in getOrdersForFulfillment.Data.Rows) {
                    DateTime dateValue;
                    var convertedDate = DateTime.TryParse(row["DateCreated"].ToString(), out dateValue);
                    var convertedOrder = new OrderStatusBindingModel {
                        Id = row["Id"].ToString(),
                        FirstName = row["FirstName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        CustFullName = row["FirstName"].ToString() + " " + row["LastName"].ToString(),
                        EmployeeFirstName = row["EmployeeFirstName"].ToString(),
                        EmployeeLastName = row["EmployeeLastName"].ToString(),
                        EmployeeFullName = row["EmployeeFirstName"].ToString() + " " + row["EmployeeLastName"].ToString(),
                        SalesTeamName = row["SalesTeamName"].ToString(),
                        SalesTeamExternalDisplayName = row["SalesTeamExternalDisplayName"].ToString(),
                        Location = row["SalesTeamExternalDisplayName"].ToString() + "/" + row["SalesTeamName"].ToString(),
                        DeviceIdentifier = row["DeviceIdentifier"].ToString(),
                        SimIdentifier = row["SimIdentifier"].ToString(),
                        FulfillmentType = row["FulfillmentType"].ToString(),
                        TenantAccountID = row["TenantAccountID"].ToString(),
                        StatusCode = Convert.ToInt32(row["StatusCode"].ToString()),
                        Status = row["Status"].ToString(),
                        DateCreated = Convert.ToDateTime(row["DateCreated"].ToString()),
                        RTR_Name = row["RTR_Name"].ToString(),
                        RTR_Date = dateValue,
                        RTR_Notes = row["RTR_Notes"].ToString(),
                        RTR_RejectCode = row["RTR_RejectCode"].ToString(),
                    };

                    if (String.IsNullOrEmpty(convertedOrder.TenantAccountID)) {
                        convertedOrder.AccountStatus = "Account not created";
                    }

                    orderList.Add(convertedOrder);
                    processingResult.Data = orderList;
                }
            } catch (Exception ex) {

            }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getOrderStatuses")]
        public async Task<IHttpActionResult> GetOrderStatuses()
        {
            var processingResult = new ServiceProcessingResult<List<OrderStatuses>> { IsSuccessful = true };

            var sqlQuery = "SELECT * FROM OrderStatuses";

            var query = new SQLQuery();
            var orderStatusesResult = await query.ExecuteReaderAsync(CommandType.Text, sqlQuery);
            if (!orderStatusesResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting order statuses.", "Error getting order statuses.", false, false);
                return Ok(processingResult);
            }
            var orderStatusesList = new List<OrderStatuses>();

            foreach (DataRow row in orderStatusesResult.Data.Rows) {
                var convertedOrderStatus = new OrderStatuses {
                    Id = row["Id"].ToString(),
                    Name = row["Name"].ToString(),
                    StatusCode = Convert.ToInt32(row["StatusCode"].ToString()),
                    DateCreated = Convert.ToDateTime(row["DateCreated"].ToString()),
                    DateModified = Convert.ToDateTime(row["DateModified"].ToString()),
                };
                orderStatusesList.Add(convertedOrderStatus);
            }

            processingResult.Data = orderStatusesList;

            return Ok(processingResult);
        }
        [HttpGet]
        [Route("activationDetail")]
        public async Task<IHttpActionResult> ActivationDetails(string IMEI)
        {
            var processingResult = new ServiceProcessingResult<List<ActivationDetailBindingModel>> { IsSuccessful = true };
            var sqlQuery = @"
                SELECT DOA.ESN, A.ESN AS ActivationESN, A.EnrollmentNumber, A.PromoCode, A.ActivationDate, A.DateProcessed, A.DeviceType 
                FROM DeviceOrderActivations DOA (NOLOCK)
                    LEFT JOIN Activations A (NOLOCK) ON DOA.ESN=A.ESN
                WHERE DOA.ESN=@ESN
                    AND COALESCE(A.IsDeleted, 0)=0 
                ORDER BY A.ActivationDate ASC";
            var parameters = new SqlParameter[] { new SqlParameter("@ESN", IMEI) };
            var query = new SQLQuery();
            var Result = await query.ExecuteReaderAsync(CommandType.Text, sqlQuery, parameters);
            if (!Result.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError(Result.Error.UserMessage, Result.Error.UserHelp, true, false);
                return Ok(processingResult);

            }
            var resultList = new List<ActivationDetailBindingModel>();
            foreach (DataRow row in Result.Data.Rows) {
                try {
                    if (row["ActivationESN"].ToString() == "") {
                        resultList.Add(new ActivationDetailBindingModel() {
                            ESN = row["ESN"].ToString(),
                            DeviceType = "Activation detail not found",
                            EnrollmentNumber = null,
                            PromoCode = null,
                            ActivationDate = null,
                            DateProcessed = null
                        });
                    } else {
                        resultList.Add(new ActivationDetailBindingModel() {
                            ESN = row["ESN"].ToString(),
                            DeviceType = row["DeviceType"].ToString(),
                            EnrollmentNumber = row["EnrollmentNumber"].ToString(),
                            PromoCode = row["PromoCode"].ToString(),
                            ActivationDate = row.Field<DateTime?>("ActivationDate"),
                            DateProcessed = row.Field<DateTime?>("DateProcessed")
                        });
                    }
                } catch (Exception ex) {

                }
            }
            processingResult.Data = resultList;
            return Ok(processingResult);
        }

        [HttpPost]
        [Route("reportSearch")]
        public async Task<IHttpActionResult> ReportSearch(ReportSearchRequestBindingModel model)
        {
            var processingResult = new ServiceProcessingResult<ReportSearchResponseBindingModel> { IsSuccessful = true };
            var a = DateTime.Now.TimeOfDay.ToString();
            var sqlQuery = "";
            var sqlSelect = "";
            var sqlFrom = "";
            var sqlWhere = "";
            var sqlDateType = "";
            var sqlGroup = "";
            var sqlOrderBy = "";

            if (model.DateFilterType == "AccountDate") {
                sqlDateType = @" AND O.DateExternalAccountCreated >= @StartDate AND O.DateExternalAccountCreated < DATEADD(day, 1, @EndDate)";
                sqlOrderBy = @" ORDER BY O.DateExternalAccountCreated DESC";
            } else {
                sqlDateType = @" AND O.DateCreated >= @StartDate AND O.DateCreated < DATEADD(day, 1, @EndDate)";
                sqlOrderBy = @" ORDER BY O.DateCreated DESC";
            }

            var parameters = new SqlParameter[0];

            if (model.Report == "Commissions") {
                DataTable LocList = new DataTable();
                LocList.Columns.Add("Item", typeof(String));
                if (model.Locations.Count() > 0) {
                    foreach (string Id in model.Locations) {
                        LocList.Rows.Add(Id);
                    }
                } else {
                    LocList.Rows.Add("");
                }
                parameters = new SqlParameter[] {
                    new SqlParameter("@LoggedInUserID", LoggedInUserId),
                    new SqlParameter("@Locations", SqlDbType.Structured) {
                        TypeName = "ItemList",
                        Value = LocList
                    },
                    new SqlParameter("@StartDate", DateTime.Parse(model.StartDate, null, DateTimeStyles.RoundtripKind)),
                    new SqlParameter("@EndDate", DateTime.Parse(model.EndDate, null, DateTimeStyles.RoundtripKind))
                };
                sqlSelect = @"
                    SELECT O.OrderCode, O.DateCreated, T.ExternalDisplayName AS SalesTeam, U.FirstName + ' ' + U.LastName AS OrderRep, CU.FirstName + ' ' + CU.LastName AS RecipientUser, CT.ExternalDisplayName AS RecipientTeam, C.Amount, COALESCE(S.StatusCode,0), COALESCE(S.Name, 'Pending RTR') AS Status, P.Id AS PaymentID, P.DatePaid        
                ";
                sqlFrom = @"
                     FROM Orders O (NOLOCK)
                        LEFT JOIN AspNetUsers U (NOLOCK) ON O.UserID=U.Id
                        LEFT JOIN OrderStatuses S (NOLOCK) ON COALESCE(O.StatusID, 0)=S.StatusCode
                        LEFT JOIN SalesTeams T (NOLOCK) ON O.SalesTeamID=T.Id
                        LEFT JOIN CommissionLogs C (NOLOCK) ON O.Id=C.OrderID AND C.OrderType='Account'
                        LEFT JOIN Payments P (NOLOCK) ON P.Id=C.PaymentID
                        LEFT JOIN AspNetUsers CU (NOLOCK) ON C.RecipientUserID=CU.Id
                        LEFT JOIN SalesTeams CT (NOLOCK) ON C.SalesTeamID=CT.Id
                        LEFT JOIN v_UserTeams UT (NOLOCK) ON O.SalesTeamID=UT.Id AND UT.UserID=@LoggedInUserID
                ";
                sqlWhere = @"
                    WHERE 1=1 
                        AND C.Amount > 0
                        AND O.IsDeleted=0
                        AND O.SalesTeamID IN (SELECT Item FROM @Locations)
                        AND UT.Id IS NOT NULL
                        AND C.RecipientUserID IS NOT NULL
                ";

            } else if (model.Report == "Detail") {
                DataTable LocList = new DataTable();
                LocList.Columns.Add("Item", typeof(String));
                if (model.Locations.Count() > 0) {
                    foreach (string Id in model.Locations) {
                        LocList.Rows.Add(Id);
                    }
                } else {
                    LocList.Rows.Add("");
                }

                parameters = new SqlParameter[] {
                    new SqlParameter("@LoggedInUserID", LoggedInUserId),
                    new SqlParameter("@Locations", SqlDbType.Structured) {
                        TypeName = "ItemList",
                        Value = LocList
                    },

                 new SqlParameter("@StartDate",DateTime.Parse(model.StartDate,null,DateTimeStyles.RoundtripKind)),
                 new SqlParameter("@EndDate",DateTime.Parse(model.EndDate,null,DateTimeStyles.RoundtripKind)),
                 new SqlParameter("@OrderStatus", model.OrderStatus)
                };


                sqlSelect = @"
                    SELECT O.OrderCode, O.DateCreated, O.DateExternalAccountCreated AS AccountDate, COALESCE(UT.Level1Name, 'Unknown') AS Level1Name, UT.ExternalDisplayName AS AgentID, UT.Name AS TeamName, U.FirstName + ' ' + U.LastName AS EmployeeName, U.ExternalUserID AS PromoCode, O.ServiceAddressState, S.StatusCode, S.Name,  COALESCE(FC.FirstName + ' ' + FC.LastName, O.ActivationUserID) AS FulfillmentName, O.DeviceIdentifier, O.ActivationDate, FC.ExternalUserID AS FulfillmentPromoCode
                ";
                sqlFrom = @"
                    FROM Orders O (NOLOCK)
                        LEFT JOIN AspNetUsers U (NOLOCK) ON O.UserID=U.Id
                        LEFT JOIN OrderStatuses S (NOLOCK) ON COALESCE(O.StatusID, 0)=S.StatusCode
                        LEFT JOIN v_UserTeams UT (NOLOCK) ON O.SalesTeamID=UT.Id AND UT.UserID=@LoggedInUserID
                        LEFT JOIN AspNetUsers FC (NOLOCK) ON O.ActivationUserID=FC.Id
                ";
                sqlWhere = @"
                    WHERE 1=1
                        AND O.IsDeleted=0
                        AND UT.Id IS NOT NULL
                        AND O.SalesTeamID IN (SELECT Item FROM @Locations)
                        AND (@OrderStatus=-200 OR O.StatusID=@OrderStatus)
                ";
            } else if (model.Report == "HeatMap") {
                DataTable LocList = new DataTable();
                LocList.Columns.Add("Item", typeof(String));
                if (model.Locations.Count() > 0) {
                    foreach (string Id in model.Locations) {
                        LocList.Rows.Add(Id);
                    }
                } else {
                    LocList.Rows.Add("");
                }
                parameters = new SqlParameter[] {
                    new SqlParameter("@LoggedInUserID", LoggedInUserId),
                    new SqlParameter("@Locations", SqlDbType.Structured) {
                        TypeName = "ItemList",
                        Value = LocList
                    },
                    new SqlParameter("@StartDate", DateTime.Parse(model.StartDate, null, DateTimeStyles.RoundtripKind)),
                    new SqlParameter("@EndDate", DateTime.Parse(model.EndDate, null, DateTimeStyles.RoundtripKind)),
                    new SqlParameter("@OrderStatus", model.OrderStatus)
                };
                if (model.MapType == "StoreMap") {
                    sqlSelect = "SELECT COUNT(O.Id) AS OrderTotal, MAX(O.OrderCode) AS OrderCode, MAX(COALESCE(U.ExternalUserID, O.UserID)) AS ExternalUserID, COUNT(DISTINCT COALESCE(U.ExternalUserID, O.UserID)) AS CountExtUserIDs, T.ExternalDisplayName AS AgentID, ROUND(AVG(O.LatitudeCoordinate),4) AS Lat, ROUND(AVG(O.LongitudeCoordinate),4) AS Lon";
                    sqlGroup = " GROUP BY T.ExternalDisplayName";
                } else if (model.MapType == "HeatMap") {
                    sqlSelect = "SELECT 1 AS OrderTotal, O.OrderCode, COALESCE(U.ExternalUserID, O.UserID) AS ExternalUserID, 1 AS CountExtUserIDs, T.ExternalDisplayName AS AgentID, ROUND(O.LatitudeCoordinate,4) AS Lat, ROUND(O.LongitudeCoordinate,4) AS Lon";
                } else {
                    sqlSelect = "SELECT COUNT(O.Id) AS OrderTotal, MAX(O.OrderCode) AS OrderCode, MAX(COALESCE(U.ExternalUserID, O.UserID)) AS ExternalUserID, COUNT(DISTINCT COALESCE(U.ExternalUserID, O.UserID)) AS CountExtUserIDs, T.ExternalDisplayName AS AgentID, ROUND(O.LatitudeCoordinate,4) AS Lat, ROUND(O.LongitudeCoordinate,4) AS Lon";
                    sqlGroup = " GROUP BY ROUND(O.LatitudeCoordinate,4), ROUND(O.LongitudeCoordinate,4), T.ExternalDisplayName";
                }

                sqlFrom = @"
                    FROM Orders O (NOLOCK)
                        LEFT JOIN SalesTeams T (NOLOCK) ON O.SalesTeamID=T.Id
                        LEFT JOIN v_UserTeams UT (NOLOCK) ON O.SalesTeamID=UT.Id AND UT.UserID=@LoggedInUserID
                        LEFT JOIN AspNetUsers U (NOLOCK) ON O.UserID=U.Id
                ";

                if (LoggedInUser.Role.IsSuperAdmin() || LoggedInUser.Role.IsAdmin()) {
                    sqlWhere = @"
                        WHERE 1=1
                            AND O.IsDeleted=0
                            AND (
                                O.SalesTeamID IN(SELECT Item FROM @Locations)
                                OR COALESCE(O.SalesTeamID, '') = ''
                            )
                            AND (@OrderStatus=-200 OR O.StatusID=@OrderStatus)
                            AND COALESCE(O.LatitudeCoordinate, 0)!=0 AND COALESCE(O.LongitudeCoordinate, 0)!=0
                    ";
                } else {
                    sqlWhere = @"
                    WHERE 1=1
                        AND O.IsDeleted=0
                        AND O.SalesTeamID IN (SELECT Item FROM @Locations)
                        AND UT.Id IS NOT NULL
                        AND (@OrderStatus=-200 OR O.StatusID=@OrderStatus)
                        AND COALESCE(O.LatitudeCoordinate, 0)!=0 AND COALESCE(O.LongitudeCoordinate, 0)!=0
                ";
                }

                sqlOrderBy = @"
                    ORDER BY T.ExternalDisplayName ASC
                ";

            } else if (model.Report == "Summary") {
                DataTable LocList = new DataTable();
                LocList.Columns.Add("Item", typeof(String));
                if (model.Locations.Count() > 0) {
                    foreach (string Id in model.Locations) {
                        LocList.Rows.Add(Id);
                    }
                } else {
                    LocList.Rows.Add("");
                }
                parameters = new SqlParameter[] {
                    new SqlParameter("@LoggedInUserID", LoggedInUserId),
                    new SqlParameter("@Locations", SqlDbType.Structured) {
                        TypeName = "ItemList",
                        Value = LocList
                    },

                    new SqlParameter("@StartDate", DateTime.Parse(model.StartDate, null, DateTimeStyles.RoundtripKind)),
                    new SqlParameter("@EndDate", DateTime.Parse(model.EndDate, null, DateTimeStyles.RoundtripKind)),
                    new SqlParameter("@ShowUserDetail", model.ShowUserDetail)
                };

                var userSelect = ""; var userGroup = ""; var userJoin = "";
                if (model.ShowUserDetail) {
                    userSelect = ", U.FirstName + ' ' + U.LastName AS RepName ";
                    userJoin = "LEFT JOIN AspNetUsers U (NOLOCK) ON O.UserID=U.Id";
                    userGroup = ", U.FirstName + ' ' + U.LastName";
                }
                sqlSelect = @"
                    SELECT COUNT(1) AS Submitted
                        , SUM(CASE WHEN COALESCE(StatusID, 0) IN (0,1) THEN 1 ELSE 0 END) AS RTRPending
                        , SUM(CASE WHEN COALESCE(StatusID, 0) IN (100) THEN 1 ELSE 0 END) AS RTRApproved
                        , SUM(CASE WHEN COALESCE(StatusID, 0) IN (-100) THEN 1 ELSE 0 END) AS RTRRejected
                        , T.ExternalDisplayName, T.Name AS TeamName " +
                        userSelect;
                sqlFrom = @" FROM Orders O (NOLOCK)
                        LEFT JOIN SalesTeams T (NOLOCK) ON O.SalesTeamID=T.Id
                        LEFT JOIN v_UserTeams UT (NOLOCK) ON O.SalesTeamID=UT.Id AND UT.UserID=@LoggedInUserID " +
                        userJoin;
                sqlWhere = @" WHERE 1=1
                        AND O.IsDeleted=0
                        AND O.SalesTeamID IN (SELECT Item FROM @Locations)
                        AND UT.Id IS NOT NULL
                ";

                sqlGroup = @" GROUP BY T.ExternalDisplayName, T.Name " + userGroup;
                sqlOrderBy = " ORDER BY T.ExternalDisplayName " + userGroup;

            } else if (model.Report == "TrueUp") {
                parameters = new SqlParameter[] {
                    new SqlParameter("@LoggedInUserID", LoggedInUserId),
					new SqlParameter("@CompanyID",LoggedInUser.CompanyId),
					new SqlParameter("@StartDate", DateTime.Parse(model.StartDate, null, DateTimeStyles.RoundtripKind)),
                    new SqlParameter("@EndDate", DateTime.Parse(model.EndDate, null, DateTimeStyles.RoundtripKind)),
                };

				//NOTE: We are not getting orders that are in the TrueUp, but do not have a corresponding order in our DB (because we can't accurately use the date filter.
				//Get all orders where we have an Enrollment Number, but it is not in the TrueUp
				sqlSelect = @"
                    SELECT '' AS ENROLLMENTNUMBER, O.OrderCode, U.ExternalUserID, U.FirstName, U.LastName 
						, O.ServiceAddressState, O.ServiceAddressZip, O.DateCreated
                    FROM Orders (NOLOCK) O
	                    LEFT JOIN ExternalDataTrueUp T (NOLOCK) ON O.OrderCode=T.EnrollmentNumber AND O.CompanyID=T.CompanyID
	                    LEFT JOIN ASPNetUsers U (NOLOCK) ON O.UserID=U.Id
                    WHERE T.EnrollmentNumber IS NULL 
                        AND O.OrderCode IS NOT NULL
						AND O.CompanyId=@CompanyID
                        AND O.IsDeleted = 0
                        AND O.DateCreated >= @StartDate 
                        AND O.DateCreated < DATEADD(day, 1, @EndDate )
				";
				//Get all orders from TrueUp that have a corresponding order in our DB
				sqlSelect += @"
					UNION
                    SELECT T.ENROLLMENTNUMBER, O.OrderCode, U.ExternalUserID, U.FirstName, U.LastName
						, O.ServiceAddressState, O.ServiceAddressZip, O.DateCreated
                ";
                sqlFrom = @" FROM ExternalDataTrueUp T (NOLOCK)
                    LEFT JOIN Orders O (NOLOCK) ON T.EnrollmentNumber=O.OrderCode AND O.CompanyID=T.CompanyID
                    LEFT JOIN ASPNetUsers U (NOLOCK) ON O.UserID=U.Id
                ";
                sqlWhere = @" WHERE T.CompanyId=@CompanyID AND T.EnrollmentNumber>=100000000 AND O.OrderCode IS NOT NULL";
                sqlDateType = @" 
            	    AND O.DateCreated>=@StartDate 
	                AND O.DateCreated<DATEADD(day, 1, @EndDate )
                ";
                sqlGroup = @"";
                sqlOrderBy = " ORDER BY EnrollmentNumber ASC, OrderCode ASC";
            } else if (model.Report == "HandsetSummary") {
                DataTable LocList = new DataTable();
                LocList.Columns.Add("Item", typeof(String));
                if (model.Level1Groups.Count() > 0) {
                    foreach (string Id in model.Level1Groups) {
                        LocList.Rows.Add(Id);
                    }
                } else {
                    LocList.Rows.Add("");
                }

                //check if start date greater than end date.
                DateTime vstartDate;
                DateTime vendDate;
                var parseResult = DateTime.TryParse(model.StartDate, out vstartDate);
                var parseResult1 = DateTime.TryParse(model.EndDate, out vendDate);
                //only 1 true if end date is empty so don't check
                if (parseResult && parseResult1) {
                    if (vendDate < vstartDate) {
                        processingResult.IsSuccessful = false;
                        processingResult.Error = new ProcessingError("Start Date is greater than End Date.", "Start Date is greater than End Date.", true, false);
                        return Ok(processingResult);
                    }
                }
                //end check start date

                bool isValidStartDate = !String.IsNullOrEmpty(model.StartDate);
                bool isValidEndDate = !String.IsNullOrEmpty(model.EndDate);

                var paramList = new List<SqlParameter>();
                if (isValidStartDate) {
                    var startDateParam = new SqlParameter("@StartDate", SqlDbType.Date);
                    startDateParam.Value = DateTime.Parse(model.StartDate, null, DateTimeStyles.RoundtripKind);
                    paramList.Add(startDateParam);
                }
                if (isValidEndDate) {
                    var endDateParam = new SqlParameter("@EndDate", SqlDbType.Date);
                    endDateParam.Value = DateTime.Parse(model.EndDate, null, DateTimeStyles.RoundtripKind);
                    paramList.Add(endDateParam);
                }

                paramList.Add(new SqlParameter("@LoggedInUserID", LoggedInUserId));
                paramList.Add(new SqlParameter("@Level1SalesGroups", SqlDbType.Structured) { TypeName = "ItemList", Value = LocList });
                paramList.Add(new SqlParameter("@PONumber", model.PONumber));

                parameters = paramList.ToArray();

                sqlSelect = @"
                   SELECT O.PONumber, O.ShipDate, O.AgentDueDate, O.ASGDueDate, O.IsReturned, D.NumDevices, D.NumReturned, D.NumActivated, SG.Name AS Level1Name, D.NumDevices - D.NumActivated AS NumNonActivatedHandsets
                ";
                sqlFrom = @"
                    FROM DeviceOrders (NOLOCK) O
                        LEFT JOIN (
                            SELECT D.OrderID, COUNT(IMEI) AS NumDevices, SUM(CASE WHEN D.IsReturned=1 THEN 1 ELSE 0 END) AS NumReturned, SUM(CASE WHEN A.ESN IS NULL THEN 0 ELSE 1 END) AS NumActivated
                            FROM DeviceOrderDevices (NOLOCK) D
                                LEFT JOIN DeviceOrderActivations (NOLOCK) A ON D.IMEI=A.ESN
                            GROUP BY D.OrderID
                        ) D ON O.ID=D.OrderID
                        LEFT JOIN Level1SalesGroup (NOLOCK) SG ON O.Level1SalesGroupID=SG.ID
                ";
                sqlWhere = @" WHERE 1=1 AND SG.Id IN(SELECT Item FROM @Level1SalesGroups) ";
                if (isValidStartDate) { sqlWhere += @" AND O.AgentDueDate >= @StartDate "; }
                if (isValidEndDate) { sqlWhere += @" AND O.AgentDueDate < DATEADD(day, 1, @EndDate) "; }

                if (!string.IsNullOrEmpty(model.PONumber)) { sqlWhere += " AND O.PONumber=@PONumber"; }

                sqlDateType = @"";
                sqlGroup = @"";
                sqlOrderBy = " ORDER BY O.PONumber";
            } else if (model.Report == "HandsetDetail") {
                parameters = new SqlParameter[] {
                    new SqlParameter("@LoggedInUserID", LoggedInUserId),
                    new SqlParameter("@PONumber", model.PONumber),
                };

                sqlSelect = @"
                  SELECT D.IMEI, D.PartNumber, D.IsReturned, CASE WHEN A.ESN IS NULL THEN 0 ELSE 1 END AS IsActivated 
                ";
                sqlFrom = @" 
                    FROM DeviceOrderDevices (NOLOCK) D
                        LEFT JOIN DeviceOrderActivations (NOLOCK) A ON D.IMEI=A.ESN
                        LEFT JOIN DeviceOrders (NOLOCK) DO ON DO.Id=D.OrderID
                ";

                sqlWhere = @" WHERE 1=1 AND DO.PONumber=@PONumber";
                sqlDateType = @"";
                sqlGroup = @"";
                sqlOrderBy = " ORDER BY D.IMEI ASC";
            }

            sqlQuery = sqlSelect + sqlFrom + sqlWhere + sqlDateType + sqlGroup + sqlOrderBy;
            var sqlQueryService = new SQLQuery();
            var queryResults = await sqlQueryService.ExecuteReaderAsync(CommandType.Text, sqlQuery, 20, parameters);
            if (queryResults.IsSuccessful) {
                var addJSON = "";
                if (model.Report == "HeatMap") {
                    try {
                        var Averages = new {
                            Lat = queryResults.Data.Compute("AVG(Lat)", ""),
                            Lon = queryResults.Data.Compute("AVG(Lon)", "")
                        };
                        addJSON = JsonConvert.SerializeObject(Averages);
                    } catch (Exception ex) {

                    }

                }
                string queryResultJSON = JsonConvert.SerializeObject(queryResults.Data);
                processingResult.Data = new ReportSearchResponseBindingModel {
                    AddJSON = addJSON,
                    QueryResult = queryResultJSON

                };
            } else {
                processingResult.IsSuccessful = false;
                //Logger.Fatal("Error getting report search results. | " + queryResults.Error.UserHelp.ToString());
                ExceptionlessClient.Default.CreateLog(typeof(OrdersController).FullName, "Error getting report search results. | " + queryResults.Error.UserHelp.ToString(), "Error").AddTags("Controller Error").Submit();
                processingResult.Error = new ProcessingError("Error getting report search results. ", "Error getting report search results.", false, false);
            }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("editGetOrder")]
        public async Task<IHttpActionResult> EditGetOrder(string orderId)
        {
            var processingResult = new ServiceProcessingResult<Order> { IsSuccessful = true };

            var connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlDataReader rdr = null;
            string sqlstring = "Select *,CONVERT(varchar(50), DecryptByPassPhrase(@SQLPassPhrase, QBSsn)) AS UnencryptedQBSsn,CONVERT(varchar(50), DecryptByPassPhrase(@SQLPassPhrase, QBDateOfBirth)) AS UnencryptedQBDateOfBirth,CONVERT(varchar(50), DecryptByPassPhrase(@SQLPassPhrase, DateOfBirth)) AS UnencryptedDateOfBirth,CONVERT(varchar(50), DecryptByPassPhrase(@SQLPassPhrase, SSN)) AS UnencryptedSsn from dbo.orders where id=@orderId";
            SqlCommand cmd = new SqlCommand(sqlstring, connection);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.Add(new SqlParameter("@SQLPassPhrase", "Test3r123!"));
            cmd.Parameters.Add(new SqlParameter("@orderId", orderId));

            try {
                connection.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read()) {
                    var vOrder = new Order() {
                        Id = rdr["Id"].ToString(),
                        CompanyId = rdr["CompanyId"].ToString(),
                        ParentOrderId = rdr["ParentOrderId"].ToString(),
                        SalesTeamId = rdr["SalesTeamId"].ToString(),
                        UserId = rdr["UserId"].ToString(),
                        HouseholdReceivesLifelineBenefits = rdr.IsDBNull(rdr.GetOrdinal("HouseholdReceivesLifelineBenefits")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HouseholdReceivesLifelineBenefits")),
                        CustomerReceivesLifelineBenefits = rdr.IsDBNull(rdr.GetOrdinal("CustomerReceivesLifelineBenefits")) ? false : rdr.GetBoolean(rdr.GetOrdinal("CustomerReceivesLifelineBenefits")),
                        QBFirstName = rdr["QBFirstName"].ToString(),
                        QBLastName = rdr["QBLastName"].ToString(),
                        UnencryptedQBSsn = rdr["UnencryptedQBSsn"].ToString(),
                        UnencryptedQBDateOfBirth = rdr["UnencryptedQBDateOfBirth"].ToString(),
                        CurrentLifelinePhoneNumber = rdr["CurrentLifelinePhoneNumber"].ToString(),
                        LifelineProgramId = rdr["LifelineProgramId"].ToString(),
                        LPProofTypeId = rdr["LPProofTypeId"].ToString(),
                        LPProofNumber = rdr["LPProofNumber"].ToString(),
                        LPProofImageID = rdr["LPProofImageID"].ToString(),
                        LPProofImageFilename = rdr["LPProofImageFilename"].ToString(),
                        StateProgramId = rdr["StateProgramId"].ToString(),
                        StateProgramNumber = rdr["StateProgramNumber"].ToString(),
                        SecondaryStateProgramId = rdr["SecondaryStateProgramId"].ToString(),
                        SecondaryStateProgramNumber = rdr["SecondaryStateProgramNumber"].ToString(),
                        FirstName = rdr["FirstName"].ToString(),
                        MiddleInitial = rdr["MiddleInitial"].ToString(),
                        LastName = rdr["LastName"].ToString(),
                        UnencryptedSsn = rdr["UnencryptedSsn"].ToString(),
                        UnencryptedDateOfBirth = rdr["UnencryptedDateOfBirth"].ToString(),
                        EmailAddress = rdr["emailAddress"].ToString(),
                        ContactPhoneNumber = rdr["ContactPhoneNumber"].ToString(),
                        IDProofTypeID = rdr["IDProofTypeID"].ToString(),
                        IDProofImageID = rdr["IDProofImageID"].ToString(),
                        IDProofImageFilename = rdr["IDProofImageFilename"].ToString(),
                        ServiceAddressStreet1 = rdr["ServiceAddressStreet1"].ToString(),
                        ServiceAddressStreet2 = rdr["ServiceAddressStreet2"].ToString(),
                        ServiceAddressCity = rdr["ServiceAddressCity"].ToString(),
                        ServiceAddressState = rdr["ServiceAddressState"].ToString(),
                        ServiceAddressZip = rdr["ServiceAddressZip"].ToString(),
                        ServiceAddressIsPermanent = rdr.IsDBNull(rdr.GetOrdinal("ServiceAddressIsPermanent")) ? false : rdr.GetBoolean(rdr.GetOrdinal("ServiceAddressIsPermanent")),
                        ServiceAddressIsRural = rdr.IsDBNull(rdr.GetOrdinal("ServiceAddressIsRural")) ? false : rdr.GetBoolean(rdr.GetOrdinal("ServiceAddressIsRural")),
                        BillingAddressStreet1 = rdr["BillingAddressStreet1"].ToString(),
                        BillingAddressStreet2 = rdr["BillingAddressStreet2"].ToString(),
                        BillingAddressCity = rdr["BillingAddressCity"].ToString(),
                        BillingAddressState = rdr["BillingAddressState"].ToString(),
                        BillingAddressZip = rdr["BillingAddressZip"].ToString(),
                        ShippingAddressStreet1 = rdr["ShippingAddressStreet1"].ToString(),
                        ShippingAddressStreet2 = rdr["ShippingAddressStreet2"].ToString(),
                        ShippingAddressCity = rdr["ShippingAddressCity"].ToString(),
                        ShippingAddressState = rdr["ShippingAddressState"].ToString(),
                        ShippingAddressZip = rdr["ShippingAddressZip"].ToString(),
                        HohSpouse = rdr.IsDBNull(rdr.GetOrdinal("HohSpouse")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohSpouse")),
                        HohAdultsParent = rdr.IsDBNull(rdr.GetOrdinal("HohAdultsParent")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohAdultsParent")),
                        HohAdultsChild = rdr.IsDBNull(rdr.GetOrdinal("HohAdultsChild")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohAdultsChild")),
                        HohAdultsRelative = rdr.IsDBNull(rdr.GetOrdinal("HohAdultsRelative")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohAdultsRelative")),
                        HohAdultsRoommate = rdr.IsDBNull(rdr.GetOrdinal("HohAdultsRoommate")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohAdultsRoommate")),
                        HohAdultsOther = rdr.IsDBNull(rdr.GetOrdinal("HohAdultsOther")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohAdultsOther")),
                        HohAdultsOtherText = rdr["HohAdultsOtherText"].ToString(),
                        HohExpenses = rdr.IsDBNull(rdr.GetOrdinal("HohExpenses")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohExpenses")),
                        HohShareLifeline = rdr.IsDBNull(rdr.GetOrdinal("HohShareLifeline")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohShareLifeline")),
                        HohShareLifelineNames = rdr["HohShareLifelineNames"].ToString(),
                        HohAgreeMultiHouse = rdr.IsDBNull(rdr.GetOrdinal("HohAgreeMultiHouse")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohAgreeMultiHouse")),
                        HohAgreeViolation = rdr.IsDBNull(rdr.GetOrdinal("HohAgreeViolation")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohAgreeViolation")),
                        HohPuertoRicoAgreeViolation = rdr.IsDBNull(rdr.GetOrdinal("HohPuertoRicoAgreeViolation")) ? false : rdr.GetBoolean(rdr.GetOrdinal("HohPuertoRicoAgreeViolation")),
                        SignatureType = rdr["SignatureType"].ToString(),
                        DeviceId = rdr["DeviceId"].ToString(),
                        DeviceIdentifier = rdr["DeviceIdentifier"].ToString(),
                        SimIdentifier = rdr["SimIdentifier"].ToString(),
                        PlanId = rdr["PlanId"].ToString(),
                        LifelineEnrollmentId = rdr["LifelineEnrollmentId"].ToString(),
                        LifelineEnrollmentType = rdr["LifelineEnrollmentType"].ToString(),
                        AIInitials = rdr["AIInitials"].ToString(),
                        AIFrequency = rdr["AIFrequency"].ToString(),
                        AIAvgIncome = rdr.IsDBNull(rdr.GetOrdinal("AIAvgIncome")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("AIAvgIncome")),
                        AINumHousehold = rdr.IsDBNull(rdr.GetOrdinal("AINumHousehold")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("AINumHousehold")),
                        TenantReferenceId = rdr["TenantReferenceId"].ToString(),
                        TenantAccountId = rdr["TenantAccountId"].ToString(),
                        TenantAddressId = rdr.IsDBNull(rdr.GetOrdinal("TenantAddressId")) ? 0 : rdr.GetInt32(rdr.GetOrdinal("TenantAddressId")),
                        PricePlan = rdr.IsDBNull(rdr.GetOrdinal("PricePlan")) ? 0 : rdr.GetDouble(rdr.GetOrdinal("PricePlan")),
                        PriceTotal = rdr.IsDBNull(rdr.GetOrdinal("PriceTotal")) ? 0 : rdr.GetDouble(rdr.GetOrdinal("PriceTotal")),
                        FulfillmentType = rdr["FulfillmentType"].ToString(),
                        DeviceModel = rdr["DeviceModel"].ToString(),
                        Gender = rdr["Gender"].ToString(),
                        Language = rdr["Language"].ToString(),
                        CommunicationPreference = rdr["CommunicationPreference"].ToString(),
                    };

                    processingResult.Data = vOrder;
                }
                processingResult.IsSuccessful = true;
            } catch (Exception ex) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error getting order info for rtr resubmit", "Error getting order info for rtr resubmit", false, false);

                ex.ToExceptionless()
                    .SetMessage("Error retrieving order.")
                    .MarkAsCritical()
                    .Submit();
            } finally { connection.Close(); }

            return Ok(processingResult);
        }

        [HttpGet]
        [Route("getSingleOrder")]
        public async Task<IHttpActionResult> GetSingleOrder(string orderId)
        {
            var processingResult = new ServiceProcessingResult<List<Order>> { IsSuccessful = true };

            var ordersService = new OrderDataService();
            var ordersResult = await ordersService.GetOrderByID(orderId);
            if (!ordersResult.IsSuccessful) {
                processingResult.IsSuccessful = false;
                processingResult.Error = new ProcessingError("Error retrieving order.", "Error retrieving order", true, false);
                return Ok(processingResult);
            }
            processingResult.Data = ordersResult.Data;

            return Ok(processingResult);
        }

        // This updates the order
        public async Task<IHttpActionResult> Put(Order model)
        {
            var processingResult = new ServiceProcessingResult<Order>();
            var dataService = new OrderDataService();

            //var orderData = await dataService.GetOrderByID(model.Id);

            processingResult = await dataService.UpdateAsync(model);


            if (processingResult.IsFatalFailure()) {
                var logMessage = string.Format("A fatal error occurred while updating Level1SalesGroup with Id: {0}", "");
                //Logger.Fatal(logMessage);
                ExceptionlessClient.Default.CreateLog(typeof(OrdersController).FullName, "A fatal error occurred while updating Level1SalesGroup Id", "Error").AddTags("Controller Error").AddObject(model).Submit();
            }

            return Ok(processingResult);

        }
    }
}

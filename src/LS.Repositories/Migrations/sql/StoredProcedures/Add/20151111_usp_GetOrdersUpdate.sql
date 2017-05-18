
CREATE PROCEDURE [dbo].[usp_GetOrders]
	@UserId [nvarchar](128) = 'ThisUserIDWontExist', 
	@CustomerName [nvarchar](500) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT O.*, EU.FirstName AS EmployeeFirstName, EU.LastName AS EmployeeLastName, T.ExternalDisplayName AS SalesTeamExternalDisplayName, T.Name AS SalesTeamName
	FROM Orders (NOLOCK) O
		LEFT JOIN AspNetUsers (NOLOCK) U ON U.Id=@UserId
		LEFT JOIN AspNetUsers (NOLOCK) EU ON O.UserId=EU.Id
		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
		LEFT JOIN SalesTeams (NOLOCK) T ON T.Id=O.SalesTeamId
	WHERE 1=1
		AND O.CompanyId=U.CompanyId
		AND O.IsDeleted = 0
		AND (
			O.SalesTeamId=U.SalesTeamId 
			OR O.SalesTeamId IN (
				SELECT T.Id AS SalesTeamId
				FROM SalesTeams (NOLOCK) T
					LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) G1U ON G1U.ApplicationUser_Id=@UserId
					LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) G2U ON G2U.ApplicationUser_Id=@UserId
					LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) G3U ON G3U.ApplicationUser_Id=@UserId
					LEFT JOIN Level2SalesGroup G2 ON G2.ParentSalesGroupId=G1U.Level1SalesGroup_Id OR G2.Id=G2U.Level2SalesGroup_Id
					LEFT JOIN Level3SalesGroup G3 ON G3.ParentSalesGroupId=G2.Id OR G3.Id=G3U.Level3SalesGroup_Id
				WHERE T.Level3SalesGroupId=G3.Id
			)
			OR R.Rank<=1
		)
		AND (O.FirstName LIKE '%'+@CustomerName+'%' OR O.LastName LIKE '%'+@CustomerName+'%' OR O.FirstName+' '+O.LastName LIKE '%'+@CustomerName+'%' OR @CustomerName IS NULL)
END
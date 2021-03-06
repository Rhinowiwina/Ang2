CREATE PROCEDURE [dbo].[usp_GetUsers]
	@UserId [nvarchar](128) = 'ThisUserIDWontExist', 
	@FilterUserName [nvarchar](500) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT U.Id, U.FirstName, U.LastName, U.IsActive,U.UserName, R.Name, R.Rank
	FROM AspNetUsers (NOLOCK) U
		-- Returned user's info
		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
		
		-- Groups that returned user manages
		LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) UG1 ON UG1.ApplicationUser_Id=U.Id
		LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) UG2 ON UG2.ApplicationUser_Id=U.Id
		LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) UG3 ON UG3.ApplicationUser_Id=U.Id
		
		--Logged in user's rank
		LEFT JOIN AspNetUsers (NOLOCK) LIU ON LIU.Id=@UserId
		LEFT JOIN AspNetUserRoles (NOLOCK) LIUR ON LIUR.UserId=LIU.Id
		LEFT JOIN AspNetRoles (NOLOCK) LIR ON LIR.Id=LIUR.RoleID
		
		--Logged in user's teams
		LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) G1LIU ON G1LIU.ApplicationUser_Id=@UserId
		LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) G2LIU ON G2LIU.ApplicationUser_Id=@UserId
		LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) G3LIU ON G3LIU.ApplicationUser_Id=@UserId
		LEFT JOIN Level2SalesGroup (NOLOCK) AG2 ON AG2.ParentSalesGroupId=G1LIU.Level1SalesGroup_Id OR AG2.Id=G2LIU.Level2SalesGroup_Id
		LEFT JOIN Level3SalesGroup (NOLOCK) AG3 ON AG3.ParentSalesGroupId=AG2.Id OR AG3.Id=G3LIU.Level3SalesGroup_Id
		LEFT JOIN SalesTeams (NOLOCK) T ON T.Level3SalesGroupId=AG3.Id
	WHERE 1=1
		AND U.CompanyId=LIU.CompanyId
		AND (
			LIR.Rank=0 --Logged in user is an admin
			OR UG1.Level1SalesGroup_Id=G1LIU.Level1SalesGroup_Id --User is a manager for one of the Logged in user's Level 1 Groups
			OR UG2.Level2SalesGroup_Id=AG2.Id --User is a manager for one of the Logged in user's Level 2 Groups
			OR UG3.Level3SalesGroup_Id=AG3.Id --User is a manager for one of the Logged in user's Level 3 Groups
			OR U.SalesTeamID=T.ID --User is assigned to one of the Logged in user's teams
		)
		AND LIR.Rank<R.Rank --User is a lower rank than the Logged in user
		AND (
			U.FirstName LIKE '%'+@FilterUserName+'%' OR U.LastName LIKE '%'+@FilterUserName+'%' OR U.FirstName+' '+U.LastName LIKE '%'+@FilterUserName+'%'
			OR U.Username LIKE '%'+@FilterUserName+'%'
			OR U.Email LIKE '%'+@FilterUserName+'%'
			OR @FilterUserName IS NULL
		)
	GROUP BY U.Id, U.FirstName, U.LastName, U.IsActive, U.Username, R.Name, R.Rank
	ORDER BY U.FirstName, U.LastName, U.Username
END



INSERT [dbo].[CompanyTranslations] ([Id], [CompanyID], [LSID], [TranslatedID], [Type], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'420', N'65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', N'3df3e85d-105f-4936-98a7-f9417a87d917', 1, N'ProviderID', CAST(N'1900-01-01 00:00:00.000' AS DateTime), CAST(N'1900-01-01 00:00:00.000' AS DateTime), 0)
INSERT [dbo].[CompanyTranslations] ([Id], [CompanyID], [LSID], [TranslatedID], [Type], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'421', N'65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', N'63be6e98-4ea5-4fb7-81e3-97ea150e1d01', 2, N'ProviderID', CAST(N'1900-01-01 00:00:00.000' AS DateTime), CAST(N'1900-01-01 00:00:00.000' AS DateTime), 0)
INSERT [dbo].[CompanyTranslations] ([Id], [CompanyID], [LSID], [TranslatedID], [Type], [DateCreated], [DateModified], [IsDeleted]) VALUES (N'422', N'65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', N'915c73be-9465-4fe0-bd6f-f8fefaf2d0d2', 3, N'ProviderID', CAST(N'1900-01-01 00:00:00.000' AS DateTime), CAST(N'1900-01-01 00:00:00.000' AS DateTime), 0)

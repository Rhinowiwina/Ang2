
CREATE VIEW [dbo].[v_UserTeams] AS
	SELECT T.*, U.Id AS UserID, U.FirstName, U.LastName
		, TG3.Id AS Level3Id, TG3.Name AS Level3Name, TG3.IsDeleted AS Level3IsDeleted, TG2.Id AS Level2Id, TG2.Name AS Level2Name, TG2.IsDeleted AS Level2IsDeleted, TG1.Id AS Level1Id, TG1.Name AS Level1Name, TG1.IsDeleted AS Level1IsDeleted
	FROM SalesTeams (NOLOCK) T
		LEFT JOIN Level3SalesGroup (NOLOCK) TG3 ON TG3.Id=T.Level3SalesGroupId
		LEFT JOIN Level2SalesGroup (NOLOCK) TG2 ON TG2.Id=TG3.ParentSalesGroupId
		LEFT JOIN Level1SalesGroup (NOLOCK) TG1 ON TG1.Id=TG2.ParentSalesGroupId
		LEFT JOIN 
			(
				SELECT U.CompanyId, U.Id, U.FirstName, U.LastName, R.Rank
				FROM AspNetUsers (NOLOCK) U
					LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
					LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
			) U ON U.CompanyId=T.CompanyId 
		LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) UG1 ON UG1.Level1SalesGroup_Id=TG1.Id AND UG1.ApplicationUser_Id=U.Id
		LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) UG2 ON UG2.Level2SalesGroup_Id=TG2.Id AND UG2.ApplicationUser_Id=U.Id
		LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) UG3 ON UG3.Level3SalesGroup_Id=TG3.Id AND UG3.ApplicationUser_Id=U.Id
		WHERE (U.Rank<=1 OR UG1.Level1SalesGroup_Id IS NOT NULL OR UG2.Level2SalesGroup_Id IS NOT NULL OR UG3.Level3SalesGroup_Id IS NOT NULL)

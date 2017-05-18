
CREATE VIEW [dbo].[v_UserTeams] AS
	SELECT U.Id AS UserID, U.FirstName, U.LastName, T.*
	FROM AspNetUsers (NOLOCK) U
		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
		LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) G1 ON G1.ApplicationUser_Id=U.Id
		LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) G2 ON G2.ApplicationUser_Id=U.Id
		LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) G3 ON G3.ApplicationUser_Id=U.Id
		LEFT JOIN Level2SalesGroup (NOLOCK) AG2 ON AG2.ParentSalesGroupId=G1.Level1SalesGroup_Id OR AG2.Id=G2.Level2SalesGroup_Id
		LEFT JOIN Level3SalesGroup (NOLOCK) AG3 ON AG3.ParentSalesGroupId=AG2.Id OR AG3.Id=G3.Level3SalesGroup_Id
		LEFT JOIN SalesTeams (NOLOCK) T ON (T.Level3SalesGroupId=AG3.Id OR R.Rank=0)
	WHERE 1=1
		AND T.Id IS NOT NULL

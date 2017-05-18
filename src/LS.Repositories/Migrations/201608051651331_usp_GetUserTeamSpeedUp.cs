namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usp_GetUserTeamSpeedUp : DbMigration
    {
        public override void Up() {
            this.Sql(@"

            DROP PROCEDURE[dbo].[usp_GetUsers]
            ");
            this.Sql(@"
                  
                CREATE PROCEDURE[dbo].[usp_GetUsers]
        @UserId[nvarchar](128) = 'ThisUserIDWontExist', 
	                @FilterUserName[nvarchar](500) = NULL,
					@FilterRank[integer] = NULL,
					@FilterUserID[nvarchar](500) = NULL
               AS
                BEGIN
	                -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
                    SET NOCOUNT ON;

					--DECLARE @UserId[nvarchar](128) = '414ad6c4-27ee-46a5-931b-09cd7d32c965';
					--DECLARE @FilterUserName[nvarchar](500) = NULL;
					--DECLARE @FilterRank[integer] = 5;
					--DECLARE @FilterUserID[nvarchar](500) = NULL;
					DECLARE @LIUInfo table(CompanyID varchar(100), UserID varchar(100), Rank integer, Level1ID varchar(100), Level2ID varchar(100), Level3ID varchar(100), TeamID varchar(100));
					INSERT INTO @LIUInfo(CompanyID,UserID,Rank,Level1ID,Level2ID,Level3ID,TeamID)
                    SELECT LIU.CompanyId,LIU.ID AS UserID,LIR.Rank,G1LIU.Level1SalesGroup_Id AS Level1ID,AG2.Id AS Level2ID,AG3.Id AS Level3ID,T.ID AS TeamID
                    FROM AspNetUsers (NOLOCK) LIU
                        LEFT JOIN AspNetUserRoles(NOLOCK) LIUR ON LIUR.UserId=LIU.Id
                       LEFT JOIN AspNetRoles(NOLOCK) LIR ON LIR.Id=LIUR.RoleID
                      LEFT JOIN Level1SalesGroupApplicationUser(NOLOCK) G1LIU ON G1LIU.ApplicationUser_Id=@UserId
                     LEFT JOIN Level2SalesGroupApplicationUser(NOLOCK) G2LIU ON G2LIU.ApplicationUser_Id=@UserId
                    LEFT JOIN Level3SalesGroupApplicationUser(NOLOCK) G3LIU ON G3LIU.ApplicationUser_Id=@UserId
                   LEFT JOIN Level2SalesGroup(NOLOCK) AG2 ON AG2.ParentSalesGroupId=G1LIU.Level1SalesGroup_Id OR AG2.Id=G2LIU.Level2SalesGroup_Id
                  LEFT JOIN Level3SalesGroup(NOLOCK) AG3 ON AG3.ParentSalesGroupId=AG2.Id OR AG3.Id=G3LIU.Level3SalesGroup_Id
                 LEFT JOIN SalesTeams(NOLOCK) T ON T.Level3SalesGroupId=AG3.Id OR T.Id=LIU.SalesTeamId
            WHERE LIU.Id=@UserId;
					--SELECT* FROM @LIUInfo
                   DECLARE @CompanyID[varchar](100) = (SELECT MIN(CompanyID) FROM @LIUInfo);

        DECLARE @UInfo table(CompanyID varchar(100), UserID varchar(100), FirstName varchar(100), LastName varchar(100), IsActive bit, IsExternalUserIDActive bit, UserName varchar(100), RoleName varchar(100), Rank integer, ExternalUserID varchar(100), SalesTeamID varchar(100), ExternalDisplayName varchar(100), Team varchar(100), Level1SalesGroup_Id varchar(100), Level2SalesGroup_Id varchar(100), Level3SalesGroup_Id varchar(100));
					INSERT INTO @UInfo(CompanyID,UserID,FirstName,LastName,IsActive,IsExternalUserIDActive,UserName,RoleName,[Rank],ExternalUserID,SalesTeamID,ExternalDisplayName,Team,Level1SalesGroup_Id,Level2SalesGroup_Id,Level3SalesGroup_Id)
                    SELECT U.CompanyID,U.Id AS UserID,U.FirstName,U.LastName,U.IsActive,U.IsExternalUserIDActive,U.UserName,R.Name AS RoleName,R.Rank,U.ExternalUserID,U.SalesTeamID,T.ExternalDisplayName,T.Name AS Team,UG1.Level1SalesGroup_Id,UG2.Level2SalesGroup_Id,UG3.Level3SalesGroup_Id
                    FROM AspNetUsers (NOLOCK) U
		                -- Returned user's info
                        LEFT JOIN AspNetUserRoles(NOLOCK) UR ON UR.UserId=U.Id
                       LEFT JOIN AspNetRoles(NOLOCK) R ON R.Id=UR.RoleID
		
		                -- Groups that returned user manages
                        LEFT JOIN Level1SalesGroupApplicationUser(NOLOCK) UG1 ON UG1.ApplicationUser_Id=U.Id
                       LEFT JOIN Level2SalesGroupApplicationUser(NOLOCK) UG2 ON UG2.ApplicationUser_Id=U.Id
                      LEFT JOIN Level3SalesGroupApplicationUser(NOLOCK) UG3 ON UG3.ApplicationUser_Id=U.Id

						-- Get the user's team
						LEFT JOIN SalesTeams(NOLOCK) T ON U.SalesTeamId=T.Id
                   WHERE 1=1
                        AND U.CompanyId=@CompanyId
                        AND U.IsDeleted= 0
                        AND (
                            U.FirstName LIKE '%'+@FilterUserName+'%' OR U.LastName LIKE '%'+@FilterUserName+'%' OR U.FirstName+' '+U.LastName LIKE '%'+@FilterUserName+'%'
                            OR U.Username LIKE '%'+@FilterUserName+'%'
                            OR U.ExternalUserID LIKE '%'+@FilterUserName+'%'
                            OR U.Email LIKE '%'+@FilterUserName+'%'
                            OR @FilterUserName IS NULL
                        )
                        AND(R.Rank= @FilterRank OR @FilterRank IS NULL)
                        AND(UR.UserId= @FilterUserID OR @FilterUserID IS NULL)
                    GROUP BY U.CompanyID, U.Id, U.FirstName, U.LastName, U.IsActive, U.IsExternalUserIDActive, U.Username, R.Name, R.Rank, U.ExternalUserID, U.SalesTeamID, T.ExternalDisplayName, T.Name, UG1.Level1SalesGroup_Id, UG2.Level2SalesGroup_Id, UG3.Level3SalesGroup_Id
                    ORDER BY U.FirstName,U.LastName,U.Username



                    SELECT U.UserID AS Id,U.FirstName,U.LastName,U.IsActive,U.UserName,U.RoleName AS Name,U.Rank,U.ExternalUserID,U.IsExternalUserIDActive,U.ExternalDisplayName,U.Team
                    FROM @UInfo U
                        LEFT JOIN @LIUInfo LIU ON LIU.UserID= @UserID
                    WHERE 1=1
                        AND (
                            LIU.Rank<=1 --Logged in user is an admin or above
                            OR U.Level1SalesGroup_Id= LIU.Level1ID--User is a manager for one of the Logged in user's Level 1 Groups
                            OR U.Level2SalesGroup_Id= LIU.Level2ID--User is a manager for one of the Logged in user's Level 2 Groups
                            OR U.Level3SalesGroup_Id= LIU.Level3ID--User is a manager for one of the Logged in user's Level 3 Groups
                            OR U.SalesTeamID= LIU.TeamID--User is assigned to one of the Logged in user's teams
		                )
                        AND LIU.Rank<U.Rank --User is a lower rank than the Logged in user
                    GROUP BY U.UserId,U.FirstName,U.LastName,U.IsActive,U.Username,U.RoleName,U.Rank,U.ExternalUserID,U.IsExternalUserIDActive,U.ExternalDisplayName,U.Team
                    ORDER BY U.FirstName,U.LastName,U.Username
                END
                ");

      
        }
        
        public override void Down()
        {
        }
    }
}

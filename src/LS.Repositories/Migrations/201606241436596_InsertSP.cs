namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertSP : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                CREATE PROCEDURE [dbo].[usp_CheckCommissionsForErrors]
                AS
                BEGIN
                    -- =============================================
                    -- Author:		Kevin White
                    -- Create date: 5/4/2016
                    -- Description:	A stored procedure to check for conditions that we consider bad
                    -- =============================================
                    SET NOCOUNT ON;

	                DECLARE @CheckPassed bit;
	                SET @CheckPassed = 1;

	                --Payment that has a transaction ID, but not date paid
	                IF EXISTS (
		                SELECT ID
		                FROM Payments
		                WHERE COALESCE(DatePaid, '')='' AND TransactionID IS NOT NULL
	                ) SET @CheckPassed = 0;

	                --Payment that has payment date, but no TransactionID
	                IF EXISTS (
		                SELECT ID
		                FROM Payments
		                WHERE COALESCE(DatePaid, '')!='' AND TransactionID IS NULL
	                ) SET @CheckPassed = 0;

	                --Unpaid payments that are associated with a Log that has no ProcessID, or have no log associated with it
	                IF EXISTS (
		                SELECT P.ID
		                FROM Payments P (NOLOCK)
			                LEFT JOIN CommissionLogs L (NOLOCK) ON P.ID=L.PaymentID
		                WHERE COALESCE(P.DatePaid, '')='' AND  L.ProcessID IS NULL
	                ) SET @CheckPassed = 0;

	                --Log that has a PaymentID that does not exist
	                IF EXISTS (
		                SELECT L.ID
		                FROM CommissionLogs L (NOLOCK)
			                LEFT JOIN Payments P (NOLOCK) ON P.ID=L.PaymentID
		                WHERE L.PaymentID IS NOT NULL AND P.ID IS NULL
	                ) SET @CheckPassed = 0;

	                --Log ProcessID that does not have any payments
	                IF EXISTS (
		                SELECT L.ProcessID
		                FROM CommissionLogs L (NOLOCK)
			                LEFT JOIN Payments P (NOLOCK) ON L.PaymentID=P.ID
		                WHERE L.ProcessID IS NOT NULL
		                GROUP BY L.ProcessID
		                HAVING COUNT(P.ID)=0
	                ) SET @CheckPassed = 0;

	                --Payments where the amount does not match the sum of the commission logs associated with it
	                IF EXISTS (
		                SELECT *
		                FROM Payments P
		                LEFT JOIN (
			                SELECT PaymentID, SUM(Amount) AS TotalAmount
			                FROM CommissionLogs
			                GROUP BY PaymentID
		                ) L ON P.ID=L.PaymentID
		                WHERE (L.TotalAmount IS NULL OR L.TotalAmount!=P.Amount)
			                AND COALESCE(P.TransactionID, '') NOT IN ('3ae576efb1a83','7c476e0981398') --These were alerady in the DB.  Don't want to consider them bad...
	                ) SET @CheckPassed = 0;

	                SELECT @CheckPassed AS CheckPassed;
                END
            ");

            this.Sql(@"
                CREATE PROCEDURE [dbo].[usp_CreateCommissionPaymentBatch]
                AS
                BEGIN
                    -- =============================================
                    -- Author:		Kevin White
                    -- Create date: 5/4/2016
                    -- Description:	A stored procedure to check for conditions that we consider bad
                    -- =============================================
                    SET NOCOUNT ON;
	                DECLARE @myProcessID varchar(50);
	                SET @myProcessID = NEWID();
	                --Commissions paid to a team
	                UPDATE C SET C.ProcessID=@myProcessID
	                --SELECT * 
	                FROM CommissionLogs C 
		                LEFT JOIN Orders O ON C.OrderID=O.Id AND C.OrderType='Account'
		                LEFT JOIN SalesTeams T ON C.SalesTeamID=T.Id 
	                WHERE (
			                (COALESCE(O.TenantAccountID, '')!='' AND O.IsDeleted=0) 
			                OR C.OrderType!='Account'
		                ) 
		                AND C.RecipientUserId IS NULL AND C.SalesTeamId IS NOT NULL AND COALESCE(T.PaypalEmail, '')!='' 
		                AND COALESCE(C.Amount, 0)>0 
		                AND C.IsDeleted=0
		                AND C.PaymentID IS NULL
		                AND C.ProcessID IS NULL;
	                --Commissions paid to a user
	                UPDATE C SET C.ProcessID=@myProcessID
	                --SELECT *
	                FROM CommissionLogs C 
		                LEFT JOIN Orders O ON C.OrderID=O.Id AND C.OrderType='Account'
		                LEFT JOIN ASPNetUsers U ON C.RecipientUserId=U.Id
	                WHERE (
			                (COALESCE(O.TenantAccountID, '')!='' AND O.IsDeleted=0) 
			                OR C.OrderType!='Account'
		                ) 
		                AND C.RecipientUserId IS NOT NULL AND COALESCE(U.PaypalEmail, '')!='' 
		                AND COALESCE(C.Amount, 0)>0 
		                AND C.IsDeleted=0
		                AND C.PaymentID IS NULL
		                AND C.ProcessID IS NULL;
	                IF OBJECT_ID('tempdb..#LSCommissions') IS NOT NULL DROP TABLE #LSCommissions;
	                CREATE TABLE #LSCommissions (Amount float, PaypalEmail varchar(500), CommissionID varchar(50), OrderID varchar(50), PaymentID varchar(50))
	                INSERT INTO #LSCommissions
	                SELECT C.Amount, T.PaypalEmail, C.Id AS CommissionID, C.OrderID, '' AS PaymentID
	                FROM CommissionLogs C 
		                LEFT JOIN Orders O ON C.OrderID=O.Id AND C.OrderType='Account'
		                LEFT JOIN SalesTeams T ON C.SalesTeamID=T.Id 
	                WHERE C.ProcessID=@myProcessID
		                AND C.RecipientUserId IS NULL AND C.SalesTeamId IS NOT NULL AND COALESCE(T.PaypalEmail, '')!='' 
		                AND C.PaymentID IS NULL;
	                INSERT INTO #LSCommissions
	                SELECT C.Amount, U.PaypalEmail, C.Id AS CommissionID, C.OrderID, '' AS PaymentID
	                FROM CommissionLogs C 
		                LEFT JOIN Orders O ON C.OrderID=O.Id AND C.OrderType='Account'
		                LEFT JOIN ASPNetUsers U ON C.RecipientUserId=U.Id
	                WHERE C.ProcessID=@myProcessID
		                AND C.RecipientUserId IS NOT NULL AND COALESCE(U.PaypalEmail, '')!='' 
		                AND C.PaymentID IS NULL 
	                ORDER BY OrderID;
	                IF OBJECT_ID('tempdb..#Payments') IS NOT NULL DROP TABLE #Payments;
	                CREATE TABLE #Payments (PaypalEmail varchar(500), PaymentID varchar(50))
	                INSERT INTO #Payments
	                SELECT PayPalEmail, NEWID() AS PaymentID
	                FROM #LSCommissions
	                GROUP BY PayPalEmail;
	                UPDATE C SET C.PaymentID=P.PaymentID
	                --SELECT *
	                FROM #LSCommissions C
	                LEFT JOIN #Payments P ON C.PayPalEmail=P.PayPalEmail;
	                INSERT INTO Payments(ID, Amount, Email, DateCreated, DateModified, IsDeleted)
	                SELECT PaymentID AS ID, SUM(Amount) AS Amount, PaypalEmail AS Email, getdate() AS DateCreated, getdate() AS DateModified, 0 AS IsDeleted
	                FROM #LSCommissions
	                GROUP BY PayPalEmail, PaymentID;
	                UPDATE L SET L.PaymentID=C.PaymentID, L.DateModified=getdate()
	                FROM CommissionLogs L
	                LEFT JOIN #LSCommissions C ON L.ID=C.CommissionID
	                WHERE C.CommissionID IS NOT NULL;
	                --SELECT * FROM #LSCommissions C ORDER BY C.PaypalEmail ASC, C.PaymentID ASC;
	                IF OBJECT_ID('tempdb..#LSCommissions') IS NOT NULL DROP TABLE #LSCommissions;
	                IF OBJECT_ID('tempdb..#Payments') IS NOT NULL DROP TABLE #Payments;
	
	                SELECT @myProcessID AS ProcessID; END
            ");
            this.Sql("UPDATE OrderStatuses SET Name='Pending RTR Completion', Description='RTR Approved, but not submitted to Solix (or Solix call failed)' WHERE StatusCode=1");
        }
        
        public override void Down()
        {
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrdersActivationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ActivationDate", c => c.DateTime(nullable: false));

            this.Sql(@"
                CREATE PROCEDURE [dbo].[usp_processActivations]
            
                AS
                BEGIN
		            --Update order details for any activations that did not have user detail when we first added the detail, but now do
		            UPDATE O SET O.ActivationUserID=UP.Id
		            FROM Orders (NOLOCK) O
			            LEFT JOIN ASPNetUsers (NOLOCK) U ON O.ActivationUserID=U.ID
			            LEFT JOIN ASPNetUsers (NOLOCK) UP ON O.ActivationUserID=UP.ExternalUserID
		            WHERE U.ID IS NULL AND O.ActivationUserID IS NOT NULL AND O.ActivationUserID!='' AND UP.ID IS NOT NULL

	                --Mark all activations that are associated with an order we don't have as processed
	                UPDATE Activations
	                SET DateProcessed=getdate()
	                WHERE DateProcessed IS NULL
		                AND EnrollmentNumber NOT IN (SELECT DISTINCT OrderCode AS EnrollmentNumber FROM Orders);

	                -- Get rid of all unprocessed duplicates.  We only want to process the most recent one
	                UPDATE A SET A.DateProcessed=getdate()
	                FROM Activations A (NOLOCK)
		                LEFT JOIN (
			                SELECT EnrollmentNumber, MAX(ActivationDate) AS MaxActivationDate
			                FROM Activations (NOLOCK)
			                WHERE DateProcessed IS NULL
			                GROUP BY EnrollmentNumber
		                ) AG ON A.EnrollmentNumber=AG.EnrollmentNumber AND A.ActivationDate=AG.MaxActivationDate
	                WHERE A.DateProcessed IS NULL AND AG.EnrollmentNumber IS NULL;

	                -- Actual update of any unprocessed rows
	                DECLARE @OrdersToUpdate TABLE(EnrollmentNumber varchar(50))

	                INSERT INTO @OrdersToUpdate(EnrollmentNumber)
	                SELECT A.EnrollmentNumber
	                FROM Activations A(NOLOCK)
		                LEFT JOIN Orders O(NOLOCK) ON A.EnrollmentNumber=O.OrderCode
	                WHERE A.DateProcessed IS NULL AND O.Id IS NOT NULL

	                UPDATE O SET O.DeviceIdentifier=A.ESN, O.ActivationDate=A.ActivationDate, O.ActivationUserID=COALESCE(U.Id, A.PromoCode)
	                FROM @OrdersToUpdate OU
		                LEFT JOIN Activations A (NOLOCK) ON OU.EnrollmentNumber=A.EnrollmentNumber
		                LEFT JOIN AspNetUsers U (NOLOCK) ON A.PromoCode=U.ExternalUserID
		                LEFT JOIN Orders O (NOLOCK) ON A.EnrollmentNumber=O.OrderCode
	                WHERE O.Id IS NOT NULL AND A.DateProcessed IS NULL;
                    
	                UPDATE A SET A.DateProcessed = getdate()
	                FROM @OrdersToUpdate O
		                LEFT JOIN Activations A(NOLOCK) ON O.EnrollmentNumber=A.EnrollmentNumber
	                WHERE A.DateProcessed IS NULL;
                END       
            ");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "ActivationDate");
        }
    }
}

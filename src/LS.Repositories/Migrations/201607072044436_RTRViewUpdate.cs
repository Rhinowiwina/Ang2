namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RTRViewUpdate : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                DROP VIEW [dbo].[v_RTRUnReviewedOrder]
            ");

            this.Sql(@"
                CREATE VIEW[dbo].[v_RTRUnReviewedOrder] AS
                    SELECT O.ID, O.DateCreated
		                , O.QBFirstName, O.QBLastName, O.QBSsn, O.QBDateOfBirth
		                , O.FirstName, O.MiddleInitial, O.LastName, O.SSN, O.DateOfBirth
		                , O.ContactPhoneNumber, O.EmailAddress
		                , O.ServiceAddressStreet1, O.ServiceAddressStreet2, O.ServiceAddressCity, O.ServiceAddressState, O.ServiceAddressZip
					    , O.ShippingAddressStreet1, O.ShippingAddressStreet2, O.ShippingAddressCity, O.ShippingAddressState, O.ShippingAddressZip
		                , O.BillingAddressStreet1, O.BillingAddressStreet2, O.BillingAddressCity, O.BillingAddressState, O.BillingAddressZip
		                , O.AIInitials, O.AINumHousehold, O.AIAvgIncome, O.AIFrequency
		                , O.LPProofNumber, O.LPProofImageFilename, O.IDProofImageFileName, O.SigFilename, O.InitialsFileName
		                , O.TpivBypass, O.TpivBypassSsnProofTypeId, O.TpivBypassSsnProofImageFileName, O.TpivBypassSsnProofNumber, O.TpivBypassDobProofTypeId, O.TpivBypassDobProofNumber
		                , U.FirstName+' '+U.LastName AS Order_Clerk
		                , T.Name AS Order_Team, T.ExternalDisplayName AS Order_Team_ExtName
		                , P.ProgramName AS LifelineProgramName, LPDT.Name AS ProgramProofName, TPDTS.Name AS TpivBypassSsnProofType, TPDTD.Name AS TpivBypassDobProofType
		                , CO.Name AS CheckedBy_Name, CO.UserID AS CheckedBy_ID, CO.System AS CheckedBy_System, CO.DateCheckedOut AS CheckedBy_Date
		                --, O.QBSSN_Dec, O.QBDOB_Dec, O.SSN_Dec, O.DOB_Dec
                    FROM Orders O (NOLOCK)
		                LEFT JOIN OrderCheckOuts CO (NOLOCK) ON O.ID=CO.OrderID AND CO.System IN ('CFRTR','RTR')
		                LEFT JOIN ASPNetUsers U (NOLOCK) ON U.Id=O.UserID
		                LEFT JOIN SalesTeams T (NOLOCK) ON T.Id=O.SalesTeamID
		                LEFT JOIN LifelinePrograms P (NOLOCK) ON P.Id=O.LifelineProgramID
		                LEFT JOIN ProofDocumentTypes LPDT (NOLOCK) ON LPDT.Id=O.LPProofTypeID
		                LEFT JOIN TPIVProofDocumentTypes TPDTS (NOLOCK) ON TPDTS.Id=O.TpivBypassSsnProofTypeId
		                LEFT JOIN TPIVProofDocumentTypes TPDTD (NOLOCK) ON TPDTD.Id=O.TpivBypassDobProofTypeId
                    WHERE O.IsDeleted = 0   
            ");
            this.Sql("DROP VIEW [dbo].[v_RTRUnReviewedOrders]");
            this.Sql(@"
                 CREATE VIEW[dbo].[v_RTRUnReviewedOrders] AS
                    SELECT O.ID, O.DateCreated, O.FirstName, O.MiddleInitial, O.LastName, O.ServiceAddressCity, O.ServiceAddressState
	                    , CO.Name AS CheckedBy_Name, CO.UserID AS CheckedBy_ID, CO.System AS CheckedBy_System, CO.DateCheckedOut AS CheckedBy_Date
                    FROM Orders O (NOLOCK)
	                    LEFT JOIN OrderCheckOuts CO (NOLOCK) ON O.ID=CO.OrderID AND CO.System IN ('CFRTR','RTR')
                    WHERE O.IsDeleted = 0 AND O.DateCreated>DateAdd(hh, -72, getdate())
	                    AND (O.StatusID IS NULL OR (O.StatusID>=0 AND O.StatusID<100))
            ");
        }
        
        public override void Down()
        {
        }
    }
}

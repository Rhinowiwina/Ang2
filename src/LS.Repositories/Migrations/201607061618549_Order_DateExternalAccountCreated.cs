namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Order_DateExternalAccountCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DateExternalAccountCreated", c => c.DateTime());
            this.Sql("UPDATE Orders SET DateExternalAccountCreated=RTR_Date WHERE DateExternalAccountCreated IS NULL AND RTR_Date IS NOT NULL AND OrderCode IS NOT NULL AND StatusID=100");

            this.Sql(@"
               UPDATE ComplianceStatements SET Statement='Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?'
                --SELECT * FROM ComplianceStatements
                WHERE Statement='Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.'

                UPDATE ComplianceStatements SET Statement='Does the customer certify that they or someone else in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons) receives the California LifeLine discount for wireless or home phone services?'
                --SELECT * FROM ComplianceStatements 
                WHERE Statement='Does the customer certify that they or someone else in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons) receives the California LifeLine discount for wireless or home phone services?<br><br>The leading providers of Lifeline service are listed below.' 
            ");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DateExternalAccountCreated");
        }
    }
}

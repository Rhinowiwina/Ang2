namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderCheckOutsCleanUp : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                ALTER TABLE [dbo].[OrderCheckOuts] DROP CONSTRAINT [PK_OrderCheckOuts]
            ");
            this.Sql(@"
                DELETE C
                FROM OrderCheckouts C
                LEFT JOIN (
                              SELECT OrderID, MAX(DateCheckedOut) AS DateCheckedOut
                              FROM OrderCheckouts
                              GROUP BY OrderID, System
                              HAVING COUNT(*)>1
                       ) DO ON C.OrderID=DO.OrderID AND C.DateCheckedOut=DO.DateCheckedOut
                WHERE C.OrderID IN (SELECT OrderID FROM OrderCheckouts GROUP BY OrderID, System HAVING COUNT(*)>1) AND DO.OrderID IS NULL
            ");
            this.Sql(@"
                ALTER TABLE [dbo].[OrderCheckOuts] ADD  CONSTRAINT [PK_OrderCheckOuts] PRIMARY KEY CLUSTERED([OrderID] ASC, [System] ASC)
            ");
        }
        
        public override void Down()
        {
            this.Sql(@"
                ALTER TABLE [dbo].[OrderCheckOuts] DROP CONSTRAINT [PK_OrderCheckOuts]
            ");
            this.Sql(@"
                ALTER TABLE [dbo].[OrderCheckOuts] ADD  CONSTRAINT [PK_OrderCheckOuts] PRIMARY KEY CLUSTERED([OrderID] ASC, [System] ASC, [UserID] ASC)
            ");
        }
    }
}

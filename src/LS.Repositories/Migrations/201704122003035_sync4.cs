namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sync4 : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.DeviceOrders",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            PONumber = c.String(maxLength: 128),
            //            InvoiceNumber = c.String(maxLength: 128),
            //            OrderDate = c.DateTime(nullable: false),
            //            ShipDate = c.DateTime(nullable: false),
            //            AgentDueDate = c.DateTime(nullable: false),
            //            ASGDueDate = c.DateTime(nullable: false),
            //            Level1SalesGroupID = c.String(maxLength: 128),
            //            IsReturned = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.DeviceOrderDevices",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 100),
            //            OrderID = c.String(maxLength: 100),
            //            IMEI = c.String(maxLength: 100),
            //            PartNumber = c.String(maxLength: 100),
            //            ASGPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            AgentPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            IsReturned = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.DeviceOrderDevices");
            //DropTable("dbo.DeviceOrders");
        }
    }
}

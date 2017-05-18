namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceOrdersDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DeviceOrders", "OrderDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.DeviceOrders", "ShipDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.DeviceOrders", "AgentDueDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.DeviceOrders", "ASGDueDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DeviceOrders", "ASGDueDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeviceOrders", "AgentDueDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeviceOrders", "ShipDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeviceOrders", "OrderDate", c => c.DateTime(nullable: false));
        }
    }
}

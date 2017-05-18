namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HandsetTableUpates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceOrders", "IsReturned", c => c.Boolean(nullable: false));
            AddColumn("dbo.DeviceOrderDevices", "OrderID", c => c.String(maxLength: 100));
            AddColumn("dbo.DeviceOrderDevices", "IsReturned", c => c.Boolean(nullable: false));
            DropColumn("dbo.DeviceOrders", "OrderArea");
            DropColumn("dbo.DeviceOrders", "OrderReturnedIndicator");
            DropColumn("dbo.DeviceOrderDevices", "HandsetReturnedIndicator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeviceOrderDevices", "HandsetReturnedIndicator", c => c.Boolean(nullable: false));
            AddColumn("dbo.DeviceOrders", "OrderReturnedIndicator", c => c.Boolean(nullable: false));
            AddColumn("dbo.DeviceOrders", "OrderArea", c => c.String(maxLength: 128));
            DropColumn("dbo.DeviceOrderDevices", "IsReturned");
            DropColumn("dbo.DeviceOrderDevices", "OrderID");
            DropColumn("dbo.DeviceOrders", "IsReturned");
        }
    }
}

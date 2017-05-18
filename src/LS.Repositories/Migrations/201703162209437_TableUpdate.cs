namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceOrders", "Level1SalesGroupID", c => c.String(maxLength: 128));
            DropColumn("dbo.DeviceOrders", "Level1Assignment");

            this.Sql("ALTER TABLE dbo.Activations ADD DeviceType [varchar](50)");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeviceOrders", "Level1Assignment", c => c.String(maxLength: 128));
            DropColumn("dbo.DeviceOrders", "Level1SalesGroupID");
        }
    }
}

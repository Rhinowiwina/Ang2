namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncIssues : DbMigration
    {
        public override void Up()
        {
           
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DeviceOrderDevices");
            DropTable("dbo.DeviceOrders");
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JWSync : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Orders", "IDProofImageID2", c => c.String());
            //AddColumn("dbo.Orders", "IDProofImageFilename2", c => c.String());
            //AddColumn("dbo.Orders", "DeviceCompatibility", c => c.String());
            //AddColumn("dbo.TempOrders", "IDProofImageID2", c => c.String());
            //AddColumn("dbo.TempOrders", "IDProofImageFilename2", c => c.String());
            //AddColumn("dbo.TempOrders", "DeviceCompatibility", c => c.String());
            //AddColumn("dbo.TempOrders", "DeviceModel", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.TempOrders", "DeviceModel");
            //DropColumn("dbo.TempOrders", "DeviceCompatibility");
            //DropColumn("dbo.TempOrders", "IDProofImageFilename2");
            //DropColumn("dbo.TempOrders", "IDProofImageID2");
            //DropColumn("dbo.Orders", "DeviceCompatibility");
            //DropColumn("dbo.Orders", "IDProofImageFilename2");
            //DropColumn("dbo.Orders", "IDProofImageID2");
        }
    }
}

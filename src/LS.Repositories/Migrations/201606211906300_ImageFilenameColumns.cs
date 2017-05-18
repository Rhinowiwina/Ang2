namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageFilenameColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "LPImageId", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "LPProofImageUploadFilename", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "IPImageId", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "IPProofImageUploadFilename", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "TpivBypassSsnImageId", c => c.String(maxLength: 128));
            AddColumn("dbo.Orders", "TpivSsnImageUploadFilename", c => c.String(maxLength: 128));
            AddColumn("dbo.TempOrders", "LPImageId", c => c.String(maxLength: 100));
            AddColumn("dbo.TempOrders", "LPProofImageUploadFilename", c => c.String());
            AddColumn("dbo.TempOrders", "IPImageId", c => c.String(maxLength: 100));
            AddColumn("dbo.TempOrders", "IPProofImageUploadFilename", c => c.String());
            DropColumn("dbo.Orders", "LPImageFileName");
            DropColumn("dbo.Orders", "IPImageFileName");
            DropColumn("dbo.Orders", "TpivBypassSsnImageFileName");
            DropColumn("dbo.TempOrders", "LPImageFileName");
            DropColumn("dbo.TempOrders", "IPImageFileName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TempOrders", "IPImageFileName", c => c.String(maxLength: 100));
            AddColumn("dbo.TempOrders", "LPImageFileName", c => c.String(maxLength: 100));
            AddColumn("dbo.Orders", "TpivBypassSsnImageFileName", c => c.String());
            AddColumn("dbo.Orders", "IPImageFileName", c => c.String(maxLength: 100));
            AddColumn("dbo.Orders", "LPImageFileName", c => c.String(maxLength: 100));
            DropColumn("dbo.TempOrders", "IPProofImageUploadFilename");
            DropColumn("dbo.TempOrders", "IPImageId");
            DropColumn("dbo.TempOrders", "LPProofImageUploadFilename");
            DropColumn("dbo.TempOrders", "LPImageId");
            DropColumn("dbo.Orders", "TpivSsnImageUploadFilename");
            DropColumn("dbo.Orders", "TpivBypassSsnImageId");
            DropColumn("dbo.Orders", "IPProofImageUploadFilename");
            DropColumn("dbo.Orders", "IPImageId");
            DropColumn("dbo.Orders", "LPProofImageUploadFilename");
            DropColumn("dbo.Orders", "LPImageId");
        }
    }
}

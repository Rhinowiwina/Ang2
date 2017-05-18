namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temptablecolumnfix : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.TempOrders", "TpivSsnImageUploadFilename", c => c.String());
            //DropColumn("dbo.TempOrders", "TpivBypassSsnImageFileName");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.TempOrders", "TpivBypassSsnImageFileName", c => c.String());
            //DropColumn("dbo.TempOrders", "TpivSsnImageUploadFilename");
        }
    }
}

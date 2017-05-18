namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.TempOrders", "TpivBypassSsnImageId", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.TempOrders", "TpivBypassSsnImageId");
        }
    }
}

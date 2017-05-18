namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderActivationUserID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ActivationUserID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "ActivationUserID");
        }
    }
}

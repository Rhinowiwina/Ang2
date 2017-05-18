namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppUser_AdditionalDataNeeded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AdditionalDataNeeded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AdditionalDataNeeded");
        }
    }
}

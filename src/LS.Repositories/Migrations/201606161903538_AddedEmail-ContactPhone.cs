namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmailContactPhone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "EmailRequiredForOrder", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "ContactPhoneRequiredForOrder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "ContactPhoneRequiredForOrder");
            DropColumn("dbo.Companies", "EmailRequiredForOrder");
        }
    }
}

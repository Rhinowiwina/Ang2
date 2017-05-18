namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToggleScrubAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "DoAddressScrub", c => c.Boolean(nullable: false));
            AddColumn("dbo.Companies", "DoWhiteListCheck", c => c.Boolean(nullable: false));
			this.Sql(@"update companies Set DoAddressScrub=1,DoWhiteListCheck=1 where id='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
		}
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "DoWhiteListCheck");
            DropColumn("dbo.Companies", "DoAddressScrub");
        }
    }
}

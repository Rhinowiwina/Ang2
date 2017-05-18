namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowNewAgentMenu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "ShowNewAgentRequestMenu", c => c.Boolean(nullable: false));
			this.Sql(@"Update Companies Set ShowNewAgentRequestMenu=1 where Id='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "ShowNewAgentRequestMenu");
        }
    }
}

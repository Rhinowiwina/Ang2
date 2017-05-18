namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApiLogEntriesNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ApiLogEntries", "DateEnded", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApiLogEntries", "DateEnded", c => c.DateTime(nullable: false));
        }
    }
}

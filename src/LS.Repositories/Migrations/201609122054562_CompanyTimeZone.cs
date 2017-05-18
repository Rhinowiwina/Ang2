namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyTimeZone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "TimeZone", c => c.String(maxLength: 100));
            AlterColumn("dbo.Companies", "OrderStart", c => c.String(maxLength: 100));
            AlterColumn("dbo.Companies", "OrderEnd", c => c.String(maxLength: 100));

            this.Sql("UPDATE Companies SET OrderStart='08:00', OrderEnd='00:00', TimeZone='Central Standard Time' WHERE Id='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Companies", "OrderEnd", c => c.String());
            AlterColumn("dbo.Companies", "OrderStart", c => c.String());
            DropColumn("dbo.Companies", "TimeZone");
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderCutOffTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "OrderStart", c => c.String());
            AddColumn("dbo.Companies", "OrderEnd", c => c.String());
            this.Sql(@"Update Companies Set OrderStart='06:00', OrderEnd= '22:00' WHERE Id='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
            }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "OrderEnd");
            DropColumn("dbo.Companies", "OrderStart");
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoCheck : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "DoPromoCodeCheck", c => c.Boolean(nullable: false));
			this.Sql(@"update companies Set DoPromoCodeCheck=1 where id='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "DoPromoCodeCheck");
        }
    }
}

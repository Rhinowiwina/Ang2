namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsExported : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsExported", c => c.Boolean());
            AddColumn("dbo.Orders", "DateExported", c => c.DateTime());

            this.Sql("ALTER TABLE Orders ADD CONSTRAINT DF_IsExported DEFAULT 0 FOR IsExported");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DateExported");
            DropColumn("dbo.Orders", "IsExported");
        }
    }
}

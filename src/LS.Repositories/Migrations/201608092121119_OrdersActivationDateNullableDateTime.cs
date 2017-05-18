namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrdersActivationDateNullableDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "ActivationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "ActivationDate", c => c.DateTime(nullable: false));
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class address_reason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AddressValidations", "Reason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AddressValidations", "Reason");
        }
    }
}

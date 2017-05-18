namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressValidationExtraColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AddressValidations", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.AddressValidations", "DateModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.AddressValidations", "ModifiedByUserID", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AddressValidations", "ModifiedByUserID");
            DropColumn("dbo.AddressValidations", "DateModified");
            DropColumn("dbo.AddressValidations", "DateCreated");
        }
    }
}

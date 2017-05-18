namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressValidation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressValidations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Street1 = c.String(maxLength: 128),
                        Street2 = c.String(maxLength: 128),
                        City = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        Zipcode = c.String(maxLength: 12),
                        IsShelter = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AddressValidations");
        }
    }
}

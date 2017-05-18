namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnBoardData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyStates",
                c => new
                    {
                    Id = c.String(nullable: false,maxLength: 128),
                    IsActive = c.Boolean(nullable: false,defaultValue: true),
                    CompanyID = c.String(maxLength: 128),
                    StateCode = c.String(),
                    DateCreated = c.DateTime(nullable: false),
                    DateModified = c.DateTime(nullable: false),
                    IsDeleted = c.Boolean(nullable: false,defaultValue: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserOnBoardData",
                c => new
                    {
                    Id = c.String(nullable: false,maxLength: 128),
                    CompanyID = c.String(maxLength: 128),
                    StreetAddress1 = c.String(maxLength: 75),
                    StreetAddress2 = c.String(maxLength: 75),
                    City = c.String(maxLength: 75),
                    State = c.String(maxLength: 50),
                    Zip = c.String(maxLength: 20),
                    PictureID = c.String(),
                    Ssn = c.String(maxLength: 15),
                    DateOfBirth = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    DateCreated = c.DateTime(nullable: false),
                    DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserOnBoardData");
            DropTable("dbo.CompanyStates");
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JWSyncFix : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.CompanyStates",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            IsActive = c.Boolean(nullable: false),
            //            CompanyID = c.String(),
            //            StateCode = c.String(),
            //            DateCreated = c.DateTime(nullable: false),
            //            DateModified = c.DateTime(nullable: false),
            //            IsDeleted = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.UserOnBoardDatas",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            CompanyID = c.String(),
            //            StreetAddress1 = c.String(),
            //            StreetAddress2 = c.String(),
            //            City = c.String(),
            //            State = c.String(),
            //            Zip = c.String(),
            //            PictureID = c.String(),
            //            Ssn = c.String(),
            //            DateOfBirth = c.DateTime(),
            //            Exported = c.Boolean(nullable: false),
            //            IsDeleted = c.Boolean(nullable: false),
            //            DateCreated = c.DateTime(nullable: false),
            //            DateModified = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //AddColumn("dbo.AspNetUsers", "AdditionalDataNeeded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            //DropColumn("dbo.AspNetUsers", "AdditionalDataNeeded");
            //DropTable("dbo.UserOnBoardDatas");
            //DropTable("dbo.CompanyStates");
        }
    }
}

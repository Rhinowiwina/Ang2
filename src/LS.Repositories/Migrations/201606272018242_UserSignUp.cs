namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSignUp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSignUps",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subcontractor = c.String(maxLength:100),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        DOB = c.String(maxLength: 25),
                        Ssn = c.String(maxLength:15),
                        Address1 = c.String(maxLength: 100),
                        Address2 = c.String(maxLength: 100),
                        City = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        Zipcode = c.String(maxLength: 25),
                        PhoneNumber = c.String(maxLength:25),
                        Email = c.String(maxLength: 100),
                        BackGroundCertificateID = c.String(maxLength: 100),
                        DrugScreenCertificateID = c.String(maxLength: 100),
                        GovernmentDocFilename = c.String(maxLength: 100),
                        DisclosureFilename = c.String(maxLength: 100),
                        AuthorizationFileName = c.String(maxLength: 100),
                        TrainingCertFileName = c.String(maxLength: 100),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserSignUps");
        }
    }
}

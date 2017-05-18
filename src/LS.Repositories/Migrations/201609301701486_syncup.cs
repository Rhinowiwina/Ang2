namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class syncup : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.AuditLogs",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 100),
            //            ModifiedByUserID = c.String(maxLength: 100),
            //            DateCreated = c.DateTime(nullable: false),
            //            TableName = c.String(maxLength: 100),
            //            TableRowID = c.String(maxLength: 100),
            //            TablePreviousData = c.String(),
            //            Reason = c.String(maxLength: 300),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.AddressValidations",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Street1 = c.String(maxLength: 128),
            //            Street2 = c.String(maxLength: 128),
            //            City = c.String(maxLength: 100),
            //            State = c.String(maxLength: 50),
            //            Zipcode = c.String(maxLength: 12),
            //            IsShelter = c.Boolean(nullable: false),
            //            Reason = c.String(),
            //            DateCreated = c.DateTime(nullable: false),
            //            DateModified = c.DateTime(nullable: false),
            //            ModifiedByUserID = c.String(maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.AddressValidations");
            //DropTable("dbo.AuditLogs");
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class auditlog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100),
                        ModifiedByUserID = c.String(maxLength: 100),
                        DateCreated = c.DateTime(nullable: false),
                        TableName = c.String(maxLength: 100),
                        TableRowID = c.String(maxLength: 100),
                        TablePreviousData = c.String(),
                        Reason = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AuditLogs");
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExternalDuplicateDataCompanyId : DbMigration
    {
        public override void Up()
        {
			this.Sql(@"ALTER TABLE dbo.ExternalDuplicateData ADD CompanyId nvarchar(128)");
        }
        
        public override void Down()
        {
        }
    }
}

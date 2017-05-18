namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExternalDatacompanyId : DbMigration
    {
        public override void Up()
        {
			this.Sql("ALTER TABLE dbo.ExternalDataTrueUp ADD CompanyId nvarchar(128)");
			this.Sql(@"Update dbo.ExternalDataTrueUp Set CompanyId ='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
			
		}
        
        public override void Down()
        {
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ZipcodeCoverage : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                CREATE TABLE[dbo].[ZipcodeCoverage](
                    [PostalCode][nvarchar](10) NOT NULL
                ) ON[PRIMARY]
            ");
        }
        
        public override void Down()
        {
        }
    }
}

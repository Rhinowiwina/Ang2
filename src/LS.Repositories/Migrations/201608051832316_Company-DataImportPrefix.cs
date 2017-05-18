namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyDataImportPrefix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "DataImportFilePrefix", c => c.String());
            this.Sql("UPDATE Companies SET DataImportFilePrefix='SLArrow'");
            this.Sql(@"
                INSERT INTO ExternalStorageCredentials (
                    Id,
                    AccessKey,
                    SecretKey,
                    Type,
                    System,
                    Path,
                    MaxImageSize,
                    CompanyId,
                    DateCreated,
                    DateModified,
                    IsDeleted
                )VALUES(
                    '189272df-d686-49bc-9bf9-e150f7c56dfb',
                    'AKIAI5GECKD7LWYDLKKQ',
                    'p4rP/nKEWDrG5O/0NcioU7CMcoWoF8UI+wAwODvF',
                    'DataImport',
                    'AWS',
                    'lifeserv.dev.arrow.dataimport',
                    '640',
                    '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',
                    '2016-05-17 16:14:27.023',
                    '2016-05-17 16:14:27.023',
                    'False'
                )               							
            ");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "DataImportFilePrefix");
        }
    }
}

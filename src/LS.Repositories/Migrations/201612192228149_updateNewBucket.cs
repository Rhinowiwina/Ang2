namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateNewBucket : DbMigration
    {
        public override void Up()
        {
            string enviroment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (enviroment == "DEV") {
                this.Sql("Insert Into ExternalStorageCredentials ( AccessKey,SecretKey,Path,Type,System,CompanyId,DateCreated,isDeleted,id,MaxImageSize,DateModified) Values('AKIAI5GECKD7LWYDLKKQ','p4rP/nKEWDrG5O/0NcioU7CMcoWoF8UI+wAwODvF','lifeserv.dev.arrow.dataimport/exported','DataExport','AWS','65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',GetDate(),0,NEWID(),640,GetDate())");
                        }
            if (enviroment == "Staging") {
                this.Sql("Insert Into ExternalStorageCredentials ( AccessKey,SecretKey,Path,Type,System,CompanyId,DateCreated,isDeleted,id,MaxImageSize,DateModified) Values('AKIAI5GECKD7LWYDLKKQ','p4rP/nKEWDrG5O/0NcioU7CMcoWoF8UI+wAwODvF','lifeserv.staging.arrow.dataimport/exported','DataExport','AWS','65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',GetDate(),0,NEWID(),640,GetDate())");
                }

            if (enviroment == "PROD") {
                this.Sql("Insert Into ExternalStorageCredentials ( AccessKey,SecretKey,Path,Type,System,CompanyId,DateCreated,isDeleted,id,MaxImageSize,DateModified) Values('AKIAI5GECKD7LWYDLKKQ','p4rP/nKEWDrG5O/0NcioU7CMcoWoF8UI+wAwODvF','lifeserv.arrow.dataimport/exported','DataExport','AWS','65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',GetDate(),0,NEWID(),640,GetDate())");
                }


           

            }

        public override void Down()
        {
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HideShowReportHandsetOrders : DbMigration
    {
        public override void Up()
        {
			this.Sql("ALTER TABLE dbo.companies ADD ShowHandsetOrders [bit]");
			this.Sql("ALTER TABLE dbo.companies ADD ShowReporting [bit]");
			this.Sql("Update dbo.companies Set ShowHandsetOrders=1,ShowReporting=1 where id='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
			this.Sql(@"INSERT INTO [dbo].[Companies]
           ([Id],
           [Name]
           ,[CompanyLogoUrl]
           ,[PrimaryColorHex]
           ,[SecondaryColorHex]
           ,[CompanySupportUrl]
           ,[MinToChangeTeam]
           ,[MaxCommission]
           ,[Notes]
           ,[DateCreated]
           ,[DateModified]
           ,[IsDeleted]
           ,[EmailRequiredForOrder]
           ,[ContactPhoneRequiredForOrder]
           ,[DataImportFilePrefix]
           ,[OrderStart]
           ,[OrderEnd]
           ,[TimeZone]
		   ,[ShowHandsetOrders]
		   ,[ShowReporting])
   SELECT 
      NEWID(),
      'Immerge'
      ,[CompanyLogoUrl]
      ,[PrimaryColorHex]
      ,[SecondaryColorHex]
      ,[CompanySupportUrl]
      ,[MinToChangeTeam]
      ,[MaxCommission]
      ,[Notes]
      ,[DateCreated]
      ,[DateModified]
      ,[IsDeleted]
      ,[EmailRequiredForOrder]
      ,[ContactPhoneRequiredForOrder]
      ,[DataImportFilePrefix]
      ,[OrderStart]
      ,[OrderEnd]
      ,[TimeZone]
	  ,0
      ,0
		
  FROM [dbo].[Companies] where Id='65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c'");
		}
        
        public override void Down()
        {
			this.Sql("ALTER TABLE dbo.companies DROP COLUMN ShowHandsetOrders");
			this.Sql("ALTER TABLE dbo.companies DROP COLUMN ShowReporting");
		}
    }
}

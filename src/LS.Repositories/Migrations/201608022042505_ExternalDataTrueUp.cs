namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExternalDataTrueUp : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                CREATE TABLE [dbo].[ExternalDataTrueUp](
	                [ID] [varchar](50) NOT NULL CONSTRAINT [DF_ExternalDataTrueUp_ID]  DEFAULT (newid()),
	                [ENROLLMENTNUMBER] [varchar](50) NULL,
	                [ENROLLMENTCHANNEL] [varchar](250) NULL,
	                [STATUS] [varchar](50) NULL,
	                [STATUSID] [varchar](50) NULL,
	                [PROMOCODE] [varchar](50) NULL,
	                [CREATIONDATE_EST] [varchar](50) NULL,
	                [STATE] [varchar](50) NULL,
	                [ZIPCODE] [varchar](50) NULL,
	                [CREATIONTIME] [varchar](50) NULL,
	                [USAC_FORM] [varchar](50) NULL,
	                [REP_NAME] [varchar](250) NULL,
	                [BATCH_DATE] [varchar](250) NULL,
	                [GROUP] [varchar](250) NULL,
	                [CHANNEL] [varchar](250) NULL,
	                [DMA] [varchar](250) NULL,
	                [GEOLOCATION] [varchar](50) NULL,
	                [DateImported] [datetime] NULL CONSTRAINT [DF_ExternalDataTrueUp_DateImported]  DEFAULT (getdate()),
                 CONSTRAINT [PK_ExternalDataTrueUp] PRIMARY KEY CLUSTERED 
                (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
            ");
        
        }
        
        public override void Down()
        {
        }
    }
}

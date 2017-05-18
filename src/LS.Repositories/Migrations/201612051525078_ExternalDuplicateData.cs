namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExternalDuplicateData : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                CREATE TABLE [dbo].[ExternalDuplicateData](
	                [ID] [varchar](50) NOT NULL CONSTRAINT [DF_ExternalDuplicateData_ID] DEFAULT (newid()),
	                [NEW_ENROLLMENT] [varchar](100) NULL,
	                [CREATIONDATE] [datetime] NULL,
	                [QUALIFYDATE] [datetime] NULL,
	                [ORGANIZATION] [varchar](100) NULL,
	                [AGENTNAME] [varchar](100) NULL,
	                [STATUS] [varchar](100) NULL,
	                [OLD_ENROLLMENTNUMBER] [varchar](100) NULL,
	                [CHECK_DEENROLL] [varchar](100) NULL,
	                [OLD_DEENROLL_DATE] [datetime] NULL,
	                [OLD_ENROLLMENT_STATUS] [varchar](100) NULL,
	                [OLD_ENROLLMENT_QUALIFYDATE] [datetime] NULL,
	                [EXPIRED] [varchar](100) NULL,
	                [DateImported] [datetime] NULL CONSTRAINT [DF_ExternalDuplicateData_DateImported]  DEFAULT (getdate()),
                 CONSTRAINT [PK_ExternalDuplicateData] PRIMARY KEY CLUSTERED 
                (
	                [ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
            ");

            this.Sql(@"
                ALTER TABLE [dbo].[Orders] ADD [IsDuplicate] [bit] NULL CONSTRAINT [DF_Orders_IsDuplicate]  DEFAULT ((0))
            ");

            this.Sql(@"
                ALTER TABLE [dbo].[Orders] ADD [DateMarkedDuplicate] [datetime] NULL
            ");

            this.Sql("UPDATE Orders SET IsDuplicate=0");
            }
        
        public override void Down()
        {
        }
    }
}

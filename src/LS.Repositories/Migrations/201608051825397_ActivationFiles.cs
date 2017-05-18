namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivationFiles : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                CREATE TABLE [dbo].[Activations](
                    [Id] [nvarchar](128) NULL,
	                [CompanyID] [nvarchar](128) NULL,
	                [PromoCode] [nvarchar](100) NULL,
	                [ESN] [nvarchar](100) NULL,
	                [EnrollmentNumber] [nvarchar](100) NULL,
	                [ActivationDate] [datetime] NOT NULL,
	                [ImportFilename] [nvarchar](100) NULL,
	                [DateProcessed] [datetime] NULL,
	                [DateCreated] [datetime] NOT NULL,
	                [DateModified] [datetime] NOT NULL,
	                [IsDeleted] [bit] NOT NULL
                ) ON [PRIMARY]
            ");
        }
        
        public override void Down()
        {
        }
    }
}

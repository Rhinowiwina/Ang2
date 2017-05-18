namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolixValidationDetail : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"CREATE TABLE [dbo].[SolixValidationDetails](
                    [IsBYOP] [varchar](1) NULL,
                    [IsRequalification] [varchar](1) NULL,
                    [RequalificationAppId] [nvarchar](25) NULL,
                    [RequalificationMDN][varchar](10)NULL,
                    [IsFreePhoneEligible] [varchar](1) NULL,
                    [OrderId] [nvarchar](128) NOT NULL,
                    [DateCreated] [datetime] NOT NULL,
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    CONSTRAINT [PK_SolixValidationDetails] PRIMARY KEY CLUSTERED 
                    (
                    [Id] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ) ON [PRIMARY]

                          ALTER TABLE [dbo].[SolixValidationDetails] ADD  CONSTRAINT [DF_SolixValidationDetails_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]");
        }
        
        public override void Down()
        {
            this.DropTable("[dbo].[SolixValidationDetails]");
        }
    }
}

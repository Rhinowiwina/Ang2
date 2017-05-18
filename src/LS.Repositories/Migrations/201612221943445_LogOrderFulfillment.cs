namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogOrderFulfillment : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                CREATE TABLE [dbo].[LogOrderFulfillment](
	                [OrderID] [varchar](70) NOT NULL,
	                [UserID] [varchar](70) NOT NULL,
	                [DateInitiated] [datetime] NOT NULL
                ) ON [PRIMARY]
            ");

            this.Sql("ALTER TABLE [dbo].[LogOrderFulfillment] ADD  CONSTRAINT [DF_LogOrderFulfillment_DateInitiated]  DEFAULT (getdate()) FOR [DateInitiated]");
            }
        
        public override void Down()
        {
        }
    }
}

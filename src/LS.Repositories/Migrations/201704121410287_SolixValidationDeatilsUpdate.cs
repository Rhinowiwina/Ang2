namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolixValidationDeatilsUpdate : DbMigration
    {
        public override void Up()
        {

            this.Sql("ALTER TABLE dbo.SolixValidationDetails ADD AgentCommission [varchar](1)");
        }
        
        public override void Down()
        {
        }
    }
}

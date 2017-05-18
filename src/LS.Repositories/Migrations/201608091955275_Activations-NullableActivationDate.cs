namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivationsNullableActivationDate : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                ALTER TABLE Orders ALTER COLUMN ActivationDate DateTime NULL        
            ");
        }
        
        public override void Down()
        {
        }
    }
}

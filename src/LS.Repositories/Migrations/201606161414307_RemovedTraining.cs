namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedTraining : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "RequiresTraining");
            DropColumn("dbo.AspNetUsers", "TrainingExpirationDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "TrainingExpirationDate", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "RequiresTraining", c => c.Boolean(nullable: false));
        }
    }
}

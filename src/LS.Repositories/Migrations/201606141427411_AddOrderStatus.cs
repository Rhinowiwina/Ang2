namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderStatus : DbMigration
    {
        public override void Up()
        {
            this.Sql("INSERT INTO OrderStatuses (Id, Name, Description, StatusCode, DateCreated, DateModified) VALUES ('70f40f46-a8d2-4c73-9db0-5d71e7b93bd8','Pending','Pending Order','1',getdate(),getdate())");
        }
        
        public override void Down()
        {
        }
    }
}

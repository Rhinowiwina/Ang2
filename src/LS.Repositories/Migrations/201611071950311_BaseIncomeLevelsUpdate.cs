namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseIncomeLevelsUpdate : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"
                IF NOT EXISTS (SELECT Base1Person FROM BaseIncomeLevels WHERE StateCode='CA' AND Base1Person=25900)
                BEGIN 
	                INSERT INTO BaseIncomeLevels(Id, StateCode, Base1Person, Base2Person, Base3Person, Base4Person, Base5Person, Base6Person, Base7Person, Base8Person, BaseAdditional, DateActive, DateCreated, DateModified, IsDeleted)
	                VALUES('E647D982-2EF0-401F-87DE-0D2D0F6F7004', 'CA', 25900, 25900, 30100, 36500, 42900, 49300, 55700, 62100, 6400, '2016-06-01 06:00:00.000', '2016-05-31 16:00:00.000', '2016-05-31 16:00:00.000', 0)
                END
            ");
        }
        
        public override void Down()
        {
        }
    }
}

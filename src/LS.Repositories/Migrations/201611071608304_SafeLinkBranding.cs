namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SafeLinkBranding : DbMigration
    {
        public override void Up()
        {
            this.Sql("UPDATE Companies SET CompanyLogoUrl='safelink-wireless.png', PrimaryColorHex='d9970e', SecondaryColorHex='7b7b7b', DateModified=getdate() WHERE Name='Arrow'");
        }
        
        public override void Down()
        {
            this.Sql("UPDATE Companies SET CompanyLogoUrl='Arrow-Sales-Group-Logo.png', PrimaryColorHex='A81217', SecondaryColorHex='6F6F6F', DateModified=getdate() WHERE Name='Arrow'");
        }
    }
}

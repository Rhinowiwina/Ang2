namespace LS.Repositories.Migrations {
    using System;
    using System.Data.Entity.Migrations;
    using System.Configuration;

    public partial class Update_Buckets : DbMigration {
        public override void Up() {
            string enviroment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (enviroment == "DEV") {
                this.Sql("Update ExternalStorageCredentials Set AccessKey='AKIAI6WJIMJG4355VN2A',SecretKey='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',Path='lifeserv.dev.arrow.signatures' where Type='Signatures' ");
                this.Sql("Update ExternalStorageCredentials Set AccessKey='AKIAIS6TH5HSGORXDPZA',SecretKey='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',Path='lifeserv.dev.arrow.proofs' where Type='Proof' ");
            }

            if (enviroment == "Staging") {
                this.Sql("Update ExternalStorageCredentials Set AccessKey='AKIAI6WJIMJG4355VN2A',SecretKey='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',Path='lifeserv.staging.arrow.signatures' where Type='Signatures' ");
                this.Sql("Update ExternalStorageCredentials Set AccessKey='AKIAIS6TH5HSGORXDPZA',SecretKey='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',Path='lifeserv.staging.arrow.proofs' where Type='Proof' ");
            }

            if (enviroment == "Prod") {
                this.Sql("Update ExternalStorageCredentials Set AccessKey='AKIAI6WJIMJG4355VN2A',SecretKey='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',Path='lifeserv.arrow.signatures' where Type='Signatures' ");
                this.Sql("Update ExternalStorageCredentials Set AccessKey='AKIAIS6TH5HSGORXDPZA',SecretKey='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',Path='lifeserv.arrow.proofs' where Type='Proof' ");
            }
        }

        public override void Down() {
        }
    }
}

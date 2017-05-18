namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExternalDataSourceLifelineSales : DbMigration
    {
        public override void Up()
        {

            this.Sql(@"
                CREATE TABLE DeviceOrderActivations( ESN nvarchar(100) NOT NULL );
                CREATE NONCLUSTERED INDEX [IX_DeviceOrderActivations_ESN] ON [dbo].[DeviceOrderActivations] ([ESN]);
            ");

            var enviroment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            if (enviroment == "DEV") {
                if (System.Configuration.ConfigurationManager.AppSettings["IsDeveloperMachine"] != "true") {
                    this.Sql(@"
                        CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'DaLXc97P8K7uH9wm';
                        CREATE DATABASE SCOPED CREDENTIAL LifelineSalesCRED WITH IDENTITY='lifelinesalesadmin', SECRET='DaLXc97P8K7uH9wm';  
                        CREATE EXTERNAL DATA SOURCE LifelineSaleEXTDATASRC WITH (
                            TYPE=RDBMS,
                           LOCATION='lifelinesalesdev.database.windows.net',
                           DATABASE_NAME='LifelineSalesDEV',
                           CREDENTIAL=LifelineSalesCRED,
                        );

                        CREATE EXTERNAL TABLE [dbo].[EXTTBL_LifelineSalesActivations](
                            [Id] [nvarchar](128) NOT NULL,
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
                        ) WITH (
                            DATA_SOURCE=LifelineSaleEXTDATASRC,
                            SCHEMA_NAME='dbo',
                            OBJECT_NAME='Activations'
                        );
                    ");
                }
            }
            if (enviroment == "Staging") {
                this.Sql(@"
                    CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'DaLXc97P8K7uH9wm';
                    CREATE DATABASE SCOPED CREDENTIAL LifelineSalesCRED WITH IDENTITY='lifelinesalesadmin', SECRET='DaLXc97P8K7uH9wm';  
                    CREATE EXTERNAL DATA SOURCE LifelineSaleEXTDATASRC WITH (
                        TYPE=RDBMS,
                       LOCATION='lifelinesalesdev.database.windows.net',
                       DATABASE_NAME='LifelineSalesSTAGE',
                       CREDENTIAL=LifelineSalesCRED,
                    );

                    CREATE EXTERNAL TABLE [dbo].[EXTTBL_LifelineSalesActivations](
                        [Id] [nvarchar](128) NOT NULL,
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
                    ) WITH (
                        DATA_SOURCE=LifelineSaleEXTDATASRC,
                        SCHEMA_NAME='dbo',
                        OBJECT_NAME='Activations'
                    );
                ");
            }

            if (enviroment == "PROD") {
                this.Sql(@"
                    CREATE MASTER KEY ENCRYPTION BY PASSWORD = '2GYCeRfXm5cynZB2';
                    CREATE DATABASE SCOPED CREDENTIAL LifelineSalesCRED WITH IDENTITY='lifelinesalesadmin', SECRET='2GYCeRfXm5cynZB2';  
                    CREATE EXTERNAL DATA SOURCE LifelineSaleEXTDATASRC WITH (
                        TYPE=RDBMS,
                       LOCATION='lifelinesalesprod.database.windows.net',
                       DATABASE_NAME='LifelineSalesPROD',
                       CREDENTIAL=LifelineSalesCRED,
                    );

                    CREATE EXTERNAL TABLE [dbo].[EXTTBL_LifelineSalesActivations](
                        [Id] [nvarchar](128) NOT NULL,
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
                    ) WITH (
                        DATA_SOURCE=LifelineSaleEXTDATASRC,
                        SCHEMA_NAME='dbo',
                        OBJECT_NAME='Activations'
                    );
                ");
            }
        }

        public override void Down()
        {
        }
    }
}

namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArrowSqlStatusID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "StatusID", c => c.String(maxLength: 50));

            this.Sql(@"
                UPDATE LifelinePrograms SET IsDeleted=1 WHERE ProgramName='Temporary Assistance for Needy Families (TANF)/CAlworks/Stan Works/Welfare to Work/GAIN' AND StateCode='CA';
                UPDATE ProofDocumentTypes SET IsDeleted=1 WHERE Name='Temporary Assistance to Needy Families (TANF)/CalWorks/Stan Works/Welfare to Work/GAIN award letter' AND StateCode='CA'
                DELETE FROM Budget_LifelinePrograms WHERE ProgramName='Temporary Assistance for Needy Families (TANF)/CAlworks/Stan Works/Welfare to Work/GAIN' AND BudgetCodeID=4;
                INSERT INTO LifelinePrograms ([Id], [ProgramName], [StateCode], [RequiresAccountNumber], [RequiredStateProgramId], [RequiredSecondaryStateProgramId], [DateCreated], [DateModified], [IsDeleted], [NladEligibilityCode]) VALUES (NEWID(), 'Temporary Assistance for Needy Families (TANF)', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                INSERT INTO LifelinePrograms ([Id], [ProgramName], [StateCode], [RequiresAccountNumber], [RequiredStateProgramId], [RequiredSecondaryStateProgramId], [DateCreated], [DateModified], [IsDeleted], [NladEligibilityCode]) VALUES (NEWID(), 'CalWORKS', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                INSERT INTO LifelinePrograms ([Id], [ProgramName], [StateCode], [RequiresAccountNumber], [RequiredStateProgramId], [RequiredSecondaryStateProgramId], [DateCreated], [DateModified], [IsDeleted], [NladEligibilityCode]) VALUES (NEWID(), 'StanWORKS', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                INSERT INTO LifelinePrograms ([Id], [ProgramName], [StateCode], [RequiresAccountNumber], [RequiredStateProgramId], [RequiredSecondaryStateProgramId], [DateCreated], [DateModified], [IsDeleted], [NladEligibilityCode]) VALUES (NEWID(), 'Welfare to Work', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                INSERT INTO LifelinePrograms ([Id], [ProgramName], [StateCode], [RequiresAccountNumber], [RequiredStateProgramId], [RequiredSecondaryStateProgramId], [DateCreated], [DateModified], [IsDeleted], [NladEligibilityCode]) VALUES (NEWID(), 'GAIN', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                INSERT INTO [dbo].[ProofDocumentTypes] ([Id], [ProofType], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'program', 'CalWORKS award letter', 'CA', GETDATE(), GETDATE(), 0)
                INSERT INTO [dbo].[ProofDocumentTypes] ([Id], [ProofType], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'program', 'StanWORKS award letter', 'CA', GETDATE(), GETDATE(), 0)
                INSERT INTO [dbo].[ProofDocumentTypes] ([Id], [ProofType], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'program', 'Welfare to Work award letter', 'CA', GETDATE(), GETDATE(), 0)
                INSERT INTO [dbo].[ProofDocumentTypes] ([Id], [ProofType], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'program', 'GAIN award letter', 'CA', GETDATE(), GETDATE(), 0)
                INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Temporary Assistance for Needy Families (TANF)', 4)
                INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'CalWORKS', 4)
                INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'StanWORKS', 4)
                INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Welfare to Work', 4)
                INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'GAIN', 4)
            ");

            this.Sql(@"
                CREATE TABLE [dbo].[Solix_LifelinePrograms](
	                [ProgramName] [varchar](250) NOT NULL,
	                [SolixProgramID] [int] NOT NULL
                ) ON [PRIMARY]
            ");
            this.Sql(@"
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Supplemental Security Income', 301)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Tribally-Administered Temporary Assistance for Needy Families', 304)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Women, Infants and Children Program ', 305)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Low Income Home Energy Assistance Program', 306)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Temporary Assistance for Needy Families (TANF)', 308)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Tribally-Administered Head Start Program (income based only)', 311)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Bureau of Indian Affairs General Assistance', 359)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Food Distribution Program on Indian Reservations', 376)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Medicaid/Medi-Cal', 377)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Federal Housing Assistance (Section 8)', 378)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Food Stamps', 379)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'National School Lunch (free program only)', 380)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'CalWORKS', 381)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'StanWORKS', 382)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Welfare to Work', 383)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'GAIN', 384)
                INSERT [dbo].[Solix_LifelinePrograms] ([ProgramName], [SolixProgramID]) VALUES (N'Supplemental Nutrition Assistance Program (SNAP)/Cal-Fresh', 379)
            ");

            this.Sql(@"
                DECLARE @ArrowID VARCHAR(MAX)  
                SET @ArrowID = (SELECT Id FROM Companies WHERE Name='Arrow');
                DELETE FROM CompanyTranslations WHERE CompanyID=@ArrowID AND Type='LifelineProgram';
                DELETE FROM CompanyTranslations WHERE CompanyID=@ArrowID AND Type='SolixLifelineProgram';

                INSERT INTO CompanyTranslations (ID, CompanyID, LSID, TranslatedID, [Type],IsDeleted,DateCreated,DateModified)
                SELECT CONVERT(NVARCHAR(50), NEWID()), @ArrowID AS CompanyID, P.Id AS LSID, BP.BudgetCodeID AS TranslatedID, 'LifelineProgram' AS [Type],0,GETDATE(),GETDATE()
                FROM LifelinePrograms P
                LEFT JOIN Budget_LifelinePrograms BP ON P.ProgramName=BP.ProgramName
                WHERE BP.BudgetCodeID IS NOT NULL AND BP.BudgetCodeID!='' AND P.IsDeleted=0

                INSERT INTO CompanyTranslations (ID, CompanyID, LSID, TranslatedID, [Type],IsDeleted,DateCreated,DateModified)
                SELECT CONVERT(NVARCHAR(50), NEWID()), @ArrowID AS CompanyID, P.Id AS LSID, BP.SolixProgramID AS TranslatedID, 'SolixLifelineProgram' AS [Type],0,GETDATE(),GETDATE()
                FROM LifelinePrograms P
                LEFT JOIN Solix_LifelinePrograms BP ON P.ProgramName=BP.ProgramName
                WHERE BP.SolixProgramID IS NOT NULL AND BP.SolixProgramID!='' AND P.IsDeleted=0
            ");

            this.Sql(@"
                CREATE TABLE [dbo].[OrderCheckOuts](
	                [OrderID] [varchar](50) NOT NULL,
	                [System] [varchar](50) NOT NULL,
	                [Name] [varchar](250) NOT NULL,
	                [UserID] [varchar](50) NOT NULL,
	                [DateCheckedOut] [datetime] NOT NULL,
                 CONSTRAINT [PK_OrderCheckOuts] PRIMARY KEY CLUSTERED 
                (
	                [OrderID] ASC,
	                [System] ASC,
	                [UserID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                )
            ");

            this.Sql(@"
                ALTER TABLE [dbo].[OrderCheckOuts] ADD  CONSTRAINT [DF_OrderCheckOuts_DateCreated]  DEFAULT (getdate()) FOR [DateCheckedOut]
            ");


            this.Sql(@"
                CREATE TABLE [dbo].[OrderStatuses](
	                [Id] [varchar](50) NOT NULL,
	                [Name] [varchar](50) NOT NULL,
	                [Description] [varchar](250) NOT NULL,
	                [StatusCode] [int] NOT NULL CONSTRAINT [DF_OrderStatuses_StatusCode]  DEFAULT ((0)),
	                [DateCreated] [datetime] NOT NULL CONSTRAINT [DF_OrderStatuses_DateCreated]  DEFAULT (getdate()),
	                [DateModified] [datetime] NOT NULL CONSTRAINT [DF_OrderStatuses_DateModified]  DEFAULT (getdate()),
                 CONSTRAINT [PK_OrderStatuses] PRIMARY KEY CLUSTERED 
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                )
            ");

            this.Sql(@"
                INSERT [dbo].[OrderStatuses] ([Id], [Name], [Description], [StatusCode], [DateCreated], [DateModified]) VALUES (N'62AC2556-B1B8-4E10-B163-936939EEC234', N'Submitted', N'Default value for submitted orders', 0, CAST(N'2016-06-06 21:22:34.217' AS DateTime), CAST(N'2016-06-06 21:22:34.217' AS DateTime))
                INSERT [dbo].[OrderStatuses] ([Id], [Name], [Description], [StatusCode], [DateCreated], [DateModified]) VALUES (N'66D7DD2A-F1C5-4B6F-A888-DE155235DDDD', N'RTR Rejected', N'Rejected by RTR', -100, CAST(N'2016-06-06 21:23:49.600' AS DateTime), CAST(N'2016-06-06 21:23:49.600' AS DateTime))
                INSERT [dbo].[OrderStatuses] ([Id], [Name], [Description], [StatusCode], [DateCreated], [DateModified]) VALUES (N'E1E0D0A8-1C54-4670-9667-E0DF0DF120BF', N'RTR Approved', N'Approved by RTR', 100, CAST(N'2016-06-06 21:23:21.823' AS DateTime), CAST(N'2016-06-06 21:23:21.823' AS DateTime))
            ");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "StatusID");
        }
    }
}

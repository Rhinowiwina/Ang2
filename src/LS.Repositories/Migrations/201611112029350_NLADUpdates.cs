namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NLADUpdates : DbMigration
    {
        public override void Up()
        {
            this.Sql(@"UPDATE LifelinePrograms SET NLADEligibilityCode='E1' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='1')
UPDATE LifelinePrograms SET NLADEligibilityCode='E2' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='2')
UPDATE LifelinePrograms SET NLADEligibilityCode='E4' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='3')
UPDATE LifelinePrograms SET NLADEligibilityCode='E6' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='4')
UPDATE LifelinePrograms SET NLADEligibilityCode='E7' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='5')
UPDATE LifelinePrograms SET NLADEligibilityCode='E13' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='6')
UPDATE LifelinePrograms SET NLADEligibilityCode='E3' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='7')
UPDATE LifelinePrograms SET NLADEligibilityCode='E5' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='8')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='16')
UPDATE LifelinePrograms SET NLADEligibilityCode='E2' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='17')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='18')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='19')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='20')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='21')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='22')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='23')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='24')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='25')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='26')
UPDATE LifelinePrograms SET NLADEligibilityCode='E10' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='27')
UPDATE LifelinePrograms SET NLADEligibilityCode='E8' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='28')
UPDATE LifelinePrograms SET NLADEligibilityCode='E9' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='29')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='30')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='31')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='32')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='33')
UPDATE LifelinePrograms SET NLADEligibilityCode='E11' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='40')
UPDATE LifelinePrograms SET NLADEligibilityCode='E7' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='41')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='42')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='43')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='44')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='45')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='46')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='47')
UPDATE LifelinePrograms SET NLADEligibilityCode='E11' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='48')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='53')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='54')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='55')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='56')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='57')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='58')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='62')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='63')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='64')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='65')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='66')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='67')
UPDATE LifelinePrograms SET NLADEligibilityCode='E12' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='68')
UPDATE LifelinePrograms SET NLADEligibilityCode='E2' WHERE ID IN (SELECT LSID AS ID FROM CompanyTranslations WHERE Type='LifelineProgram' AND TranslatedID='69')
");
            AddColumn("dbo.LifelinePrograms", "DateEnd", c => c.DateTime(nullable: false));
            AddColumn("dbo.LifelinePrograms", "DateStart", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProofDocumentTypes", "DateStart", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProofDocumentTypes", "DateEnd", c => c.DateTime(nullable: false));
            AddColumn("dbo.BaseIncomeLevels", "IncomeLevel", c => c.Int(nullable: false));
            DropColumn("dbo.StateSettings", "IncomeLevel");
            this.Sql(@"UPDATE StateAgreements SET IsDeleted=1 WHERE ID IN (SELECT StateAgreementParentId AS ID FROM StateAgreements WHERE StateAgreementParentId IS NOT NULL)");
            this.Sql(@"UPDATE StateAgreements SET StateAgreementParentId=NULL WHERE StateAgreementParentId IS NOT NULL;");
            //DropColumn("dbo. StateSettings","IncomeLevel");
            this.Sql(@"UPDATE LifelinePrograms SET DateStart = '1/1/2005 06:00',DateEnd='1/1/2100 06:00'");
            //do all rows
            this.Sql(@"UPDATE ProofDocumentTypes SET DateStart = '1/1/2005 06:00',DateEnd='1/1/2100 06:00'");
            //update date ends values
            this.Sql(@"UPDATE LifelinePrograms SET DateEnd='12/2/2016 06:00' 
                        --SELECT * FROM LifelinePrograms
                        WHERE NLADEligibilityCode IN ('E5','E6','E7','E12')

                        UPDATE ProofDocumentTypes SET DateEnd='12/2/2016 06:00' 
                        --SELECT * FROM ProofDocumentTypes
                        WHERE Name IN ('Certificate of Eligbility for Exchange Vistor status (only if Section 5 is completed)','Certified by the OK Tax Commission to participate in or receive assistance or benefits pursuant to the Sales Tax Relief Act, 68 O.S. Â§ 5011 et seq.','Certified program by the State Dept. of Rehabilitation, providing vocational rehabilitation assistance or benefits, including hearing impaired)','Disabled and Children (EAEDC) award letter','Electrical Universal Service Program (EUSP) award letter','E-MAC award letter','Emergency Aid to the Elderly','Emergency Assistance Program award letter','Family Independence Program (FIP) award letter','General Public Assistance (GPA) award letter','General/Disability Assistance award letter','Home Energy Assistance Program (HEAP) award letter','Kids Connection card/award letter','Last 3 consecutive months paycheck stubs','Last 3 consecutive paycheck stubs','Low Income Home Energy Assistance Program (LIHEAP) award letter','MAC card/award letter','Maine Care award letter/card','Maryland Energy Assistance Program (MEAP) award letter','MassHealth card/award letter','Medical Assistance (MA) card/award letter','Minnesota Family Investment Program (MFIP) award letter','MO Health','National Administered Free Lunch Program','National Administered Free Lunch Program Benfits Letter','Public Assistance to Adults (PAA) awards letter','Rhode Island Medical Assistance Program (MAP) award letter','Rhode Island Pharmaceutical Assistance to Elderly (RIPAE) award letter','Rhode Island Works (Formerly known as AFDC) award letter','SAM card/award letter','School Clothing Allowance (SCA) award letter','SSI - Blind and Disabled (SSDI) award letter','State Children’s Health Insurance Plan (SCHIP) or KidsCare (Medicaid)','Temporary Assistance to Needy Families (TANF) award letter','Temporary Assistance to Needy Families (TANF)/CalWorks/Stan Works/Welfare to Work/GAIN award letter','Temporary Cash Assistance (TCA) awards letter','Temporary Disability Assistance Program (TDAP) awards letter','Transitional Aid to Families with Dependent Children (TAFDC) award letter','Tribally Administered Heard Start Program award letter','Utility Bill with Senior Discount / Senior Citizen Low-income Discount plan letter','WI Homestead Tax Credit (Schedule H) award letter','Wisconsin Works (W2) statement/award letter','Women, Infants and Children Program card/awards letter/voucher','Worker''s Compensation Appeals Board Award Letter','WV Children''s Health Insurance Program (WV CHIP) card/award letter')
                       ");
            //add new programs
            this.Sql(
                @"DECLARE @statecodes TABLE (
                        vstatecode nvarchar(2)
                        );
                        insert into  @statecodes (vstatecode) 
                        Select statecode from Lifelineprograms GROUP BY statecode;

                        Select vstatecode from @statecodes;

                        Insert Into LifelinePrograms (Id,ProgramName,StateCode,RequiresAccountNumber, DateCreated, DateModified,
                        IsDeleted,NladEligibilityCode,DateStart,DateEnd)

                        Select NewID() as Id,'Veterans Pension/Survivors Pension',s.vstatecode,0,GetDate() as DateCreated,GetDate() as DateModified,
                        0,'E15','12/02/2016 06:00','1/1/2100 06:00'

                        From (Select vstatecode From @statecodes Group By vstatecode) s");

            this.Sql(@"Insert Into CompanyTranslations (Id,CompanyID,LSID,TranslatedID,Type, DateCreated, DateModified,
                        IsDeleted)
			Select NewId(),'65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',s.progId,70,'LifelineProgram',GetDate(),GetDate(),0
			From(Select Id as progId from Lifelineprograms where ProgramName='Veterans Pension/Survivors Pension') S");



            this.Sql(
               @"INSERT INTO ProofDocumentTypes(Id,ProofType,Name,StateCode,DateCreated, DateModified,IsDeleted,DateStart,DateEnd)
                 Select NEWID(),'program','Veterans Pension Grant Letter',S.StateCode,GetDate(),GetDate(),0,'12/2/2016 05:00','1/1/2100 06:00'
				 From(Select StateCode from ProofDocumentTypes Group By StateCode) S
                ");
            this.Sql(
               @"INSERT INTO ProofDocumentTypes(Id,ProofType,Name,StateCode,DateCreated, DateModified,IsDeleted,DateStart,DateEnd)
                 Select NEWID(),'program','Veterans Pension COLA Adjustment Letter',S.StateCode,GetDate(),GetDate(),0,'12/2/2016 05:00','1/1/2100 06:00'
				 From(Select StateCode from ProofDocumentTypes Group By StateCode) S
                ");
            this.Sql(
               @"INSERT INTO ProofDocumentTypes(Id,ProofType,Name,StateCode,DateCreated, DateModified,IsDeleted,DateStart,DateEnd)
                 Select NEWID(),'program','Survivors Pension Benefit Summary Letter',S.StateCode,GetDate(),GetDate(),0,'12/2/2016 05:00','1/1/2100 06:00'
				 From(Select StateCode from ProofDocumentTypes Group By StateCode) S
                ");
            this.Sql(@"Update StateAgreements Set StateAgreementParentId=NULL");

            //add old baseincome percentages and insert rows with new percentages
            this.Sql(@"Update BaseIncomeLevels Set IncomeLevel=135");
            this.Sql(@"Update BaseIncomeLevels Set IncomeLevel=150 where StateCode IN('KS','AZ','CA','MI','TX','OH')");
            this.Sql(@"Update BaseIncomeLevels Set IncomeLevel=175 where StateCode='NV'");

            this.Sql(
                    @"INSERT INTO BaseIncomeLevels(Id,StateCode,Base1Person,Base2Person,Base3Person,Base4Person,Base5Person,Base6Person,Base7Person,Base8Person,BaseAdditional,DateActive,DateCreated,DateModified,IsDeleted,IncomeLevel)
                Select NEWID(),S.StateCode,16038,21627,27216,32805,38394,43983,49586,55202,56616,'12/2/2016 06:00',GETDATE(),GETDATE(),0 ,135
                  From (Select StateCode From BaseIncomeLevels Where StateCode IN('NV','AZ','KS','OH','MI')) S                  
               ");
            }
        
        public override void Down()
        {
            AddColumn("dbo.StateSettings", "IncomeLevel", c => c.Int(nullable: false));
            DropColumn("dbo.BaseIncomeLevels", "IncomeLevel");
            DropColumn("dbo.ProofDocumentTypes", "DateEnd");
            DropColumn("dbo.ProofDocumentTypes", "DateStart");
            DropColumn("dbo.LifelinePrograms", "DateStart");
            DropColumn("dbo.LifelinePrograms", "DateEnd");
            this.Sql(@"Delete from BaseIncomeLevels where DateActive='12/2/2016 06:00'");
            this.Sql(@"Delete from ProofDocumentTypes WHERE Name IN ('Veterans Pension COLA Adjustment Letter', 'Survivors Pension Benefit Summary Letter', 'Veterans Pension Grant Letter')");
            this.Sql(@"Delete from LifelinePrograms where ProgramName='Veterans Pension/Survivors Pension'");
            }
        }
    }


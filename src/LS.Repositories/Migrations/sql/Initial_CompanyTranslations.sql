SELECT * FROM LifelinePrograms P
LEFT JOIN Budget_LifelinePrograms BP ON P.ProgramName=BP.ProgramName
WHERE BP.ProgramName IS NULL
DECLARE @BudgetID VARCHAR(MAX)  
SET @BudgetID = (SELECT Id FROM Companies WHERE Name='Budget Mobile');
DELETE FROM CompanyTranslations WHERE CompanyID=@BudgetID AND Type='LifelineProgram';

INSERT INTO CompanyTranslations (ID, CompanyID, LSID, TranslatedID, [Type])
SELECT CONVERT(NVARCHAR(50), NEWID()), @BudgetID AS CompanyID, P.Id AS LSID, BP.BudgetCodeID AS TranslatedID, 'LifelineProgram' AS [Type]
FROM LifelinePrograms P
LEFT JOIN Budget_LifelinePrograms BP ON P.ProgramName=BP.ProgramName
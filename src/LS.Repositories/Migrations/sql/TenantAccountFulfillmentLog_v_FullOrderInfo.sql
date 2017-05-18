CREATE VIEW [dbo].[v_FullOrderInfo] AS
SELECT O.*
	, T.ExternalDisplayName, T.Name AS SalesTeam, T.PayPalEmail AS TeamPaypalEmail
	, U.FirstName AS Emp_FName, U.LastName AS Emp_LName, U.PayPalEmail AS UserPayPalEmail
	, BLP.TranslatedID AS BLPID
	, LPP.Name AS ProgramProofType, IDP.Name AS IDProofType
	, SP.Name AS StateProgramType, SP2.Name AS SecondaryStateProgramType
	, SSNP.Name AS TPIVBypassSSNDocType, SSNP.LifelineSystemID AS TPIVBypassSSNTCode, DOBP.Name AS TPIVBypassDOBDocType, DOBP.LifelineSystemID AS TPIVBypassDOBTCode
FROM Orders O
	LEFT JOIN SalesTeams T ON O.SalesTeamID=T.ID
	LEFT JOIN AspNetUsers U ON O.UserID=U.ID
	LEFT JOIN CompanyTranslations BLP ON O.LifelineProgramID=BLP.LSID AND BLP.Type='LifelineProgram' AND O.CompanyID=BLP.CompanyID
	LEFT JOIN ProofDocumentTypes LPP ON O.LPProofTypeID=LPP.ID
	LEFT JOIN ProofDocumentTypes IDP ON O.IPProofTypeID=IDP.ID
	LEFT JOIN StatePrograms SP ON O.StateProgramID=SP.ID
	LEFT JOIN StatePrograms SP2 ON O.StateProgramID=SP2.ID
	LEFT JOIN TPIVProofDocumentTypes SSNP ON O.TPIVBypassSSNProofTypeID=SSNP.ID
	LEFT JOIN TPIVProofDocumentTypes DOBP ON O.TPIVBypassDOBProofTypeID=DOBP.ID
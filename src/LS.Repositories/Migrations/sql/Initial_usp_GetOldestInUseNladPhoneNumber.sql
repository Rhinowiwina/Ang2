-- =============================================
-- Author:		Jacob Rieger
-- Create date: 6/2/2015
-- Description:	A stored procedure to retrieve a valid Nlad phone number by returning the value of
-- the oldest in use number and updating it's date modified value
-- =============================================
SET NOCOUNT ON;
UPDATE TOP(1) dbo.NladPhoneNumbers
SET DateModified = GETUTCDATE()
OUTPUT INSERTED.Number
WHERE CompanyId = @companyId AND ID = (SELECT TOP(1) Id FROM dbo.NladPhoneNumbers ORDER BY DateModified)
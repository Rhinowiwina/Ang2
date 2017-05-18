-- =============================================
-- Author:		Jacob Rieger
-- Create date: 6/2/2015
-- Description:	A stored procedure to retrieve a valid Nlad phone number by returning the value and
-- settings IsCurrentlyInUse to true
-- =============================================
SET NOCOUNT ON;

-- Insert statements for procedure here
UPDATE TOP(1) dbo.NladPhoneNumbers
SET IsCurrentlyInUse = 1, DateModified = GETUTCDATE()
OUTPUT INSERTED.Number
WHERE IsCurrentlyInUse = 0 AND CompanyId = @companyId
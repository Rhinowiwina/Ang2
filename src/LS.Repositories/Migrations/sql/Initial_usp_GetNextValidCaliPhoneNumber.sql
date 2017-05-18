-- =============================================
-- Author:		Jacob Rieger
-- Create date: 6/2/2015
-- Description:	A stored procedure to retrieve a valid Cali phone number by returning the value and
-- incrementing the existing value by 1
-- =============================================
SET NOCOUNT ON;
UPDATE dbo.CaliPhoneNumbers
SET Number = 
	CASE WHEN (Number >= 9999999)
		THEN 0
	ELSE
		Number + 1
	END
OUTPUT CONVERT(varchar(3), INSERTED.AreaCode) + CONVERT(varchar(7), INSERTED.Number)
WHERE CompanyId = @companyId

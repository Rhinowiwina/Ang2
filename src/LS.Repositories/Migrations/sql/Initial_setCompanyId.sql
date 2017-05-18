
CREATE PROCEDURE [dbo].[usp_SetCompanyId]
	-- Add the parameters for the stored procedure here
	@companyid [nvarchar](128),
	@id [nvarchar](128)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	update dbo.AspNetUsers
	SET [CompanyId]=@companyid 
	WHERE [Id]=@id;
END




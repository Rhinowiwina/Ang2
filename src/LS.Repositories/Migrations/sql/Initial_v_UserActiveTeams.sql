CREATE VIEW [dbo].[v_UserActiveTeams] AS
	SELECT *
	FROM v_UserTeams
	WHERE IsDeleted=0 AND Level1IsDeleted=0 AND Level2IsDeleted=0 AND Level3IsDeleted=0

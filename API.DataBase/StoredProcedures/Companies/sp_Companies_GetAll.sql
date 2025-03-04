CREATE PROCEDURE [dbo].[sp_Companies_GetAll]
AS
BEGIN
    SELECT C.[Id],
           C.[Name],
           C.[Description],
           C.[UpdatedAt],
           C.[CreatedAt]
    FROM [dbo].[Companies] C
END
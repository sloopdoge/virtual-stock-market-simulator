CREATE PROCEDURE [dbo].[sp_Companies_GetById] @CompanyId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT C.[Id],
           C.[Name],
           C.[Description],
           C.[UpdatedAt],
           C.[CreatedAt]
    FROM [dbo].[Companies] C
    WHERE C.[Id] = @CompanyId
END

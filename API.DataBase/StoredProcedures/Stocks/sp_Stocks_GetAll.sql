CREATE PROCEDURE [dbo].[sp_Stocks_GetAll]
AS
BEGIN
    SELECT S.[Id],
           S.[Symbol],
           S.[Name],
           S.[Price],
           S.[CompanyId],
           S.[CreatedAt],
           S.[UpdatedAt]
    FROM [dbo].[Stocks] S
END

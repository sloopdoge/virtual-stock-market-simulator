CREATE PROCEDURE [dbo].[sp_Stocks_Delete] @Id INT
AS
BEGIN
    DELETE
    FROM [dbo].[Stocks]
    WHERE [Id] = @Id
END

CREATE PROCEDURE [dbo].[sp_Stocks_Create] @Id INT OUTPUT,
                                          @Symbol NVARCHAR(50),
                                          @Name NVARCHAR(255),
                                          @Price DECIMAL,
                                          @CompanyId UNIQUEIDENTIFIER,
                                          @CreatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Stocks] ([Symbol], [Name], [Price], [CompanyId], [CreatedAt])
    VALUES (@Symbol, @Name, @Price, @CompanyId, @CreatedAt)

    SET @Id = SCOPE_IDENTITY()

    SELECT S.[Id],
           S.[Symbol],
           S.[Name],
           S.[Price],
           S.[CompanyId],
           S.[CreatedAt],
           S.[UpdatedAt],
           S.[LastUpdate]
    FROM [dbo].[Stocks] S
    WHERE [Id] = @Id
END

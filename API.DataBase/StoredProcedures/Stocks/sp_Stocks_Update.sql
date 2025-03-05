CREATE PROCEDURE [dbo].[sp_Stocks_Update] @Id INT,
                                          @Symbol NVARCHAR(50),
                                          @Name NVARCHAR(255),
                                          @Price DECIMAL,
                                          @CompanyId UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE [dbo].[Stocks]
    SET [Symbol]    = @Symbol,
        [Name]      = @Name,
        [Price]     = @Price,
        [CompanyId] = @CompanyId
    WHERE [Id] = @Id

    SELECT S.[Id],
           S.[Symbol],
           S.[Name],
           S.[Price],
           S.[CompanyId],
           S.[CreatedAt],
           S.[UpdatedAt]
    FROM [dbo].[Stocks] S
    WHERE S.[Id] = @Id
END

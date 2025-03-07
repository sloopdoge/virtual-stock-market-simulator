CREATE PROCEDURE [dbo].[sp_Stocks_Create]
    @Symbol NVARCHAR(50),
    @Name NVARCHAR(255),
    @Price DECIMAL(38, 10),
    @CompanyId UNIQUEIDENTIFIER,
    @CreatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;

    INSERT INTO [dbo].[Stocks] ([Symbol], [Name], [Price], [CompanyId], [CreatedAt])
    VALUES (@Symbol, @Name, @Price, @CompanyId, @CreatedAt);

    SET @Id = SCOPE_IDENTITY();

    SELECT S.[Id],
           S.[Symbol],
           S.[Name],
           S.[Price],
           S.[CompanyId],
           S.[CreatedAt],
           S.[UpdatedAt],
           S.[LastUpdate]
    FROM [dbo].[Stocks] S
    WHERE S.[Id] = @Id;
END;

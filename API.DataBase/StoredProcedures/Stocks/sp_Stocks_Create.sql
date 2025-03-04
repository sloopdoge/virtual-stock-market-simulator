CREATE PROCEDURE [dbo].[sp_Stocks_Create] @Id INT,
                                          @Symbol NVARCHAR(50),
                                          @Name NVARCHAR(255),
                                          @Price DECIMAL,
                                          @CompanyId UNIQUEIDENTIFIER
AS
BEGIN
    INSERT INTO [dbo].[Stocks] ([Id], [Symbol], [Name], [Price], [CompanyId])
    VALUES (@Id, @Symbol, @Name, @Price, @CompanyId)
END

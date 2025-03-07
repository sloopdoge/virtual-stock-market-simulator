CREATE PROCEDURE [dbo].[sp_Stocks_UpdateMany] @Stocks NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF ISJSON(@Stocks) = 0
        RETURN 0;

    DECLARE @StockTable TABLE
                        (
                            Id        INT PRIMARY KEY,
                            Symbol    NVARCHAR(50),
                            Name      NVARCHAR(255),
                            Price     DECIMAL(18, 2),
                            CompanyId UNIQUEIDENTIFIER
                        );

    INSERT INTO @StockTable (Id, Symbol, Name, Price, CompanyId)
    SELECT [Id],
           [Symbol],
           [Name],
           [Price],
           [CompanyId]
    FROM OPENJSON(@Stocks)
                  WITH (
                      Id INT,
                      Symbol NVARCHAR(50),
                      Name NVARCHAR(255),
                      Price DECIMAL(18, 2),
                      CompanyId UNIQUEIDENTIFIER
                      );

    IF EXISTS (SELECT 1 FROM @StockTable)
        BEGIN
            UPDATE s
            SET s.Symbol    = st.Symbol,
                s.Name      = st.Name,
                s.Price     = st.Price,
                s.CompanyId = st.CompanyId
            FROM [dbo].[Stocks] s
                     INNER JOIN @StockTable st ON s.Id = st.Id;
        END

    RETURN 1;
END;
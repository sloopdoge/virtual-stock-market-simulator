CREATE PROCEDURE [dbo].[sp_Stocks_UpdatePriceForMany] @Stocks NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF ISJSON(@Stocks) = 0
        RETURN;

    DECLARE @StockTable TABLE
                        (
                            Id    INT PRIMARY KEY,
                            Price DECIMAL(18, 2)
                        );

    INSERT INTO @StockTable (Id, Price)
    SELECT [Id],
           [Price]
    FROM OPENJSON(@Stocks)
                  WITH (
                      Id INT,
                      Price DECIMAL(18, 2)
                      );

    IF EXISTS (SELECT 1 FROM @StockTable)
        BEGIN
            UPDATE s
            SET s.Price = st.Price
            FROM [dbo].[Stocks] s
                     INNER JOIN @StockTable st ON s.Id = st.Id;
        END
END;
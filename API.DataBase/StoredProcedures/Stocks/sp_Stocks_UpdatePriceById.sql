CREATE PROCEDURE [dbo].[sp_Stocks_UpdatePriceById] @Id INT,
                                                   @Price DECIMAL(38, 10)
AS
BEGIN
    UPDATE [dbo].[Stocks]
    SET [Price] = @Price
    WHERE [Id] = @Id
END

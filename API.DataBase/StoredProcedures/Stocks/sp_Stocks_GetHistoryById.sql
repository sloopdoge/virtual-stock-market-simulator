CREATE PROCEDURE [dbo].[sp_Stocks_GetHistoryById] @Id INT,
                                                  @StartDate DATETIME2,
                                                  @EndDate DATETIME2
AS
BEGIN
    SELECT SH.[Id],
           SH.[Symbol],
           SH.[Name],
           SH.[Price],
           SH.[CompanyId],
           SH.[CreatedAt],
           SH.[UpdatedAt],
           SH.[LastUpdate]
    FROM [dbo].[StocksHistory] SH
    WHERE SH.[Id] = @Id
      AND SH.UpdatedAt BETWEEN @StartDate AND @EndDate
    ORDER BY SH.UpdatedAt
END

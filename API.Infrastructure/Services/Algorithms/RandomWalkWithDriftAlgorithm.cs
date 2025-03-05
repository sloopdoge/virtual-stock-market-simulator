using API.Domain.Enums;
using API.Infrastructure.Interfaces;
using API.Infrastructure.Interfaces.Algorithms;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services.Algorithms;

public class RandomWalkWithDriftAlgorithm(
    ILogger<RandomWalkWithDriftAlgorithm> logger) 
    : IRandomWalkWithDriftAlgorithm
{
    private static readonly Random Random = new();

    public decimal PredictPrice(decimal currentPrice)
    {
        try
        {
            var trend = GetRandomTrend(); // Get a random market trend

            decimal marketDrift;
            decimal volatility;

            switch (trend)
            {
                case StockTrendEnum.Bullish:
                    marketDrift = (decimal)Random.NextDouble() * 0.01m;  // 0% to 1% positive drift
                    volatility = (decimal)Random.NextDouble() * 0.02m;   // 0% to 2% fluctuation
                    break;

                case StockTrendEnum.Bearish:
                    marketDrift = (decimal)-Random.NextDouble() * 0.01m; // -1% to 0% drift
                    volatility = (decimal)Random.NextDouble() * 0.03m;   // 0% to 3% fluctuation
                    break;

                case StockTrendEnum.Volatile:
                    marketDrift = 0;                                     // No clear trend
                    volatility = (decimal)Random.NextDouble() * 0.05m;   // 0% to 5% high fluctuation
                    break;

                default: // Neutral
                    marketDrift = 0;
                    volatility = (decimal)Random.NextDouble() * 0.01m;   // 0% to 1% fluctuation
                    break;
            }

            var randomFactor = (2 * Random.NextDouble() - 1); // -1 to +1

            var changeFactor = marketDrift + volatility * (decimal)randomFactor;
            var newPrice = Math.Round(currentPrice * (1 + changeFactor), 2);

            logger.LogInformation($"Trend: {trend}, Change: {changeFactor:P}, New Price: {newPrice}");
            return newPrice;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    private static StockTrendEnum GetRandomTrend()
    {
        var values = Enum.GetValues<StockAlgorithmTypeEnum>();
        return (StockTrendEnum)values.GetValue(Random.Next(values.Length))!;
    }
}
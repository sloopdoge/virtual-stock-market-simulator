using API.Domain.Entities;
using API.Domain.Enums;
using API.Infrastructure.Interfaces.Algorithms;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services.Algorithms;

public class RandomWalkWithDriftAlgorithm(
    ILogger<RandomWalkWithDriftAlgorithm> logger) 
    : IAlgorithm
{
    private static readonly Random Random = new();

    public Task<Stock> PredictPrice(Stock stock)
    {
        try
        {
            var trend = GetRandomTrend();

            decimal marketDrift;
            decimal volatility;

            switch (trend)
            {
                case StockTrendEnum.Bullish:
                    marketDrift = (decimal)Random.NextDouble() * 0.005m;
                    volatility = (decimal)Random.NextDouble() * 0.015m;
                    break;
                case StockTrendEnum.Bearish:
                    marketDrift = (decimal)-Random.NextDouble() * 0.005m;
                    volatility = (decimal)Random.NextDouble() * 0.02m;
                    break;
                case StockTrendEnum.Volatile:
                    marketDrift = 0;
                    volatility = (decimal)Random.NextDouble() * 0.03m;
                    break;
                default:
                    marketDrift = 0;
                    volatility = (decimal)Random.NextDouble() * 0.01m;
                    break;
            }

            var randomFactor = (decimal)(2 * Random.NextDouble() - 1);
            var changeFactor = marketDrift + volatility * randomFactor;

            stock.Price = Math.Max(0.01m, Math.Round(stock.Price * (1 + changeFactor), 2));

            return Task.FromResult(stock);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    private static StockTrendEnum GetRandomTrend()
    {
        var values = Enum.GetValues<StockTrendEnum>();
        return (StockTrendEnum)values.GetValue(Random.Next(values.Length))!;
    }
}
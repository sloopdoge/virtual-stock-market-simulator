using API.Domain.Entities;
using API.Infrastructure.Interfaces;
using API.Infrastructure.Interfaces.Algorithms;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services.Algorithms;

public class ExponentialMovingAverageAlgorithm(
    ILogger<ExponentialMovingAverageAlgorithm> logger,
    IStockService stockService)
    : IAlgorithm
{
    private const int EmaPeriod = 30;
    private const decimal SmoothingFactor = 2m / (EmaPeriod + 1);
    private static readonly Random Random = new();

    public async Task<Stock> PredictPrice(Stock stock)
    {
        try
        {
            var history = await stockService.GetHistoryById(stock.Id, DateTime.UtcNow.AddDays(-EmaPeriod));

            if (history.Count < 2)
                return stock;

            var ema = CalculateExponentialMovingAverage(history);
            var previousPrice = history.Last().Price;

            var emaSlope = ema - previousPrice;
            var trendFactor = emaSlope > 0 ? 1.01m : 0.99m;

            var randomNoise = (decimal)(Random.NextDouble() - 0.5) * 0.005m;

            var changeFactor = Math.Clamp((emaSlope * 0.01m) + randomNoise, -0.03m, 0.03m);

            stock.Price = Math.Max(0.01m, Math.Round(stock.Price * (1 + changeFactor) * trendFactor, 2));

            return stock;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    private static decimal CalculateExponentialMovingAverage(List<Stock> history)
    {
        var ema = history.First().Price;

        return history
            .Skip(1)
            .Aggregate(ema, (current, stock) => 
                (stock.Price - current) * SmoothingFactor + current);
    }
}



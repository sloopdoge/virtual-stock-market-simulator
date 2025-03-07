using API.Domain.Entities;
using API.Domain.Enums;
using API.Infrastructure.Interfaces;
using API.Infrastructure.Interfaces.Algorithms;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services.Algorithms;

public class MeanReversionAlgorithm(
    ILogger<MeanReversionAlgorithm> logger,
    IStockService stockService) : IAlgorithm
{
    private static readonly Random Random = new();
    
    private const int HistoryDays = 5;
    
    public async Task<Stock> PredictPrice(Stock stock)
    {
        try
        {
            var history = await stockService.GetHistoryById(stock.Id, DateTime.UtcNow.AddDays(-HistoryDays));
            if (history.Count == 0) 
                return stock;

            var prices = new Queue<decimal>(history.Select(s => s.Price));

            var meanPrice = CalculateMeanPrice(prices);
            var reversionSpeed = CalculateReversionSpeed(prices);
            var volatility = CalculateVolatility(prices);

            var meanReversionFactor = reversionSpeed * (meanPrice - stock.Price);
            var randomFactor = (decimal)(Random.NextDouble() * 2 - 1);

            var changeFactor = Math.Clamp(meanReversionFactor + volatility * randomFactor, -0.03m, 0.03m);

            stock.Price = Math.Max(0.01m, Math.Round(stock.Price * (1 + changeFactor), 2));

            return stock;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
    
    private static decimal CalculateMeanPrice(Queue<decimal> prices) => prices.Average();

    private static decimal CalculateReversionSpeed(Queue<decimal> prices)
    {
        var mean = prices.Average();
        var daysOutOfMean = prices.Count(p => Math.Abs(p - mean) > mean * 0.05m);
        return daysOutOfMean > 0 ? 1m / daysOutOfMean : 0.05m;
    }

    private static decimal CalculateVolatility(Queue<decimal> prices)
    {
        var mean = prices.Average();
        var variance = prices.Average(p => (p - mean) * (p - mean));
        return Math.Max(0.01m, (decimal)Math.Sqrt((double)variance));
    }
}

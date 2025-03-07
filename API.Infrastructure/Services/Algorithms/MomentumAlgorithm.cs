using API.Domain.Entities;
using API.Infrastructure.Interfaces;
using API.Infrastructure.Interfaces.Algorithms;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services.Algorithms;

public class MomentumAlgorithm(
    ILogger<MomentumAlgorithm> logger,
    IStockService stockService)
    : IAlgorithm
{
    private const int MomentumWindow = 30;
    private static readonly Random Random = new();

    public async Task<Stock> PredictPrice(Stock stock)
    {
        try
        {
            var history = await stockService.GetHistoryById(stock.Id, DateTime.UtcNow.AddDays(-MomentumWindow));

            if (history.Count < 2)
                return stock;

            var momentumFactor = Math.Clamp(CalculateMomentum(history), -0.05m, 0.05m);

            var randomNoise = (decimal)(Random.NextDouble() - 0.5) * 0.005m;

            var changeFactor = momentumFactor + randomNoise;
            stock.Price = Math.Max(0.01m, Math.Round(stock.Price * (1 + changeFactor), 2));

            return stock;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    private static decimal CalculateMomentum(List<Stock> history)
    {
        var firstPrice = history.First().Price;
        var lastPrice = history.Last().Price;

        return (lastPrice - firstPrice) / firstPrice;
    }
}


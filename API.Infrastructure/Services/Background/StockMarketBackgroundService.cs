using API.Domain.Enums;
using API.Infrastructure.Interfaces;
using API.Infrastructure.Interfaces.Algorithms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services.Background;

public class StockMarketBackgroundService(
    ILogger<StockMarketBackgroundService> logger,
    IServiceScopeFactory serviceScopeFactory) 
    : BackgroundService
{
    private static readonly Random Random = new();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var stocks = await stockService.GetAll();
                    foreach (var stock in stocks)
                    {
                        stock.Price = SimulatePriceChange(stock.Price);
                        await stockService.Update(stock);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    private decimal SimulatePriceChange(decimal stockPrice)
    {
        var newStockPrice = stockPrice;
        
        try
        {
            var algorithmTypes = Enum.GetValues<StockAlgorithmTypeEnum>();

            // switch ((StockAlgorithmTypeEnum)algorithmTypes.GetValue(Random.Next(algorithmTypes.Length))!)
            switch (StockAlgorithmTypeEnum.RandomWalkWithDrift)
            {
                case StockAlgorithmTypeEnum.RandomWalkWithDrift:
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var algorithmService = scope.ServiceProvider.GetRequiredService<IRandomWalkWithDriftAlgorithm>();

                    newStockPrice = algorithmService.PredictPrice(stockPrice);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
        
        return newStockPrice;
    }
}
using API.Domain.Entities;
using API.Domain.Enums;
using API.Infrastructure.Interfaces;
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
        await Task.Delay(10_000, stoppingToken);
        
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();
            var algorithmFactory = scope.ServiceProvider.GetRequiredService<IAlgorithmFactory>();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogInformation("=== Starting Stock Market changes ===");
                    var stocks = await stockService.GetAll();
                    var updatedStocks = new List<Stock>();
                    foreach (var stock in stocks)
                    {
                        var updatedStock = await SimulatePriceChange(stock, algorithmFactory);
                        updatedStocks.Add(updatedStock);
                        logger.LogInformation($"Stock: {stock.Symbol} | New price: {stock.Price}");
                    }
                    
                    var res = await stockService.UpdatePriceForMany(updatedStocks);
                    if (!res)
                        logger.LogError($"There was error updating stocks values in database");
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                }
                finally
                {
                    logger.LogInformation("=== Finished Stock Market changes ===");
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    private async Task<Stock> SimulatePriceChange(Stock stock, IAlgorithmFactory algorithmFactory)
    {
        try
        {
            var algorithmType = (StockAlgorithmTypeEnum)Enum.GetValues<StockAlgorithmTypeEnum>()
                .GetValue(Random.Next(Enum.GetValues<StockAlgorithmTypeEnum>().Length))!;

            var algorithm = algorithmFactory.GetAlgorithm(algorithmType);
            
            return await algorithm.PredictPrice(stock);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return stock;
        }
    }
}
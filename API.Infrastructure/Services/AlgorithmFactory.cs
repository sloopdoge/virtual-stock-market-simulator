using API.Domain.Enums;
using API.Infrastructure.Interfaces;
using API.Infrastructure.Interfaces.Algorithms;
using API.Infrastructure.Services.Algorithms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services;

public class AlgorithmFactory(
    ILogger<AlgorithmFactory> logger, 
    IServiceProvider serviceProvider) : IAlgorithmFactory
{
    public IAlgorithm GetAlgorithm(StockAlgorithmTypeEnum algorithmType)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            return algorithmType switch
            {
                StockAlgorithmTypeEnum.RandomWalkWithDrift => provider.GetRequiredService<RandomWalkWithDriftAlgorithm>(),
                StockAlgorithmTypeEnum.MeanReversion => provider.GetRequiredService<MeanReversionAlgorithm>(),
                StockAlgorithmTypeEnum.Momentum => provider.GetRequiredService<MomentumAlgorithm>(),
                StockAlgorithmTypeEnum.ExponentialMovingAverage => provider.GetRequiredService<ExponentialMovingAverageAlgorithm>(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}

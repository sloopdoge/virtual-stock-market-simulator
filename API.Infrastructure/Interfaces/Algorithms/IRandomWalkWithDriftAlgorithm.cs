namespace API.Infrastructure.Interfaces.Algorithms;

public interface IRandomWalkWithDriftAlgorithm
{
    decimal PredictPrice(decimal currentPrice);
}
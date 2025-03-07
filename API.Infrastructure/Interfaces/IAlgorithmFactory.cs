using API.Domain.Enums;
using API.Infrastructure.Interfaces.Algorithms;

namespace API.Infrastructure.Interfaces;

public interface IAlgorithmFactory
{
    IAlgorithm GetAlgorithm(StockAlgorithmTypeEnum algorithmType);
}
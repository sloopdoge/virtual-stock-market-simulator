using API.Domain.Entities;

namespace API.Infrastructure.Interfaces.Algorithms
{
    public interface IAlgorithm
    {
        Task<Stock> PredictPrice(Stock stock);
    }
}
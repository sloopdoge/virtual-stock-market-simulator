using API.Domain.Entities;

namespace API.Infrastructure.Interfaces;

public interface IStockService
{
    Task<Stock?> GetById(long stockId);
    Task<List<Stock>> GetHistoryById(long stockId, DateTime startDate, DateTime? endDate = null);
    Task<List<Stock>> GetAll();
    Task<Stock?> Create(Stock stock);
    Task<Stock?> Update(Stock stock);
    Task<bool> Delete(long stockId);
    Task<bool> UpdateMany(List<Stock> updatedStocks);
    Task<bool> UpdatePriceById(Stock stock);
    Task<bool> UpdatePriceForMany(List<Stock> stocks);
}
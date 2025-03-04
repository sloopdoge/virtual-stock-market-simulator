using API.Domain.Entities;

namespace API.Infrastructure.Interfaces;

public interface IStockService
{
    Task<Stock?> GetById(long stockId);
    Task<List<Stock>> GetAll();
    Task<Stock?> Create(Stock stock);
    Task<Stock?> Update(Stock stock);
    Task<bool> Delete(Guid stockId);
}
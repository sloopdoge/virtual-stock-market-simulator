using API.Domain.Entities;
using API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services;

public class StockService(
    ILogger<StockService> logger) 
    : IStockService
{
    public async Task<Stock?> GetById(long stockId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Stock>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Stock?> Create(Stock stock)
    {
        throw new NotImplementedException();
    }

    public async Task<Stock?> Update(Stock stock)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Delete(Guid stockId)
    {
        throw new NotImplementedException();
    }
}
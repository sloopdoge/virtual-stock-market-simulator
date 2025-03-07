using API.Domain.Entities;
using API.Infrastructure.Interfaces;
using API.Repository;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services;

public class StockService(
    ILogger<StockService> logger,
    StockDbRepository stockDbRepository) 
    : IStockService
{
    public async Task<Stock?> GetById(long stockId)
    {
        try
        {
            return await stockDbRepository.GetById(stockId);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<Stock>> GetHistoryById(long stockId, DateTime startDate, DateTime? endDate = null)
    {
        try
        {
            return await stockDbRepository.GetHistoryById(stockId, startDate, endDate);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<Stock>> GetAll()
    {
        try
        {
            return await stockDbRepository.GetAll();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<Stock?> Create(Stock stock)
    {
        try
        {
            stock.CreatedAt = DateTime.UtcNow;
            
            return await stockDbRepository.Create(stock);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<Stock?> Update(Stock stock)
    {
        try
        {
            return await stockDbRepository.Update(stock);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> Delete(long stockId)
    {
        try
        {
            return await stockDbRepository.Delete(stockId);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateMany(List<Stock> stocks)
    {
        try
        {
            if (stocks.Count == 0)
                return false;
            
            return await stockDbRepository.UpdateMany(stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }
    
    public async Task<bool> UpdatePriceById(Stock stock)
    {
        try
        {
            return await stockDbRepository.UpdatePriceById(stock);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdatePriceForMany(List<Stock> stocks)
    {
        try
        {
            return await stockDbRepository.UpdatePriceForMany(stocks);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<List<UserStock>> GetStocksByUserId(Guid userId)
    {
        try
        {
            return await stockDbRepository.GetStocksByUserId(userId);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return [];
        }
    }
}
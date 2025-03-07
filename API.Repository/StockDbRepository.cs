using API.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace API.Repository;

public class StockDbRepository(IConfiguration configuration) : BaseDbRepository(configuration, "Stocks")
{
    public Task<Stock> GetById(long stockId)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", stockId);
        
        return ExecuteQueryWithSingleReturn<Stock>("GetById", obj);
    }

    public Task<List<Stock>> GetHistoryById(long stockId, DateTime startDate, DateTime? endDate = null)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", stockId);
        obj.Add("@StartDate", startDate);
        obj.Add("@EndDate", endDate ?? DateTime.UtcNow);

        return ExecuteQueryWithListReturn<Stock>("GetHistoryById", obj);
    }

    public Task<List<Stock>> GetAll()
    {
        var obj = new DynamicParameters();
        
        return ExecuteQueryWithListReturn<Stock>("GetAll");
    }

    public Task<Stock> Create(Stock model)
    {
        var obj = new DynamicParameters();
        obj.Add("@Symbol", model.Symbol);
        obj.Add("@Name", model.Name);
        obj.Add("@Price", model.Price);
        obj.Add("@CompanyId", model.CompanyId);
        obj.Add("@CreatedAt", model.CreatedAt);
        
        return ExecuteQueryWithSingleReturn<Stock>("Create", obj);
    }

    public Task<Stock> Update(Stock model)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", model.Id);
        obj.Add("@Symbol", model.Symbol);
        obj.Add("@Name", model.Name);
        obj.Add("@Price", model.Price);
        obj.Add("@CompanyId", model.CompanyId);
        
        return ExecuteQueryWithSingleReturn<Stock>("Update", obj);
    }

    public Task<bool> Delete(long stockId)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", stockId);
        
        return ExecuteQuery(obj, "Delete");
    }

    public Task<bool> UpdateMany(List<Stock> updatedStocks)
    {
        var obj = new DynamicParameters();
        var json = JsonConvert.SerializeObject(updatedStocks);
        obj.Add("@Stocks", json);
        
        return ExecuteQuery(obj, "UpdateMany");
    }

    public Task<bool> UpdatePriceById(Stock stock)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", stock.Id);
        obj.Add("@Price", stock.Price);
        
        return ExecuteQuery(obj, "UpdatePriceById");
    }

    public Task<bool> UpdatePriceForMany(List<Stock> stocks)
    {
        var obj = new DynamicParameters();
        var json = JsonConvert.SerializeObject(stocks);
        obj.Add("@Stocks", json);
        
        return ExecuteQuery(obj, "UpdatePriceForMany");
    }

    public Task<List<UserStock>> GetStocksByUserId(Guid userId)
    {
        var obj = new DynamicParameters();
        obj.Add("@UserId", userId);
        
        return ExecuteQueryWithListReturn<UserStock>("GetStocksByUserId", obj);
    }
}
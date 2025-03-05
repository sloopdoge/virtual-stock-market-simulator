using API.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;

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
        if (endDate.HasValue) obj.Add("@EndDate", endDate.Value);

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
}
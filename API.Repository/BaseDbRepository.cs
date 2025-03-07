using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace API.Repository;

public class BaseDbRepository
{
    private IConfiguration _configuration;
    private string _tableName;
    protected string ConnectionString => _configuration.GetConnectionString("DefaultConnection");

    protected BaseDbRepository(IConfiguration configuration, string tableName)
    {
        _configuration = configuration;
        _tableName = tableName;
    }
    
    protected SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }
    
    protected async Task<SqlConnection> GetConnectionAsync()
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
    
    protected string StoredProcedure(string storedProcedureEnding)
    {
        return $"sp_{_tableName}_{storedProcedureEnding}";
    }
    
    protected async Task<bool> ExecuteQuery(DynamicParameters parameters, string storedProcedureName, bool customName = false, SqlTransaction? transaction = null)
    {
        var storedProcedure = customName ? storedProcedureName : StoredProcedure(storedProcedureName);

        if (transaction != null)
        {
            var rows = await transaction.Connection.ExecuteAsync(storedProcedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return rows != 0;
        }

        using (var connection = await GetConnectionAsync())
        {
            var rows = await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return rows != 0;
        }
    }
    
    protected async Task<List<TModel>> ExecuteQueryWithListReturn<TModel>(string storedProcedureName, DynamicParameters? parameters = null, bool customName = false, SqlTransaction? transaction = null) where TModel : class
    {
        var storedProcedure = customName ? storedProcedureName : StoredProcedure(storedProcedureName);

        if (transaction != null)
        {               
            var rows = await transaction.Connection.QueryAsync<TModel>(storedProcedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }

        using (var connection = await GetConnectionAsync())
        {
            var rows = await connection.QueryAsync<TModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }
    }
    
    protected async Task<TModel> ExecuteQueryWithSingleReturn<TModel>(string storedProcedureName, DynamicParameters? parameters = null, bool customName = false, SqlTransaction? transaction = null)
    {
        var storedProcedure = customName ? storedProcedureName : StoredProcedure(storedProcedureName);

        if (transaction != null)
        {
            var row = await transaction.Connection.QueryFirstOrDefaultAsync<TModel>(storedProcedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            return row;
        }

        using (var connection = await GetConnectionAsync())
        {
            var row = await connection.QueryFirstOrDefaultAsync<TModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return row;
        }
    }
    
    protected async Task<TModel> ExecuteQueryWithSingleJsonReturn<TModel>(string storedProcedureName, DynamicParameters? parameters = null, bool customName = false, SqlTransaction? transaction = null)
    {
        var storedProcedure = customName ? storedProcedureName : StoredProcedure(storedProcedureName);
        
        if (transaction != null)
        {
            var rows = await transaction.Connection.QueryAsync<string>(storedProcedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            var tempResult = string.Empty;

            foreach (var item in rows)
                tempResult = tempResult + item;

            var res = JsonConvert.DeserializeObject<List<TModel>>(tempResult);
            return res != null ? res.FirstOrDefault() : default(TModel);
        }

        using (var connection = await GetConnectionAsync())
        {
            var rows = await connection.QueryAsync<string>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            var tempResult = string.Empty;

            foreach (var item in rows)
                tempResult = tempResult + item;

            var res = JsonConvert.DeserializeObject<List<TModel>>(tempResult);
            return res != null ? res.FirstOrDefault() : default(TModel);
        }
    }

    protected async Task<List<TModel>> ExecuteQueryWithListJsonReturn<TModel>(string storedProcedureName, DynamicParameters? parameters = null, bool customName = false, SqlTransaction? transaction = null)
    {
        var storedProcedure = customName ? storedProcedureName : StoredProcedure(storedProcedureName);

        if (transaction != null)
        {
            var rows = await transaction.Connection.QueryAsync<string>(storedProcedure, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
            var tempResult = string.Empty;

            foreach (var item in rows)
                tempResult = tempResult + item;

            var res = JsonConvert.DeserializeObject<IEnumerable<TModel>>(tempResult);
            return res.ToList();
        }

        using (var connection = await GetConnectionAsync())
        {
            var rows = await connection.QueryAsync<string>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            var tempResult = string.Empty;

            foreach (var item in rows)
                tempResult = tempResult + item;

            var res = JsonConvert.DeserializeObject<IEnumerable<TModel>>(tempResult);
            return res.ToList();
        }
    }
}
using API.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace API.Repository;

public class CompanyDbRepository(IConfiguration configuration) : BaseDbRepository(configuration, "Companies")
{
    public Task<Company> GetById(Guid companyId)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", companyId);
        
        return Get<Company>("GetById", obj);
    }

    public Task<List<Company>> GetAll()
    {
        var obj = new DynamicParameters();
        
        return GetMany<Company>("GetAll");
    }

    public Task<bool> IsExist(Guid companyId)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", companyId);
        
        return ExecuteQuery(obj, "IsExist");
    }

    public Task<bool> Create(Company model)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", model.Id);
        obj.Add("@Name", model.Name);
        obj.Add("@Description", model.Description);
        obj.Add("@UpdatedAt", model.UpdatedAt);
        obj.Add("@CreatedAt", model.CreatedAt);
        
        return ExecuteQuery(obj, "Create");
    }

    public Task<bool> Update(Company model)
    {
        var obj = new DynamicParameters();
        obj.Add("@Id", model.Id);
        obj.Add("@Name", model.Name);
        obj.Add("@Description", model.Description);
        obj.Add("@UpdatedAt", model.UpdatedAt);
        obj.Add("@CreatedAt", model.CreatedAt);
        
        return ExecuteQuery(obj, "Update");
    }
}
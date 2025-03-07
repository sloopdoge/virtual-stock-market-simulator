using API.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace API.Repository;

public class UserProfilesDbRepository(IConfiguration configuration) : BaseDbRepository(configuration, "UserProfiles")
{
    public Task<UserProfile> GetById(Guid userId)
    {
        var obj = new DynamicParameters();
        obj.Add("@UserId", userId);
        
        return ExecuteQueryWithSingleReturn<UserProfile>("Delete", obj); //TODO db side
    }

    public Task<UserProfile> Create(UserProfile profile)
    {
        var obj = new DynamicParameters();
        obj.Add("@UserId", profile.UserId);
        obj.Add("@WalletBalance", profile.WalletBalance);
        obj.Add("@TotalMoneySpent", profile.TotalMoneySpent);
        obj.Add("@TotalMoneyEarned", profile.TotalMoneyEarned);
        obj.Add("@CreatedAt", profile.CreatedAt);
        obj.Add("@UpdatedAt", profile.UpdatedAt);
        var stocksIds = JsonConvert.SerializeObject(profile.UserStocks.Select(us => us.Stock.Id));
        obj.Add("@UserStocks", stocksIds);
        
        return ExecuteQueryWithSingleReturn<UserProfile>("Create", obj); //TODO db side
    }

    public Task<UserProfile> Update(UserProfile profile)
    {
        var obj = new DynamicParameters();
        obj.Add("@UserId", profile.UserId);
        obj.Add("@WalletBalance", profile.WalletBalance);
        obj.Add("@TotalMoneySpent", profile.TotalMoneySpent);
        obj.Add("@TotalMoneyEarned", profile.TotalMoneyEarned);
        obj.Add("@CreatedAt", profile.CreatedAt);
        obj.Add("@UpdatedAt", profile.UpdatedAt);
        var stocksIds = JsonConvert.SerializeObject(profile.UserStocks.Select(us => us.Stock.Id));
        obj.Add("@UserStocks", stocksIds);
        
        return ExecuteQueryWithSingleReturn<UserProfile>("Update", obj); //TODO db side
    }
}
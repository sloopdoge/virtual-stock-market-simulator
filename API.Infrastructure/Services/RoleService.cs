using API.Identity.Entities;
using API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services;

public class RoleService(
    ILogger<UserService> logger, 
    RoleManager<ApplicationRole> roleManager) 
    : IRoleService
{
    public async Task<ApplicationRole?> GetById(Guid roleId)
    {
        try
        {
            return await roleManager.FindByIdAsync(roleId.ToString());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }
}
using API.Domain.Entities;
using API.Domain.Models;
using API.Identity.Entities;
using API.Identity.Repositories;
using API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Services;

public class UserService(
    ILogger<UserService> logger,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IUserService
{
    public async Task<ApplicationUser?> GetById(Guid userId)
    {
        try
        {
            return await userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<ApplicationUser?> GetByEmail(string email)
    {
        try
        {
            return await userManager.Users
                .FirstOrDefaultAsync(u =>  string.Equals(u.NormalizedEmail, email.ToUpperInvariant()));
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<ApplicationUser?> GetByUsername(string username)
    {
        try
        {
            return await userManager.Users
                .FirstOrDefaultAsync(u => string.Equals(u.NormalizedUserName, username.ToUpperInvariant()));
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<ApplicationUser>> GetAll()
    {
        try
        {
            return await userManager.Users.ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<ApplicationUser?> Create(ApplicationUser user)
    {
        try
        {
            var id = Guid.NewGuid();
            user.Id = id;
            
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
                return null;
                
            return await userManager.FindByIdAsync(id.ToString());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<ApplicationUser?> Update(ApplicationUser user)
    {
        try
        {
            var saveRes = await userManager.UpdateAsync(user);
            if (!saveRes.Succeeded)
                return null;
            
            return await userManager.FindByIdAsync(user.Id.ToString());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<bool> Delete(Guid userId)
    {
        try
        {
            var dbUser = await GetById(userId);
            if (dbUser == null)
                return false;

            var deleteRes = await userManager.DeleteAsync(dbUser);
            
            return deleteRes.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateRole(Guid userId, string roleName)
    {
        try
        {
            var dbUser = await GetById(userId);
            if (dbUser == null)
                return false;

            if (await userManager.IsInRoleAsync(dbUser, roleName))
            {
                logger.LogInformation($"User {userId} is already in role {roleName}");

                return false;
            }
            
            var userRoles = await userManager.GetRolesAsync(dbUser);
            var userRoleAddRes = await userManager.AddToRoleAsync(dbUser, roleName);
            if (!userRoleAddRes.Succeeded)
                return false;

            foreach (var userRole in userRoles)
            {
                await userManager.RemoveFromRoleAsync(dbUser, userRole);
            }

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<ApplicationRole?> GetRoleByUserId(Guid userId)
    {
        try
        {
            var dbUser = await GetById(userId);
            if (dbUser == null)
                throw new Exception($"{nameof(GetRoleByUserId)} User not found");
            
            var userRoleName = (await userManager.GetRolesAsync(dbUser)).FirstOrDefault();
            if (userRoleName == null || string.IsNullOrEmpty(userRoleName))
                throw new Exception($"{nameof(GetRoleByUserId)} User without role");

            var role = await roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == userRoleName);
            return role;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }
    
    public async Task<bool> CreateFromModel(UserModel user)
    {
        try
        {
            var newId = Guid.NewGuid();
            
            var result = await userManager.CreateAsync(new AppUser()
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                RegistrationDate = user.RegistrationDate,
            });
            
            return result.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateFromModel(UserModel user)
    {
        try
        {
            var dbUser = await userManager.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (dbUser == null)
                return false;
            
            ((AppUser)dbUser).Update(user);
            
            var saveRes = await userManager.UpdateAsync(dbUser);
            
            return saveRes.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return false;
        }
    }

    public async Task<UserModel?> GetModelById(Guid userId)
    {
        try
        {
            var user = await GetById(userId);

            return user == null ? null : new UserModel(user);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<UserModel?> GetModelByEmail(string email)
    {
        try
        {
            var user = await GetByEmail(email);

            return user == null ? null : new UserModel(user);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<UserModel?> GetModelByUsername(string username)
    {
        try
        {
            var user = await GetByUsername(username);

            return user == null ? null : new UserModel(user);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return null;
        }
    }

    public async Task<List<ApplicationUser>> GetAllModels()
    {
        try
        {
            return await userManager.Users
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return [];
        }
    }

    public async Task<List<ApplicationUser>> GetAllInRole(string roleName)
    {
        try
        {
            var dbUsers = await userManager.GetUsersInRoleAsync(roleName);

            return dbUsers.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return [];
        }
    }
}
using API.Identity.Entities;

namespace API.Infrastructure.Interfaces;

public interface IUserService
{
    Task<ApplicationUser?> GetById(Guid userId);
    Task<ApplicationUser?> GetByEmail(string email);
    Task<ApplicationUser?> GetByUsername(string username);
    Task<List<ApplicationUser>> GetAll();
    Task<ApplicationUser?> Create(ApplicationUser user);
    Task<ApplicationUser?> Update(ApplicationUser user);
    Task<bool> Delete(Guid userId);
    Task<bool> UpdateRole(Guid userId, string roleName);
    Task<ApplicationRole?> GetRoleByUserId(Guid userId);
    
    Task<List<ApplicationUser>> GetAllModels();
    Task<List<ApplicationUser>> GetAllInRole(string roleName);
}
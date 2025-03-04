using API.Identity.Entities;

namespace API.Infrastructure.Interfaces;

public interface IRoleService
{
    Task<ApplicationRole?> GetById(Guid roleId);
}
using API.Identity.Entities;
using API.Identity.Models;

namespace API.Identity.Interfaces;

public interface IJwtTokenService
{
    TokenModel GenerateToken(ApplicationUser user, ApplicationRole role, bool rememberMe);
}
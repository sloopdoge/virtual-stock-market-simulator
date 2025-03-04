using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Identity.Entities;
using API.Identity.Interfaces;
using API.Identity.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace API.Identity.Services;

public class JwtTokenService(
    ILogger<JwtTokenService> logger, 
    IConfiguration configuration)
    : IJwtTokenService
{
    public TokenModel GenerateToken(ApplicationUser user, ApplicationRole role, bool rememberMe)
    {
        try
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var expiresIn = jwtSettings.GetValue<int>("ExpiresInMinutes");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, role.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetValue<string>("Issuer"),
                audience: jwtSettings.GetValue<string>("Audience"),
                claims: claims,
                expires: rememberMe 
                    ? DateTime.UtcNow.AddMinutes(expiresIn) 
                    : DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new TokenModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = rememberMe 
                    ? DateTime.UtcNow.AddMinutes(expiresIn) 
                    : DateTime.UtcNow.AddDays(1)
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
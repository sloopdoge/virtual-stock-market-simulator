using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.Hubs;

[Authorize]
public class StocksHub(
    ILogger<StocksHub> logger) 
    : BaseHub
{
    public override Task OnConnectedAsync()
    {
        logger.LogDebug($"User connected. Connection Id: {GetConnectionId()}. UserId: {GetUserId()}");
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogDebug($"User disconnected. Connection Id: {GetConnectionId()}. User: {GetClaimValue(JwtRegisteredClaimNames.Email)} {GetUserId()}");
        return Task.CompletedTask;
    }
}
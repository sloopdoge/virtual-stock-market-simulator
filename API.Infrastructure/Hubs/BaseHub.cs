using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace API.Infrastructure.Hubs;

public class BaseHub : Hub
{
    protected string GetConnectionId()
    {
        return Context.ConnectionId;
    }
    
    protected string? GetUserId()
    {
        return GetClaimValue(ClaimTypes.NameIdentifier);
    }

    protected string? GetClaimValue(string claimType)
    {
        return Context.User?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
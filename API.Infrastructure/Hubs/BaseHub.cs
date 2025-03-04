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
    
    private string? GetClaimValue(string claimType)
    {
        var claim = Context.User?.Claims.FirstOrDefault(c => c.Type == claimType);
        return claim?.Value;
    }
}
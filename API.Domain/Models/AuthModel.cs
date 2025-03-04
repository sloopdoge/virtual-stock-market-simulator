using API.Identity.Models;

namespace API.Domain.Models;

public class AuthModel
{
    public bool Succeeded { get; set; } = false;
    public Guid UserId { get; set; }
    public string RoleName { get; set; }
    public string UserFullName { get; set; }
    public string UserEmail { get; set; }
    public TokenModel Token { get; set; }
    public List<string> Errors { get; set; }
}
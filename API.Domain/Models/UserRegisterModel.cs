namespace API.Domain.Models;

public class UserRegisterModel
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    
    public string Password { get; set; }
    public RoleModel Role { get; set; }
}

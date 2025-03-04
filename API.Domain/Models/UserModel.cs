using API.Identity.Entities;

namespace API.Domain.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegistrationDate { get; set; }

    public UserModel() { }

    public UserModel(ApplicationUser user)
    {
        Id = user.Id;
        UserName = user.UserName ?? string.Empty;
        Email = user.Email ?? string.Empty;
        PhoneNumber = user.PhoneNumber ?? string.Empty;
        FirstName = user.FirstName;
        LastName = user.LastName;
        BirthDate = user.BirthDate;
        RegistrationDate = user.RegistrationDate;
    }
    
    public UserModel(UserRegisterModel user, Guid userId)
    {
        Id = userId;
        UserName = user.UserName;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        FirstName = user.FirstName;
        LastName = user.LastName;
        BirthDate = user.BirthDate;
        RegistrationDate = DateTime.UtcNow;
    }
}
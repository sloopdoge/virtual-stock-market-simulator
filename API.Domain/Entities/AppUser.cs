using API.Domain.Models;
using API.Identity.Entities;

namespace API.Domain.Entities;

public class AppUser : ApplicationUser
{
    public AppUser() { }

    public AppUser(UserModel userModel)
    {
        UserName = userModel.UserName;
        Email = userModel.Email;
        PhoneNumber = userModel.PhoneNumber;
        FirstName = userModel.FirstName;
        LastName = userModel.LastName;
        BirthDate = userModel.BirthDate;
        RegistrationDate = userModel.RegistrationDate;
    }
    
    public AppUser(UserRegisterModel userModel)
    {
        UserName = userModel.UserName;
        Email = userModel.Email;
        PhoneNumber = userModel.PhoneNumber;
        FirstName = userModel.FirstName;
        LastName = userModel.LastName;
        BirthDate = userModel.BirthDate;
        RegistrationDate = DateTime.UtcNow;
    }

    public void Update(UserModel userModel)
    {
        UserName = userModel.UserName;
        Email = userModel.Email;
        PhoneNumber = userModel.PhoneNumber;
        FirstName = userModel.FirstName;
        LastName = userModel.LastName;
        BirthDate = userModel.BirthDate;
        RegistrationDate = userModel.RegistrationDate;
        UpdateDate = DateTime.UtcNow;
    }
}
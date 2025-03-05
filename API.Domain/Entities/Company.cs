using API.Domain.Models;

namespace API.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Company() { }

    public Company(CreateCompanyModel model)
    {
        Name = model.Name;
        Description = model.Description;
    }

    public bool IsValid()
    {
        if (Id == Guid.Empty)
            return false;
        
        if (string.IsNullOrEmpty(Name))
            return false;
        
        if (string.IsNullOrEmpty(Description))
            return false;
        
        return true;
    }
}
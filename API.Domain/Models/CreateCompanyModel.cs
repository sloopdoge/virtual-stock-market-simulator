namespace API.Domain.Models;

public class CreateCompanyModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Name))
            return false;
        
        if (string.IsNullOrEmpty(Description))
            return false;
        
        return true;
    }
}
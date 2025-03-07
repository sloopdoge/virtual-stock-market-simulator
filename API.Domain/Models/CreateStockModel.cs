namespace API.Domain.Models;

public class CreateStockModel
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Guid CompanyId { get; set; }
    
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Symbol))
            return false;
        if (string.IsNullOrEmpty(Name))
            return false;
        if (Price < 0)
            return false;
        if (CompanyId == Guid.Empty)
            return false;
        
        return true;
    }
}
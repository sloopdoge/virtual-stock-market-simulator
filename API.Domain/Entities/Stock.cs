using API.Domain.Models;

namespace API.Domain.Entities;

public class Stock
{
    public long Id { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public Guid CompanyId { get; set; }

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Symbol))
            return false;
        if (string.IsNullOrEmpty(Name))
            return false;
        if (Price < 0)
            return false;
        
        return true;
    }

    public Stock() { }

    public Stock(CreateStockModel model)
    {
        Symbol = model.Symbol;
        Name = model.Name;
        Price = model.Price;
        CompanyId = model.CompanyId;
    }
}
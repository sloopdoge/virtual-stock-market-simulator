namespace API.Domain.Entities;

public class Stock
{
    public long Id { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public Guid CompanyId { get; set; }
}
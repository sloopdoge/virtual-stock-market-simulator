namespace API.Domain.Entities;

public class UserStock
{
    public Guid UserId { get; set; }
    public Stock Stock { get; set; }
    public long Quantity { get; set; }
    public DateTime UpdatedAt { get; set; }
}
namespace API.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; set; }
    public decimal WalletBalance { get; set; } = 0;
    public decimal TotalMoneySpent { get; set; } = 0;
    public decimal TotalMoneyEarned { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<UserStock> UserStocks { get; set; } = [];
}

namespace wallet.api.Features.DataAccess.Models;

public class UserBalance
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}

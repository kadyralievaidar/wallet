namespace wallet.api.Features.DataAccess.Models;

public class UserBalance : BaseModel
{
    public decimal Balance { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}

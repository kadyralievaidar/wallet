namespace wallet.api.Features.DataAccess.Models;

public class PaymentEntity : BaseModel
{
    public decimal Amount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }


    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid UserBalanceId { get; set; }
    public UserBalance UserBalance { get; set; } = null!;
}
public enum PaymentStatus
{
    Completed, Failed, Pending
}

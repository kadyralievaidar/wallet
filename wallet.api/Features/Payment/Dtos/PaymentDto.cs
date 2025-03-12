using System.Linq.Expressions;

namespace wallet.api.Features.Payment.Dtos;

public class PaymentDto
{
    public Guid UserId {  get; set; } 
    public Guid UserBalanceId { get; set; }
    public decimal Amount { get; set; } 
    public ExpressionType ExpressionType { get; set; }
}

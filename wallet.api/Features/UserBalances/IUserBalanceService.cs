namespace wallet.api.Features.UserBalances;

public interface IUserBalanceService
{
    Task<decimal> SetBalanceAsync(Guid userId);
    Task<decimal> GetBalanceAsync();
    Task<decimal> SubtractAsync();
}

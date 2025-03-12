using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using wallet.api.Features.Core;
using wallet.api.Features.DataAccess;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.Payment;
using wallet.api.Features.Payment.Dtos;

namespace wallet.api.Features.UserBalances;

public class UserBalanceService : IUserBalanceService
{
    private readonly WalletDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    private const decimal StartAmount = 8;
    private const decimal SubstractAmount = 1.1M;

    public UserBalanceService(WalletDbContext dbContext, IPaymentService paymentService, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _paymentService = paymentService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<decimal> SetBalanceAsync(Guid userId)
    {
        using var transaction = _dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        try
        {
            var balance = new UserBalance()
            {
                Balance = StartAmount,
                UserId = userId,
                Id = Guid.NewGuid()
            };
            var payment = new PaymentDto()
            {
                Amount = SubstractAmount,
                UserId = userId,
                UserBalanceId = balance.Id,
                ExpressionType = ExpressionType.Add
            };

            var status = await _paymentService.CreatePayment(payment);

            await _dbContext.UserBalances.AddAsync(balance);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return balance.Balance;

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<decimal> SubtractAsync()
    {
        using var transaction = _dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        try
        {
            var userId = _httpContextAccessor.GetUserId();
            var balance = await _dbContext.UserBalances.FirstOrDefaultAsync(x => x.UserId == userId) ??
                throw new NullReferenceException(nameof(UserBalance));

            var payment = new PaymentDto()
            {
                Amount = SubstractAmount,
                UserId = userId,
                UserBalanceId = balance.Id,
                ExpressionType = ExpressionType.Subtract
            };

            var status = await _paymentService.CreatePayment(payment);

            if (status == PaymentStatus.Completed)
            {
                balance.Balance -= SubstractAmount;
                _dbContext.Update(balance);
                await _dbContext.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return balance.Balance;

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<decimal> GetBalanceAsync()
    {
        var userId = _httpContextAccessor.GetUserId();
        var balance = await _dbContext.UserBalances.FirstOrDefaultAsync(x => x.UserId == userId)
            ?? throw new NullReferenceException();

        return balance.Balance;
    }
}

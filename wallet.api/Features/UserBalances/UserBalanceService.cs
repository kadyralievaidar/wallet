using System.Linq.Expressions;
using wallet.api.Features.Core;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.DataAccess.UOW;
using wallet.api.Features.Payment;
using wallet.api.Features.Payment.Dtos;

namespace wallet.api.Features.UserBalances;

public class UserBalanceService : IUserBalanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    private const decimal StartAmount = 8.0M;
    private const decimal SubstractAmount = 1.1M;

    public UserBalanceService(IUnitOfWork unitOfWork, IPaymentService paymentService, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<decimal> SetBalanceAsync(Guid userId)
    {
        var balance = new UserBalance()
        {
            Balance = StartAmount,
            UserId = userId,
            Id = Guid.NewGuid()
        };
        await _unitOfWork.UserBalanceRepository.AddAsync(balance);

        var payment = new PaymentDto()
        {
            Amount = SubstractAmount,
            UserId = userId,
            UserBalanceId = balance.Id,
            ExpressionType = ExpressionType.Add
        };

        await _paymentService.CreatePayment(payment);

        return balance.Balance;
    }
    public async Task<decimal> GetBalanceAsync()
    {
        var userId = _httpContextAccessor.GetUserId();
        var balance = await _unitOfWork.UserBalanceRepository.GetByUserId(userId)
            ?? throw new NullReferenceException();

        var paymentDto = new PaymentDto()
        {
            UserBalanceId = balance.Id,
            Amount = SubstractAmount,
            UserId = userId,
            ExpressionType = ExpressionType.Subtract
        };
        var status = await _paymentService.CreatePayment(paymentDto);
        if(status == PaymentStatus.Completed)
        {
            balance.Balance -= SubstractAmount;
            balance.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.UserBalanceRepository.Update(balance);
        }
        return balance.Balance;
    }
}

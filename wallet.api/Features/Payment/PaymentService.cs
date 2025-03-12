using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using wallet.api.Features.Core;
using wallet.api.Features.DataAccess;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.Payment.Dtos;

namespace wallet.api.Features.Payment;

public class PaymentService : IPaymentService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly WalletDbContext _dbContext;
    private readonly IMapper _mapper;

    public PaymentService(IHttpContextAccessor contextAccessor, WalletDbContext dbContext, IMapper mapper)
    {
        _contextAccessor = contextAccessor;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaymentStatus> CreatePayment(PaymentDto paymentDto)
    {
        using var transaction = _dbContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        try
        {
            var userBalance = await _dbContext.UserBalances.FirstOrDefaultAsync(x => x.Id == paymentDto.UserBalanceId);
            var payment = _mapper.Map<PaymentEntity>(paymentDto);
            var status = PaymentStatus.Pending;

            if (paymentDto.ExpressionType == ExpressionType.Subtract)
            {
                if (userBalance!.Balance - paymentDto.Amount > 0)
                    status = PaymentStatus.Completed;
                else
                    status = PaymentStatus.Failed;
            }

            if (paymentDto.ExpressionType == ExpressionType.Add)
                status = PaymentStatus.Completed;

            payment.UserId = _contextAccessor.GetUserId();
            payment.PaymentStatus = status;
            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            await transaction.RollbackAsync();
            return payment.PaymentStatus;

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

using System.Linq.Expressions;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.DataAccess.UOW;
using wallet.api.Features.Payment.Dtos;

namespace wallet.api.Features.Payment;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentStatus> CreatePayment(PaymentDto paymentDto)
    {
        var userBalance = await _unitOfWork.UserBalanceRepository.GetByIdWithouthTracking(paymentDto.UserBalanceId);
        var payment = new PaymentEntity()
        {
            UserId = paymentDto.UserId,
            UserBalanceId = paymentDto.UserBalanceId,
            Amount = paymentDto.Amount
        };
        var status = PaymentStatus.Pending;
        if (userBalance != null)
        {
            if (paymentDto.ExpressionType == ExpressionType.Subtract)
            {
                if (userBalance!.Balance - paymentDto.Amount > 0)
                    status = PaymentStatus.Completed;
                else
                    status = PaymentStatus.Failed;
            }

            if (paymentDto.ExpressionType == ExpressionType.Add)
                status = PaymentStatus.Completed;
        }

        payment.PaymentStatus = status;
        payment.CreatedAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.PaymentRepository.AddAsync(payment);
        return payment.PaymentStatus;
    }
}

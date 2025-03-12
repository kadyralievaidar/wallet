using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.Payment.Dtos;

namespace wallet.api.Features.Payment;

public interface IPaymentService
{
    Task<PaymentStatus> CreatePayment(PaymentDto paymentDto);
}

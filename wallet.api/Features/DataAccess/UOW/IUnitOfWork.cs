using wallet.api.Features.DataAccess.Repositories;

namespace wallet.api.Features.DataAccess.UOW;

public interface IUnitOfWork
{
    IPaymentRepository PaymentRepository { get; }
    IUserBalanceRepository UserBalanceRepository { get; }
}

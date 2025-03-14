using wallet.api.Features.DataAccess.Models;

namespace wallet.api.Features.DataAccess.Repositories;

public class PaymentRepository : GenericRepository<PaymentEntity>, IPaymentRepository
{
    public PaymentRepository(WalletDbContext context) : base(context)
    {
    }
}

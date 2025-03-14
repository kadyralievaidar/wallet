using wallet.api.Features.DataAccess.Models;

namespace wallet.api.Features.DataAccess.Repositories;

public interface IUserBalanceRepository : IGenericRepository<UserBalance>
{
    Task<UserBalance> GetByUserId(Guid userId);

    Task<UserBalance> GetByIdWithouthTracking(Guid userBalanceId);
}

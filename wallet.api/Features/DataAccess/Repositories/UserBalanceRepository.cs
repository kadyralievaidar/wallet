using Microsoft.EntityFrameworkCore;
using wallet.api.Features.DataAccess.Models;

namespace wallet.api.Features.DataAccess.Repositories;

public class UserBalanceRepository : GenericRepository<UserBalance>, IUserBalanceRepository
{
    private readonly WalletDbContext _context;
    public UserBalanceRepository(WalletDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<UserBalance> GetByIdWithouthTracking(Guid userBalanceId)
    {
        var userBalance = await _context.UserBalances.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userBalanceId);
        return userBalance;
    }

    public async Task<UserBalance> GetByUserId(Guid userId)
    {
        var userBalance = await _context.UserBalances.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
        return userBalance;
    }
}

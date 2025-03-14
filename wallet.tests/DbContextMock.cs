using Microsoft.EntityFrameworkCore;
using wallet.api.Features.DataAccess;

namespace wallet.tests;
public class DbContextMock : WalletDbContext
{
    public DbContextMock(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}


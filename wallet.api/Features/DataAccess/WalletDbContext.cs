using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wallet.api.Features.DataAccess.Models;

namespace wallet.api.Features.DataAccess;

public class WalletDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }
    public DbSet<PaymentEntity> Payments { get; set; }
    public DbSet<UserBalance> UserBalances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .HasKey(l => new { l.LoginProvider, l.ProviderKey });
    }
}

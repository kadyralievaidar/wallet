using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace wallet.api.Features.DataAccess.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
{
    public GenericRepository(WalletDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    /// <summary>
    ///     Database context
    /// </summary>
    public WalletDbContext Context { get; }

    /// <summary>
    ///     DbSet
    /// </summary>
    public DbSet<TEntity> DbSet { get; }
    public async Task AddAsync(TEntity entity)
    {
        using var transaction = Context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        try
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync(); ;
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public IEnumerable<TEntity> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        var result = DbSet.Where(filter);
        return result.ToList();
    }

    public void Remove(TEntity entityToDelete)
    {
        if (Context.Entry(entityToDelete).State == EntityState.Detached)
            DbSet.Attach(entityToDelete);

        DbSet.Remove(entityToDelete);
    }

    public virtual async void Update(TEntity entityToUpdate)
    {
        Context.Entry(entityToUpdate).State = EntityState.Detached;
        DbSet.Attach(entityToUpdate);
        Context.Entry(entityToUpdate).State = EntityState.Modified;
        DbSet.Update(entityToUpdate);
        await Context.SaveChangesAsync();
    }
    public virtual TEntity? GetById(params object[]? ids) => ids == null ? null : DbSet.Find(ids);
    public virtual bool Any(Expression<Func<TEntity, bool>> predicate) => DbSet.Any(predicate);
}


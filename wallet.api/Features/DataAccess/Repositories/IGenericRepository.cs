using System.Linq.Expressions;

namespace wallet.api.Features.DataAccess.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class, new()
{
    Task AddAsync(TEntity entity);
    void Remove(TEntity entityToDelete);
    TEntity? GetById(params object[]? ids);
    IEnumerable<TEntity> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
    /// <summary>
    ///     Update an entity in the repository
    /// </summary>
    /// <param name="entityToUpdate">Entity to update</param>
    void Update(TEntity entityToUpdate);

    /// <summary>
    ///     Presence of records by condition
    /// </summary>
    /// <param name="predicate">Filter for selection condition</param>
    bool Any(Expression<Func<TEntity, bool>> predicate);
}

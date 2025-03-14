using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using wallet.api.Features.DataAccess;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.DataAccess.Repositories;
using wallet.api.Features.DataAccess.UOW;
using wallet.tests;

namespace OrderManagmentTests;
internal class UnitOfWorkFactory : IDisposable
{
    internal SqliteConnection _connection;

    internal DbContextMock _context;

    internal UnitOfWorkFactory()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<WalletDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        _context = new DbContextMock(options);

        ClearData();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _connection?.Dispose();
        _context.Database.EnsureDeleted();
        _context?.Dispose();
    }

    internal void AddData<T>(List<T> data) where T : BaseModel
    {
        var type = typeof(DbContextMock);
        var prop = type.GetProperties().FirstOrDefault(x => x.PropertyType == typeof(DbSet<T>));
        var getter = prop.GetGetMethod();

        var dbSet = (DbSet<T>)getter.Invoke(_context, null);
        dbSet.AddRange(data);

        _context.SaveChanges();
    }

    private void ClearData()
    {
        if (_context.Database.EnsureCreated())
        {
            foreach (var entity in _context.UserBalances)
            {
                _context.UserBalances.Remove(entity);
                _context.SaveChanges();
            }
            foreach (var entity in _context.Payments)
            {
                _context.Payments.Remove(entity);
                _context.SaveChanges();
            }

            _context.SaveChanges();
        }
    }

    internal IUnitOfWork CreateUnitOfWork()
    {
        var paymentRepository = new Mock<IPaymentRepository>();
        var userBalanceRepository = new Mock<IUserBalanceRepository>();

        return new UnitOfWork(paymentRepository.Object, userBalanceRepository.Object, _context);
    }
}
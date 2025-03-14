using wallet.api.Features.DataAccess.Repositories;

namespace wallet.api.Features.DataAccess.UOW;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly Lazy<IPaymentRepository> _paymentRepository;
    private readonly Lazy<IUserBalanceRepository> _userBalanceRepository;

    private readonly WalletDbContext _context;

    public UnitOfWork
       (IPaymentRepository paymentRepository,
       IUserBalanceRepository userBalanceRepository,
       WalletDbContext context)
    {
        _paymentRepository = new Lazy<IPaymentRepository>(paymentRepository);
        _userBalanceRepository = new Lazy<IUserBalanceRepository>(userBalanceRepository);
        _context = context;
    }

    public IPaymentRepository PaymentRepository => _paymentRepository.Value;
    public IUserBalanceRepository UserBalanceRepository => _userBalanceRepository.Value;
    ~UnitOfWork()
    {
        Dispose(false);
    }
    private void Dispose(bool disposing)
    {
        if (disposing)
            _context.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

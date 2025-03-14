using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OrderManagmentTests;
using wallet.api.Features.DataAccess.UOW;
using wallet.api.Features.Payment;
using wallet.api.Features.UserBalances;

namespace wallet.tests;

[TestFixture]
public class UserBalanceServiceTests
{
    private IUserBalanceService _userBalanceService;
    private Mock<IPaymentService> _paymentService;
    private Mock<IHttpContextAccessor> _contextAccessor;
    private UnitOfWorkFactory _factory;
    private IUnitOfWork _unitOfWork;

    private Guid UserId = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        _paymentService = new Mock<IPaymentService>();
        _contextAccessor = HttpContextMock.CreateMockHttpContextAccessor(UserId);
        _factory = new UnitOfWorkFactory();
        _unitOfWork = _factory.CreateUnitOfWork();
        _userBalanceService = new UserBalanceService(_unitOfWork, _paymentService.Object, _contextAccessor.Object);
    }
    [Test]
    public async Task SetUpBalance()
    {
        var userId = Guid.NewGuid();

        var result = await _userBalanceService.SetBalanceAsync(userId);

        result.Should().Be(8.0M);
    }
}

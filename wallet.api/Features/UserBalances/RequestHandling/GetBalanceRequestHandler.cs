using MediatR;

namespace wallet.api.Features.UserBalances.RequestHandling;

public class GetBalanceRequestHandler : IRequestHandler<GetBalanceRequest, decimal>
{
    private IUserBalanceService _userBalanceService;
    private readonly ILogger<GetBalanceRequestHandler> _logger;

    public GetBalanceRequestHandler(IUserBalanceService userBalanceService, ILogger<GetBalanceRequestHandler> logger)
    {
        _userBalanceService = userBalanceService;
        _logger = logger;
    }

    public async Task<decimal> Handle(GetBalanceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userBalanceService.GetBalanceAsync();
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}

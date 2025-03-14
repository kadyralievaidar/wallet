using MediatR;

namespace wallet.api.Features.UserBalances.RequestHandling;

public class GetBalanceRequest : IRequest<decimal>
{
}

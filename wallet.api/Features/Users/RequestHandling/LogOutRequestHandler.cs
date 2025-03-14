using MediatR;

namespace wallet.api.Features.Users.RequestHandling;

public class LogOutRequestHandler : IRequestHandler<LogOutRequest>
{
    private readonly IIdentityUserService _identityUserService;

    public LogOutRequestHandler(IIdentityUserService identityUserService)
    {
        _identityUserService = identityUserService;
    }

    public async Task Handle(LogOutRequest request, CancellationToken cancellationToken)
    {
        await _identityUserService.SignOut();
    }
}

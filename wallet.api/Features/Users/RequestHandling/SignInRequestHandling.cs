using MediatR;

namespace wallet.api.Features.Users.RequestHandling;

public class SignInRequestHandling : IRequestHandler<SignInRequest, string>
{
    private readonly ILogger<RegisterUserRequestHandler> _logger;
    private readonly IIdentityUserService _service;

    /// <summary>
    ///     Initialize a new instance
    /// </summary>
    public SignInRequestHandling(IServiceProvider serviceProvider)
    {
        _service = serviceProvider.GetRequiredService<IIdentityUserService>();
        _logger = serviceProvider.GetRequiredService<ILogger<RegisterUserRequestHandler>>();
    }

    /// <summary>
    ///     Handler for request register user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationCustomException"></exception>
    public async Task<string> Handle(SignInRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.SignIn(request.Dto, cancellationToken);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}

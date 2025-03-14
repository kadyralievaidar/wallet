﻿using MediatR;
using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users.RequestHandling;


public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, CreateUserDto>
{
    private readonly ILogger<RegisterUserRequestHandler> _logger;
    private readonly IIdentityUserService _service;

    /// <summary>
    ///     Initialize a new instance
    /// </summary>
    public RegisterUserRequestHandler(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<RegisterUserRequestHandler>>();
        _service = serviceProvider.GetRequiredService<IIdentityUserService>();
    }

    /// <summary>
    ///     Handler for request register user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationCustomException"></exception>
    public async Task<CreateUserDto> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateUser(request.Dto);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Identity;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.Users.Dtos;

namespace wallet.api.Features.Users.RequestHandling;


public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, CreateUserDto>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterUserRequestHandler> _logger;

    /// <summary>
    ///     Initialize a new instance
    /// </summary>
    public RegisterUserRequestHandler(IServiceProvider serviceProvider)
    {
        _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
        _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _logger = serviceProvider.GetRequiredService<ILogger<RegisterUserRequestHandler>>();
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
            var user = new ApplicationUser
            {
                Email = request.Dto.Email
            };
            var result = await _userManager.CreateAsync(user, request.Dto.Password);
            if (!result.Succeeded)
                throw new Exception();

            await _signInManager.SignInAsync(user, isPersistent: true);
            return request.Dto;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}

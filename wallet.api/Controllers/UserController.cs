using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.Users.Dtos;
using wallet.api.Features.Users.RequestHandling;

namespace wallet.api.Controllers;

public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly SignInManager<ApplicationUser> _signInManager;

    /// <summary>
    ///     Initialize a new instance
    /// </summary>
    public UserController(IMediator mediator, SignInManager<ApplicationUser> signInManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }
    /// <summary>
    ///     User registration 
    /// </summary>
    /// <param name="model">Gets the model from form</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("register")]
    [ProducesResponseType(typeof(CreateUserDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Register(CreateUserDto model, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new RegisterUserRequest(model), cancellationToken));

    /// <summary>
    ///     The logout endpoint
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> LogOff()
    {
        await _mediator.Send(new LogOutRequest());
        return Ok();
    }

    /// <summary>
    ///     The logout endpoint
    /// </summary>
    [HttpPost("sign-in")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> SignIn(CreateUserDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new SignInRequest(dto), cancellationToken));
    }
}

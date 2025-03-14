using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wallet.api.Features.UserBalances.RequestHandling;

namespace wallet.api.Controllers;
public class UserBalanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserBalanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("GetBalance")]
    public async Task<IActionResult> GetBalance(CancellationToken cancellation)
    {
        return Ok(await _mediator.Send(new GetBalanceRequest(), cancellation));
    }
}

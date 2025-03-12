using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace wallet.api.Controllers;
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult GetBalance()
    {
        return Ok();
    }
    [HttpPost]
    public IActionResult Pay()
    {
        return Ok();
    }
}

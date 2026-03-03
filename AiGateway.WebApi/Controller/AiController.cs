using AiGateway.Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AiGateway.WebApi.Controller;
[ApiController]
[Route("api/[controller]")]
public class AiController:ControllerBase
{
    private readonly IMediator _mediator;

    public AiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskAiCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);
        
        return Ok(response);
        
    }
    
}
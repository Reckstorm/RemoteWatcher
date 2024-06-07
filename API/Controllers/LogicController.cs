using Api.Controllers;
using Application.Logic;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LogicController : BaseApiController
{
    [HttpPost("start")]
    public async Task<IActionResult> PostStart()
    {
        return HandleResult(await Mediator.Send(new Start.Command()));
    }

    [HttpPost("stop")]
    public async Task<IActionResult> PostStop()
    {
        return HandleResult(await Mediator.Send(new Stop.Command()));
    }
}
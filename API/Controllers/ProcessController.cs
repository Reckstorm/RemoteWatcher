using Api.Controllers;
using Application.Processes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProcessController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return HandleResult(await Mediator.Send(new List.Query()));
    }

    [HttpGet("details/{processName}")]
    public async Task<IActionResult> GetDetails(string processName)
    {
        return HandleResult(await Mediator.Send(new Details.Query { ProcessName = processName }));
    }

    [HttpPost("kill/{processName}")]
    public async Task<IActionResult> PostKill(string processName)
    {
        return HandleResult(await Mediator.Send(new Kill.Command { ProcessName = processName }));
    }
}
using System.Reflection.Metadata.Ecma335;
using Application.RProcesses;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public class RProcessController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return HandleResult(await Mediator.Send(new List.Query()));
    }

    [HttpPost]
    public async Task<IActionResult> Post(RProcess process)
    {
        return HandleResult(await Mediator.Send(new Add.Command { Process = process }));
    }

    [HttpDelete("{processName}")]
    public async Task<IActionResult> Delete(string processName)
    {
        return HandleResult(await Mediator.Send(new Delete.Command { ProcessName = processName }));
    }

    [HttpDelete("deleteAll")]
    public async Task<IActionResult> DeleteAll()
    {
        return HandleResult(await Mediator.Send(new DeleteAll.Command { }));
    }

    [HttpGet("details/{processName}")]
    public async Task<IActionResult> GetProcess(string processName)
    {
        return HandleResult(await Mediator.Send(new Details.Query { ProcessName = processName }));
    }

    [HttpPut("edit/{processName}")]
    public async Task<IActionResult> EditProcess(string processName, RProcess process)
    {
        return HandleResult(await Mediator.Send(new Edit.Command { ProcessName = processName, Process = process }));
    }

    [HttpPut("editAll")]
    public async Task<IActionResult> EditAll(RProcessDTO Boundaries)
    {
        return HandleResult(await Mediator.Send(new EditAll.Command { Boundaries = Boundaries }));
    }
}
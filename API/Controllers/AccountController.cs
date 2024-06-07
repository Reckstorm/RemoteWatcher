using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = new Domain.User();
        if (loginDto == null) return Unauthorized();
        if (await EncryptorService.SimpleEnc(loginDto.Password) == await EncryptorService.SimpleEnc(user.Password))
            return Ok(new UserDto { Name = user.Name, Token = new TokenService().CreateToken(user) });
        return Unauthorized();
    }
}
using StreamNest.API.ActionFilters;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace StreamNest.API.Controllers
{
[Route("api/token")]
[ApiController]
public class TokenController : ControllerBase 
{
    private readonly IServiceManager _service;

    public TokenController(IServiceManager service)
    {
        _service = service;
    }
    [HttpPost("refresh")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
     var tokenDtoReturn = await _service.AuthenticationService.RefreshToken(tokenDto);

     return Ok(tokenDtoReturn);
    }
}
}
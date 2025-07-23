using StreamNest.API.ActionFilters;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="tokenDto">The expired access token and corresponding refresh token.</param>
        /// <returns>Returns a new access token and refresh token pair.</returns>
        /// <response code="200">Returns the refreshed token</response>
        /// <response code="400">If the input is invalid</response>
        /// <response code="401">If the refresh token is invalid or expired</response>
    [HttpPost("refresh")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(Summary = "Refresh token", Description = "Generates a new access token using a valid refresh token.")]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
     var tokenDtoReturn = await _service.AuthenticationService.RefreshToken(tokenDto);

     return Ok(tokenDtoReturn);
    }
}
}
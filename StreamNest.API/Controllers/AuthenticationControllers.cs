using Microsoft.AspNetCore.Mvc;
using StreamNest.API.ActionFilters;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;


namespace StreamNest.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthenticationController(IServiceManager service)
        {
            _service = service;
        }
        [HttpPost("Register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult>
        RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var result = await _service.AuthenticationService.RegisterUser(userForRegistration);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    var key = string.IsNullOrWhiteSpace(error.Code) ? "Registration" : error.Code;
                    ModelState.TryAddModelError(key, error.Description);
                }
                return ValidationProblem(ModelState);
            }
            return StatusCode(201);
        }  
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            var isValid = await _service.AuthenticationService.ValidateUser(user);
            if (!isValid)
            {
                ModelState.AddModelError("Authentication", "Invalid email or password.");
                return ValidationProblem(ModelState);
            }

            var tokenDto = await _service.AuthenticationService.CreateToken(populateExp: true);
            return Ok(tokenDto);
        }  
    }
}
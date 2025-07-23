using Microsoft.AspNetCore.Mvc;
using StreamNest.API.ActionFilters;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using Swashbuckle.AspNetCore.Annotations;


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
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userForRegistration">The registration details.</param>
        /// <returns>201 Created if successful, 400 Bad Request if validation fails.</returns>
        [HttpPost("Register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new user account with the provided email and password."
        )]
        [SwaggerResponse(201, "User registered successfully")]
        [SwaggerResponse(400, "Validation errors occurred during registration")]
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
        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="user">User login credentials.</param>
        /// <returns>JWT access token and refresh token if valid.</returns>
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [SwaggerOperation(
            Summary = "Login with email and password",
            Description = "Authenticates the user and issues a JWT access token and refresh token."
        )]
        [SwaggerResponse(200, "Login successful, token returned", typeof(TokenDto))]
        [SwaggerResponse(400, "Invalid email or password")]
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
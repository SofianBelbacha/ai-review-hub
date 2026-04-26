using AiReviewHub.Application.Users.Commands.GenerateRefreshToken;
using AiReviewHub.Application.Users.Commands.GoogleLogin;
using AiReviewHub.Application.Users.Commands.LoginUser;
using AiReviewHub.Application.Users.Commands.RegisterUser;
using AiReviewHub.Application.Users.Commands.RevokeToken;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiReviewHub.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName);

            var result = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RefreshTokenCommand(request.Token),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new RevokeTokenCommand(request.Token, request.RevokeAll),
                cancellationToken);

            return NoContent();
        }

        [HttpGet("google")]
        [AllowAnonymous]
        public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback),
                    new { returnUrl }),
                Items = { { "returnUrl", returnUrl ?? "/" } }
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string? returnUrl = null,
            CancellationToken cancellationToken = default)
        {
            // Récupère les infos Google depuis le cookie temporaire
            var authenticateResult = await HttpContext
                .AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return Unauthorized("Google authentication failed");

            var claims = authenticateResult.Principal?.Claims;

            var email = claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var firstName = claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var googleId = claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (email is null || googleId is null)
                return Unauthorized("Missing required Google claims");

            var result = await _mediator.Send(
                new GoogleLoginCommand(
                    email,
                    firstName ?? string.Empty,
                    lastName ?? string.Empty,
                    googleId),
                cancellationToken);

            // Retourne les tokens JWT — le frontend les stocke
            return Ok(result);
        }
    }

    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);

    public record LoginRequest(string Email, string Password);
    public record RefreshTokenRequest(string Token);
    public record RevokeTokenRequest(string Token, bool RevokeAll = false);


}

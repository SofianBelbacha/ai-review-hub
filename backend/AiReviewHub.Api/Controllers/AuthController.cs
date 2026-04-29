using AiReviewHub.Api.Extensions;
using AiReviewHub.Application.Users.Commands.GenerateRefreshToken;
using AiReviewHub.Application.Users.Commands.GoogleLogin;
using AiReviewHub.Application.Users.Commands.LoginUser;
using AiReviewHub.Application.Users.Commands.RegisterUser;
using AiReviewHub.Application.Users.Commands.RevokeToken;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
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

            // Refresh token → cookie httpOnly
            HttpContext.SetRefreshTokenCookie(result.RefreshToken);

            // Access token → body uniquement
            return Created(string.Empty, new
            {
                result.AccessToken,
                result.Id,
                result.Email,
                result.FirstName,
                result.LastName,
                result.Plan
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await _mediator.Send(command, cancellationToken);
            HttpContext.SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { result.AccessToken });

        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            // Refresh token lu depuis le cookie
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh token not found");

            var result = await _mediator.Send(
                new RefreshTokenCommand(refreshToken),
                cancellationToken);

            HttpContext.SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { result.AccessToken });
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return NoContent();

            await _mediator.Send(
                new RevokeTokenCommand(refreshToken, request.RevokeAll),
                cancellationToken);

            // Efface le cookie
            HttpContext.ClearRefreshTokenCookie();

            return NoContent();
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin(
            [FromBody] GoogleLoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GoogleLoginCommand(request.IdToken),
                cancellationToken);

            HttpContext.SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { result.AccessToken, result.IsNewUser });
        }

    }

    public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
    public record LoginRequest(string Email, string Password);
    public record RevokeTokenRequest(bool RevokeAll = false);
    public record GoogleLoginRequest(string IdToken);


}

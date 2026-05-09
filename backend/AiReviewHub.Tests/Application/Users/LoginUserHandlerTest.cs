using AiReviewHub.Application.Abstractions;
using AiReviewHub.Application.Users.Commands.LoginUser;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Infrastructure.Persistence;
using AiReviewHub.Tests.Helpers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Tests.Application.Users
{
    public class LoginUserHandlerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly FakeDateTimeProvider _clock;
        private readonly Mock<IPasswordHasher> _passwordHasher;
        private readonly Mock<ITokenService> _tokenService;
        private readonly LoginUserHandler _handler;

        public LoginUserHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _clock = new FakeDateTimeProvider();
            _passwordHasher = new Mock<IPasswordHasher>();
            _tokenService = new Mock<ITokenService>();

            var (refreshTokenEntity, rawToken) =
                RefreshToken.Create(Guid.NewGuid(), _clock.UtcNow);

            _tokenService
                .Setup(t => t.PrepareSession(
                    It.IsAny<User>(),
                    It.IsAny<DateTime>()))
                .Returns(new SessionResult("access-token", rawToken, refreshTokenEntity));

            _handler = new LoginUserHandler(_context, _passwordHasher.Object, _clock, _tokenService.Object);
        }

        [Fact]
        public async Task Handle_WithValidCredentials_ShouldReturnTokens()
        {
            // Arrange
            var user = EntityBuilders.BuildUser("jean@example.com");
            _context.Users.Add(user);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _passwordHasher
                .Setup(p => p.Verify("Password123!", It.IsAny<string>()))
                .Returns(true);

            var command = new LoginUserCommand("jean@example.com", "Password123!");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.AccessToken.Should().Be("access-token");
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Handle_WithWrongPassword_ShouldThrowUnauthorized()
        {
            // Arrange
            var user = EntityBuilders.BuildUser("jean@example.com");
            _context.Users.Add(user);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _passwordHasher
                .Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false); // mauvais mot de passe

            var command = new LoginUserCommand("jean@example.com", "WrongPassword");

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("*Invalid credentials*");
        }

        [Fact]
        public async Task Handle_WithNonExistentEmail_ShouldThrowUnauthorized()
        {
            // Arrange
            var command = new LoginUserCommand("inconnu@example.com", "Password123!");

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("*Invalid credentials*");
        }

        [Fact]
        public async Task Handle_WithOAuthUser_ShouldThrowUnauthorized()
        {
            // Arrange — user créé via Google (pas de mot de passe)
            var user = User.CreateWithGoogle(
                "google@example.com", "Jean", "Dupont",
                "google-id-123", _clock.UtcNow);
            _context.Users.Add(user);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var command = new LoginUserCommand("google@example.com", "Password123!");

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("*Google*");
        }

        [Fact]
        public async Task Handle_WithEmailCaseInsensitive_ShouldSucceed()
        {
            // Arrange
            var user = EntityBuilders.BuildUser("jean@example.com");
            _context.Users.Add(user);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            _passwordHasher
                .Setup(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            // Email en majuscules
            var command = new LoginUserCommand("JEAN@EXAMPLE.COM", "Password123!");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("access-token");
        }

        public void Dispose() => _context.Dispose();
    }
}

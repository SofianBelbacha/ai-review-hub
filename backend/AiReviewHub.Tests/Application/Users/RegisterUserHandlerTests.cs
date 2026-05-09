using AiReviewHub.Application.Abstractions;
using AiReviewHub.Application.Users.Commands.LoginUser;
using AiReviewHub.Application.Users.Commands.RegisterUser;
using AiReviewHub.Domain.Entities;
using AiReviewHub.Domain.Exceptions;
using AiReviewHub.Infrastructure.Persistence;
using AiReviewHub.Tests.Helpers;
using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Tests.Application.Users
{
    public class RegisterUserHandlerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly FakeDateTimeProvider _clock;
        private readonly Mock<IPasswordHasher> _passwordHasher;
        private readonly Mock<ITokenService> _tokenService;
        private readonly Mock<IMapper> _mapper;
        private readonly RegisterUserHandler _handler;

        public RegisterUserHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _clock = new FakeDateTimeProvider();
            _passwordHasher = new Mock<IPasswordHasher>();
            _tokenService = new Mock<ITokenService>();
            _mapper = new Mock<IMapper>();


            // Setup par défaut du password hasher
            _passwordHasher
                .Setup(p => p.Hash(It.IsAny<string>()))
                .Returns("$2a$12$hashedpasswordXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

            var (refreshTokenEntity, rawToken) =
                RefreshToken.Create(Guid.NewGuid(), _clock.UtcNow);


            // Setup par défaut du token service
            _tokenService
                .Setup(t => t.PrepareSession(
                    It.IsAny<User>(),
                    It.IsAny<DateTime>()))
                .Returns(new SessionResult("access-token", rawToken, refreshTokenEntity));

            _handler = new RegisterUserHandler(
                _context,
                _clock,
                _passwordHasher.Object,
                _tokenService.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateUser()
        {
            // Arrange
            var expectedResult = new RegisterUserResult(
                Guid.NewGuid(), "jean@example.com",
                "Jean", "Dupont", "Free", "access-token", "refresh-token",
                _clock.UtcNow);

            _mapper.Setup(m => m.Map<RegisterUserResult>(It.IsAny<User>()))
                   .Returns(expectedResult);

            var command = new RegisterUserCommand(
                "jean@example.com", "Password123!", "Jean", "Dupont");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("jean@example.com");

            var userInDb = _context.Users.Single();
            userInDb.Email.Value.Should().Be("jean@example.com");
            userInDb.FirstName.Should().Be("Jean");
            userInDb.LastName.Should().Be("Dupont");
        }

        [Fact]
        public async Task Handle_WithDuplicateEmail_ShouldThrowConflictException()
        {
            // Arrange — user existant avec le même email
            var existingUser = EntityBuilders.BuildUser("jean@example.com");
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var command = new RegisterUserCommand(
                "jean@example.com", "Password123!", "Jean", "Dupont");

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                     .WithMessage("*already in use*");
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldHashPassword()
        {
            // Arrange
            _mapper.Setup(m => m.Map<RegisterUserResult>(It.IsAny<User>()))
                   .Returns(new RegisterUserResult(
                       Guid.NewGuid(), "jean@example.com",
                       "Jean", "Dupont", "Free", "access-token", "refresh-token",
                       _clock.UtcNow));

            var command = new RegisterUserCommand(
                "jean@example.com", "Password123!", "Jean", "Dupont");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert — le password hasher a été appelé avec le bon mot de passe
            _passwordHasher.Verify(
                p => p.Hash("Password123!"),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateSession()
        {
            // Arrange
            _mapper.Setup(m => m.Map<RegisterUserResult>(It.IsAny<User>()))
                   .Returns(new RegisterUserResult(
                       Guid.NewGuid(), "jean@example.com",
                       "Jean", "Dupont", "Free", "access-token", "refresh-token",
                       _clock.UtcNow));

            var command = new RegisterUserCommand(
                "jean@example.com", "Password123!", "Jean", "Dupont");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert — le token service a été appelé
            _tokenService.Verify(
                t => t.PrepareSession(
                    It.IsAny<User>(),
                    _clock.UtcNow),
                Times.Once);
        }

        public void Dispose() => _context.Dispose();
    }
}
